using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly SignInManager<MyUsers> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public ShoppingController(ComputerAccessoriesV2Context db, SignInManager<MyUsers> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductListFilter(int? categoryId = null, int? brandId = null)
        {
            var context = new QueryDbContext();
            var listProducts = _db.Products.Where(x => x.CategoryId == categoryId || x.BrandId == brandId).Select(x =>
                new ProductGridModel
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId ?? x.CategoryId.Value,
                    CategoryName = x.Category.CategoryName,
                    BrandId = x.BrandId ?? x.BrandId.Value,
                    BrandName = x.Brand.BrandName,
                    OriginalPrice = x.OriginalPrice.Value.ToString("###,###"),
                    ProductImages = _db.ProductImages.Where(z => z.ProductId == z.Id).ToList(),
                    Quantity = x.Quantity.Value,
                    Thumnail = x.Thumnail,
                    Thumnail2 = x.Thumnail2,
                    ShorDescription =x.ShorDescription
                }).ToList();
            string str =
                @"SELECT c.Id,c.CategoryName,(SELECT Count(id) FROM dbo.Products WHERE CategoryId = c.Id) 'ProductQuantity' FROM dbo.Category c WHERE c.Status = 1";

            var listCategory = context.CategoryShoppingModel.FromSqlRaw(str).Select(x => new CategoryShoppingModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                ProductQuantity = x.ProductQuantity.HasValue ? x.ProductQuantity.Value : 0
            }).ToList();
            ViewBag.category = listCategory;
            return View(listProducts);
        }

        [Route("/[controller]/GetCategories")]
        public IActionResult GetCategories()
        {
            var context = new QueryDbContext();
            string str =
                @"SELECT c.Id,c.CategoryName,(SELECT Count(id) FROM dbo.Products WHERE CategoryId = c.Id) 'ProductQuantity' FROM dbo.Category c WHERE c.Status = 1";

            var listCategory = context.CategoryShoppingModel.FromSqlRaw(str).Select(x=> new CategoryShoppingModel
            {
                Id=x.Id,
                CategoryName = x.CategoryName,
                ProductQuantity = x.ProductQuantity.HasValue ? x.ProductQuantity.Value : 0
            }).ToList();

            return View(listCategory);
        }

        public IActionResult CheckOut()
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value).ToString();
            var context = new QueryDbContext();
            if (!_signInManager.IsSignedIn(User))
            {
                return View();
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.Append(
                @"SELECT u.Id 'UserId',u.DisplayName,u.PhoneNumber,ua.ProvinceId,(SELECT ProvinceName FROM dbo.Provinces WHERE ProvinceId=ua.ProvinceId)'ProvinceName',ua.DistrictId,(SELECT DistrictName FROM dbo.Districts WHERE DistrictId = ua.DistrictId) 'DistrictName', ua.WardId, (SELECT WardName FROM dbo.Ward WHERE WardId = ua.WardId) 'WardName',ua.PlaceDetails FROM dbo.AspNetUsers u LEFT JOIN dbo.UserAddress ua ON ua.UserId = u.Id WHERE u.Id = @userId");

            var userInfo = context.UserInformationModels.FromSqlRaw(sqlQuery.ToString(), new SqlParameter("userId", userId)).FirstOrDefault();

            ViewBag.UserInfo = userInfo;


            //Get session list product order
            string key = "SessionSP_" + userId;
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = 0;
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
                    var productPrice = _db.Products.Where(x => x.Id == item.Products.Id).Select(x => x.OriginalPrice).FirstOrDefault();
                    totalPrice += item.Quantity * (productPrice.HasValue ? productPrice.Value : 0);
                }

                previewCart.TotalPrice = totalPrice;
            }

            ViewBag.listProduct = previewCart;



            return View();
        }
    }
}