using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using ComputerAccessoriesV2.Data;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class HomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly SignInManager<MyUsers> _signInManager;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(ComputerAccessoriesV2Context db, IHttpContextAccessor httpContextAccessor, SignInManager<MyUsers> signInManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            try
            {
                _db.Database.SetCommandTimeout(99999);
                ProductHomeViewModel products = new ProductHomeViewModel();

                products.ListNewProducts = _db.Products.Join(_db.Category, x => x.CategoryId, y => y.Id, (x, y) => new { x, y })
                                                        .Join(_db.Brand, z => z.x.BrandId, b => b.Id, (z, b) => new { z, b })
                                                        .Where(c => c.z.x.IsNew.HasValue && c.z.x.IsNew == true).Select(c => new ProductGridModel
                                                        {
                                                            Id = c.z.x.Id,
                                                            ProductName = c.z.x.ProductName,
                                                            Thumnail = c.z.x.Thumnail,
                                                            Thumnail2 =c.z.x.Thumnail2,
                                                            BrandId = c.b.Id,
                                                            BrandName = c.b.BrandName,
                                                            CategoryId = c.z.y.Id,
                                                            CategoryName = c.z.y.CategoryName,
                                                            OriginalPrice = c.z.x.OriginalPrice.Value,
                                                            Code = c.z.x.Code,
                                                            IsNew = c.z.x.IsNew.HasValue ? c.z.x.IsNew.Value : false,
                                                        }).Take(20).ToList();

                products.ListMostViewsProducts = _db.Products.Join(_db.Category, x => x.CategoryId, y => y.Id, (x, y) => new { x, y })
                                                            .Join(_db.Brand, z => z.x.BrandId, b => b.Id, (z, b) => new { z, b })
                                                            .OrderByDescending(c => c.z.x.ViewCounts).Select(c => new ProductGridModel
                                                            {
                                                                Id = c.z.x.Id,
                                                                ProductName = c.z.x.ProductName,
                                                                Thumnail = c.z.x.Thumnail,
                                                                Thumnail2 = c.z.x.Thumnail2,
                                                                BrandId = c.b.Id,
                                                                BrandName = c.b.BrandName,
                                                                CategoryId = c.z.y.Id,
                                                                CategoryName = c.z.y.CategoryName,
                                                                OriginalPrice = c.z.x.OriginalPrice.Value,
                                                                Code = c.z.x.Code,
                                                                IsNew = c.z.x.IsNew.HasValue ? c.z.x.IsNew.Value : false,
                                                            }).Take(20).ToList();


                //footer brand slider

                List<Brand> brands = _db.Brand.ToList();
                ViewBag.brandsFooter = brands;

                //Load all components
                
                return View(products);
            }
            catch (Exception)
            {

                throw;
            }
            //var listProducts = _db.Products.Include(x => x.Brand).Include(x => x.Category).ToList();

           
        }

        [Route("/[controller]/Categories")]
        public IActionResult Categories()
        {
            List<CategoryHomeModel> categoryModel = new List<CategoryHomeModel>();
            categoryModel = _db.Category.Where(x=>x.ParendId == null).Select(x => new CategoryHomeModel
            {
                MainCategory = _db.Category.Where(z => z.Id == x.Id).FirstOrDefault(),

            }).ToList();

            foreach (var category in categoryModel)
            {
                category.ListChildrenNode = _db.Category.Where(x => x.ParendId == category.MainCategory.Id).Select(x => new CategoryHomeModel {
                    MainCategory = _db.Category.Where(z => z.Id == x.Id).FirstOrDefault(),
                    ListChildrenNode = _db.Category.Where(z=>z.ParendId == x.Id).Select(z=>new CategoryHomeModel { 
                        MainCategory = _db.Category.Where(c=>c.Id ==z.Id).FirstOrDefault()
                    }).ToList()
                }).ToList();
                //foreach (var child in category.ListChildrenNode1)
                //{
                //    category.ListChildrendNode2 = _db.Category.Where(x => x.ParendId == child.Id).ToList();
                //}

            }
            return View(categoryModel);
        }

        [Route("/Home/QuickView")]
        public JsonResult QuickView(int id)
        {
            ProductGridModel products = _db.Products.Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y }).Where(c => c.x.Id == id).Select(c => new ProductGridModel
            {
                Id = c.x.Id,
                ProductName = c.x.ProductName,
                BrandId = c.x.BrandId.Value,
                BrandName = c.x.Brand.BrandName,
                CategoryId = c.x.CategoryId.Value,
                CategoryName = c.x.Category.CategoryName,
                OriginalPrice = c.x.OriginalPrice.Value,
                Thumnail = c.x.Thumnail,
                Thumnail2 = c.x.Thumnail2,
                Code = c.x.Code,
                Color = c.x.Color,
                ShorDescription = c.x.ShorDescription,
                Status = c.x.Status,
                ViewCounts = c.x.ViewCounts,
                ProductImages = _db.ProductImages.Where(z => z.ProductId == id).ToList()

            }).FirstOrDefault();
            //ViewBag.products = products;
            return Json(products);
            //return View(products);
        }

        [Route("/[controller]/AddToCart")]
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {

            //if (!_signInManager.IsSignedIn(User))
            //{
            //    return RedirectToAction("Account", "SignIn");
            //}

            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if(user == null)
            {
                return Json(new {code =0, returnUrl ="/Customer/Account/SignIn" });
            }

            var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var productFromDb = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
            var ListShoppingCart = new List<ShoppingCartViewModel>();
            int sum = 0;
            #region write session
            string sskey = "SessionSP_" + currentUser;
            if(_session.GetString(sskey) == null)
            {
                var obj = new ShoppingCartViewModel
                {
                    Products = _db.Products.Where(x=>x.Id==productId).FirstOrDefault(),
                    Quantity = quantity
                };
                ListShoppingCart.Add(obj);
                sum = ListShoppingCart.Count;
                _session.SetString(sskey, JsonConvert.SerializeObject(ListShoppingCart));
            }
            else
            {
                ListShoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(sskey));
                if(!ListShoppingCart.Any(x=>x.Products.Id == productId))
                {
                    ListShoppingCart.Add(new ShoppingCartViewModel
                    {
                        Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                        Quantity = quantity
                    });
                }
                else
                {
                    foreach (var item in ListShoppingCart)
                    {
                        if (item.Products.Id == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }

                }
                sum = ListShoppingCart.Count;

                _session.SetString(sskey, JsonConvert.SerializeObject(ListShoppingCart));
            }
            #endregion
            return Json(new { code = 1,Name = productFromDb.ProductName, quantity = quantity,sum=sum });
        }

        [Route("/[controller]/ShoppingCartDrop")]
        [HttpPost]
        public IActionResult ShoppingCartDrop()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            string key = "SessionSP_" + userId;
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart =new ShoppingCartPreview();
            Decimal totalPrice = 0;
            if(_session.GetString(key) == null)
            {
                return View(previewCart);
            }
            else
            {
                listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                previewCart.ListProducts = listProducts;
                foreach (var item in previewCart.ListProducts)
                {
                    var productPrice = _db.Products.Where(x => x.Id == item.Products.Id).Select(x => x.OriginalPrice).FirstOrDefault();
                    totalPrice += item.Quantity * (productPrice.HasValue ? productPrice.Value:0);
                }
                previewCart.TotalPrice = totalPrice;
                return View(previewCart);
            }
        }

        [Route("/[controller]/UpdateQuantity")]
        [HttpPost]
        public JsonResult UpdateQuantity(int productId, int quantity,int currentTotalPrice, bool IsSub)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            string key = "SessionSP_" + userId;
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = currentTotalPrice;
            if (_session.GetString(key) == null)
            {
                return Json(new { code = 0 });
            }
            else
            {
                listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                foreach (var item in listProducts)
                {
                    if(item.Products.Id == productId)
                    {
                        item.Quantity = quantity;
                    }
                }
                var unitPrice = _db.Products.Where(x => x.Id == productId).Select(x => x.OriginalPrice).FirstOrDefault();
                if (IsSub)
                {
                    totalPrice -= unitPrice.Value;
                }
                else
                {
                    totalPrice += unitPrice.Value;
                }
                
                _session.SetString(key, JsonConvert.SerializeObject(listProducts));
                return Json(new { code = 1, totalPrice = totalPrice });
            }
        }

        [Route("/[controller]/RemoveProductFromCart")]
        [HttpPost]
        public JsonResult RemoveProductFromCart(int productId, int currentTotalPrice)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            string key = "SessionSP_" + userId;
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = currentTotalPrice;
            if(_session.GetString(key) == null)
            {
                return Json(new { code = 0 });
            }
            else
            {
                
                listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                var item = listProducts.Where(x => x.Products.Id == productId).FirstOrDefault();
                var unitPrice = _db.Products.Where(x => x.Id == productId && x.OriginalPrice.HasValue).Select(x => x.OriginalPrice).FirstOrDefault();
                Decimal price = unitPrice.Value * item.Quantity;
                totalPrice -= price;
                listProducts.Remove(item);
                _session.SetString(key, JsonConvert.SerializeObject(listProducts));
                return Json(new { code = 1, totalPrice = totalPrice, name = item.Products.ProductName, counter = listProducts.Count });
            }
        }

    }
}