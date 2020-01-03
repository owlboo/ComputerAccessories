using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using ComputerAccessoriesV2.Data;
using Microsoft.Extensions.Caching.Distributed;
using ComputerAccessoriesV2.DI;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class HomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<MyUsers> _signInManager;
        private readonly IRedis _redis;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(ComputerAccessoriesV2Context db, IHttpContextAccessor httpContextAccessor, SignInManager<MyUsers> signInManager, IRedis redis)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _redis = redis;
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
                                                            OriginalPrice = c.z.x.OriginalPrice.Value.ToString("###.###"),
                                                            PromotionPrice=c.z.x.PromotionPrice.HasValue? c.z.x.PromotionPrice.Value.ToString("###,###") :"", 
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
                                                                OriginalPrice = c.z.x.OriginalPrice.Value.ToString("###,###"),
                                                                PromotionPrice=c.z.x.PromotionPrice.HasValue? c.z.x.PromotionPrice.Value.ToString("###,###"):"",
                                                                Code = c.z.x.Code,
                                                                IsNew = c.z.x.IsNew.HasValue ? c.z.x.IsNew.Value : false
                                                            }).Take(20).ToList();

                //footer brand slider
                products.ListHotDealProducts = _db.Products.Where(x => x.PromotionPrice.HasValue).Select(x => new ProductGridModel
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Thumnail = x.Thumnail,
                    Thumnail2 = x.Thumnail2,
                    BrandId = x.BrandId.Value,
                    BrandName = x.BrandId.HasValue ? x.Brand.BrandName : "",
                    CategoryId = x.CategoryId.Value,
                    CategoryName = x.CategoryId.HasValue ? x.Category.CategoryName : "",
                    OriginalPrice = x.OriginalPrice.Value.ToString("###,###"),
                    PromotionPrice = x.PromotionPrice.HasValue ? x.PromotionPrice.Value.ToString("###,###") : "",
                    Code = x.Code,
                    IsNew = x.IsNew.HasValue ? x.IsNew.Value : false,
                    SaleValue = 100-(int)(x.OriginalPrice/x.PromotionPrice)
                }).Take(20).ToList();



                List<ProductGridModel> newArrivals = _db.Products.Select(x => new ProductGridModel
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Thumnail = x.Thumnail,
                    Thumnail2 = x.Thumnail2,
                    BrandId = x.BrandId.Value,
                    BrandName = x.BrandId.HasValue ? x.Brand.BrandName : "",
                    CategoryId = x.CategoryId.Value,
                    CategoryName = x.CategoryId.HasValue ? x.Category.CategoryName : "",
                    OriginalPrice = x.OriginalPrice.Value.ToString("###,###"),
                    PromotionPrice = x.PromotionPrice.HasValue ? x.PromotionPrice.Value.ToString("###,###") : "",
                    Code = x.Code,
                    IsNew = x.IsNew.HasValue ? x.IsNew.Value : false,
                    SaleValue = 100 - (int)(x.OriginalPrice / x.PromotionPrice)
                }).OrderByDescending(x=>x.Id).Take(20).ToList();

                var listNewArrivals = new List<ProductGridModel>();

                foreach (var item in newArrivals)
                {
                    if((DateTime.Now - item.CreatedDate).TotalDays > 10)
                    {
                        listNewArrivals.Add(item);
                    }
                }


                ViewBag.newArrival = listNewArrivals;
                List<Brand> brands = _db.Brand.ToList();
                ViewBag.brandsFooter = brands;
                ViewBag.Index = 1;
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
                OriginalPrice = c.x.OriginalPrice.Value.ToString("###,###"),
                Thumnail = c.x.Thumnail,
                Thumnail2 = c.x.Thumnail2,
                Code = c.x.Code,
                Color = c.x.Color,
                ShorDescription = c.x.ShorDescription,
                Status = c.x.Status,
                ViewCounts = c.x.ViewCounts,
                ProductImages = _db.ProductImages.Where(z => z.ProductId == id).ToList(),
                PromotionPrice = c.x.PromotionPrice.HasValue ? c.x.PromotionPrice.Value.ToString("###,###"):""
                
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
            
            var productFromDb = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
            var ListShoppingCart = new List<ShoppingCartViewModel>();
            int sum = 0;
            try
            {
                if (user == null)
                {
                    //write cookie for customer who do not log on to system
                    string cookieKey = "CookieShopping";
                    var cookie = Request.Cookies[cookieKey];
                    if (cookie == null)
                    {
                        var obj = new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == productId).Select(x => new Products
                            {
                                Id = x.Id,
                                OriginalPrice = x.OriginalPrice,
                                PromotionPrice=x.PromotionPrice,
                                CategoryId = x.CategoryId,
                                Thumnail = x.Thumnail,
                                ProductName = x.ProductName
                            }).FirstOrDefault(),
                            Quantity = quantity
                        };
                        ListShoppingCart.Add(obj);
                        sum = ListShoppingCart.Count;
                        CookieOptions options = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(30),
                            IsEssential = true,
                            HttpOnly=true
                            
                        };
                        //var a = JsonConvert.SerializeObject(ListShoppingCart).ToString();
                        Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(ListShoppingCart).ToString(), options);
                        //_response.Append(cookieKey, JsonConvert.SerializeObject(ListShoppingCart), options);
                        sum = ListShoppingCart.Count;
                    }
                    else
                    {
                        ListShoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                        if(!ListShoppingCart.Any(x=>x.Products.Id == productId))
                        {
                            var obj = new ShoppingCartViewModel
                            {
                                Products = _db.Products.Where(x => x.Id == productId).Select(x => new Products
                                {
                                    Id = x.Id,
                                    OriginalPrice = x.OriginalPrice,
                                    CategoryId = x.CategoryId,
                                    Thumnail = x.Thumnail,
                                    ProductName = x.ProductName,
                                    PromotionPrice=x.PromotionPrice
                                }).FirstOrDefault(),
                                Quantity = quantity
                            };
                            ListShoppingCart.Add(obj);
                            
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
                        Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(ListShoppingCart), new CookieOptions { Expires = DateTime.Now.AddMinutes(30),IsEssential=true});
                    }

                    //return Json(new { code = 0, returnUrl = "/Customer/Account/SignIn" });
                }
                else
                {
                    #region write session
                    var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    string sskey = "SessionSP_" + currentUser;
                    if (_session.GetString(sskey) == null)
                    {
                        var obj = new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                            Quantity = quantity
                        };
                        ListShoppingCart.Add(obj);
                        sum = ListShoppingCart.Count;
                        _session.SetString(sskey, JsonConvert.SerializeObject(ListShoppingCart));
                    }
                    else
                    {
                        ListShoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(sskey));
                        if (!ListShoppingCart.Any(x => x.Products.Id == productId))
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
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 0, returnUrl = "/Customer/Account/SignIn" });
                throw;
            }

            return Json(new { code = 1, Name = productFromDb.ProductName, quantity = quantity, sum = sum });

        }

        [Route("/[controller]/ShoppingCartDrop")]
        [HttpPost]
        public IActionResult ShoppingCartDrop()
        {
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = 0;
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (user == null)
            {
                string cookieKey = "CookieShopping";
                var cookie = Request.Cookies[cookieKey];
                
                if(cookie == null)
                {
                    return View(previewCart);
                }
                else
                {
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                    previewCart.ListProducts = listProducts;
                    foreach (var item in previewCart.ListProducts)
                    {
                        var productPrice = _db.Products.Where(x => x.Id == item.Products.Id).Select(x => x.OriginalPrice).FirstOrDefault();
                        totalPrice += item.Quantity * (productPrice.HasValue ? productPrice.Value : 0);
                    }
                    previewCart.TotalPrice = totalPrice;
                    return View(previewCart);
                }
            }
            else
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string key = "SessionSP_" + userId;

                if (_session.GetString(key) == null)
                {
                    return View(previewCart);
                }
                else
                {
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                    previewCart.ListProducts = listProducts;
                    foreach (var item in previewCart.ListProducts)
                    {
                        var product = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                        totalPrice += item.Quantity * (product.PromotionPrice.HasValue ? product.PromotionPrice.Value : product.OriginalPrice.Value);
                    }
                    previewCart.TotalPrice = totalPrice;
                    return View(previewCart);
                }
            }
            
        }

        [Route("/[controller]/UpdateQuantity")]
        [HttpPost]
        public JsonResult UpdateQuantity(int productId, int quantity,int currentTotalPrice, bool IsSub)
        {
            //var userId = String.IsNullOrEmpty(User.FindFirst(ClaimTypes.NameIdentifier).Value) ? 0 : int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = currentTotalPrice;
            if (user == null)
            {
                string cookieKey = "CookieShopping";
                var cookie = Request.Cookies[cookieKey];
                if(String.IsNullOrEmpty(cookie))
                {
                    return Json(new { code = 0 });
                }
                else
                {
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                    foreach (var item in listProducts)
                    {
                        if (item.Products.Id == productId)
                        {
                            item.Quantity = quantity;
                        }
                    }
                    var unit = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
                    if (IsSub)
                    {
                        totalPrice -= unit.PromotionPrice.HasValue?unit.PromotionPrice.Value:unit.OriginalPrice.Value;
                    }
                    else
                    {
                        totalPrice += unit.PromotionPrice.HasValue? unit.PromotionPrice.Value:unit.OriginalPrice.Value;
                    }

                    Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(listProducts));
                    return Json(new { code = 1, totalPrice = totalPrice });
                }
            }
            else
            {
                var userId = int.Parse(user.Value);
                string key = "SessionSP_" + userId;

                if (_session.GetString(key) == null)
                {
                    return Json(new { code = 0 });
                }
                else
                {
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                    foreach (var item in listProducts)
                    {
                        if (item.Products.Id == productId)
                        {
                            item.Quantity = quantity;
                        }
                    }
                    var unit = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
                    if (IsSub)
                    {
                        totalPrice -= unit.PromotionPrice.HasValue? unit.PromotionPrice.Value:unit.OriginalPrice.Value;
                    }
                    else
                    {
                        totalPrice += unit.PromotionPrice.HasValue ? unit.PromotionPrice.Value : unit.OriginalPrice.Value;
                    }

                    _session.SetString(key, JsonConvert.SerializeObject(listProducts));
                    return Json(new { code = 1, totalPrice = totalPrice });
                }
            }
            
        }

        [Route("/[controller]/RemoveProductFromCart")]
        [HttpPost]
        public JsonResult RemoveProductFromCart(int productId, int currentTotalPrice)
        {
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = currentTotalPrice;
            //var userId = String.IsNullOrEmpty(User.FindFirst(ClaimTypes.NameIdentifier).Value) ? 0 : int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                string cookieKey = "CookieShopping";
                var cookie = Request.Cookies[cookieKey];
                if (String.IsNullOrEmpty(cookie))
                {
                    return Json(new { code = 0 });
                }
                else
                {
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                    var item = listProducts.Where(x => x.Products.Id == productId).FirstOrDefault();
                    var unit = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
                    Decimal price = (unit.PromotionPrice.HasValue?unit.PromotionPrice.Value:unit.OriginalPrice.Value) * item.Quantity;
                    totalPrice -= price;
                    listProducts.Remove(item);
                    Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(listProducts));
                    return Json(new { code = 1, totalPrice = totalPrice, name = item.Products.ProductName, counter = listProducts.Count });
                }
            }
            else
            {
                var userId = int.Parse(user.Value);
                string key = "SessionSP_" + userId;

                if (_session.GetString(key) == null)
                {
                    return Json(new { code = 0 });
                }
                else
                {

                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                    var item = listProducts.Where(x => x.Products.Id == productId).FirstOrDefault();
                    var unit = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
                    Decimal price = (unit.PromotionPrice.HasValue ? unit.PromotionPrice.Value : unit.OriginalPrice.Value) * item.Quantity;
                    totalPrice -= price;
                    listProducts.Remove(item);
                    _session.SetString(key, JsonConvert.SerializeObject(listProducts));
                    return Json(new { code = 1, totalPrice = totalPrice, name = item.Products.ProductName, counter = listProducts.Count });
                }
            }
            
        }

    }
}