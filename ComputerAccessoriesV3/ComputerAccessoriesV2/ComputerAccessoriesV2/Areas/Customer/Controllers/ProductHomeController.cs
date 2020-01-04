using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.DI;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductHomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly UserManager<MyUsers> _userManager;
        private readonly QueryDbContext context;
        private readonly IRedis _redis;
        
        public ProductHomeController(ComputerAccessoriesV2Context db,QueryDbContext _context, UserManager<MyUsers> userManager, IRedis redis)
        {
            context = _context;
            _db = db;
            _userManager = userManager;
            _redis = redis;
        }
        public IActionResult ProductDetails(int productId)
        {
            ProductGridModel products = _db.Products
                .Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y })
                .Where(c => c.x.Id == productId)
                .Select(c => new ProductGridModel
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
                ViewCounts = int.Parse(_redis.GetValue(Constants.CACHE_PRODUCT_CURRENT_VIEWING_PREFIX + productId, "0")),
                ReviewStarPoint = c.x.Reviews.Average(x => x.Star).Value,
                ReviewCount = c.x.Reviews.Count(),
                ProductImages =  c.x.ProductImages.ToList(),
                ProductAttributes = _db.ProductAttribute.Where(z => z.ProductId == productId).Include(z => z.Attribute).ToList(),
                FullDescription = c.x.FullDescription

            }).FirstOrDefault();


            List<ProductGridModel> deals = _db.Products.Where(x => x.PromotionPrice.HasValue).Select(x => new ProductGridModel
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
                SaleValue = (100 - (Decimal)(x.PromotionPrice / x.OriginalPrice) * 100).ToString("###")
            }).Take(20).ToList();

            List<Products> relatedProducts = _db.Products
                .Where(x => x.CategoryId == products.CategoryId || x.BrandId == products.BrandId).ToList();

            string brandStr = @"SELECT b.Id,b.BrandName,(SELECT COUNT(Id) FROM dbo.Products WHERE BrandId=b.Id)'ProductCount' FROM dbo.Brand b WHERE 1=1";
            var brandList = context.BrandPartials.FromSqlRaw(brandStr).ToList();

            string str =
               @"SELECT c.Id,c.CategoryName,(SELECT Count(id) FROM dbo.Products WHERE CategoryId = c.Id) 'ProductQuantity' FROM dbo.Category c WHERE c.Status = 1";
            var listCategory = context.CategoryShoppingModel.FromSqlRaw(str).ToList().OrderByDescending(x=>x.ProductQuantity).Take(5).ToList();

            ViewBag.ListDeal = deals;
            ViewBag.brandList = brandList;
            ViewBag.productName = products.ProductName;
            ViewBag.relatedProduct = relatedProducts;
            ViewBag.ListTags = listCategory;

            _redis.Publish(Constants.REDIS_PS_USER_COUNT_PRODUCT_PREFIX_CHANNEL + productId, "1");
            _redis.GetRedisBD().StringIncrement(Constants.CACHE_PRODUCT_CURRENT_VIEWING_PREFIX + productId);
            return View(products);
        }

        //public IActionResult RelatedProducts(int categoryId, int brandId)
        //{
        //    ProductGridModel relatedProducts = _db.Products.Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y }).Where(c => c.x.BrandId == brandId || c.x.CategoryId == categoryId).Select(c => new ProductGridModel
        //    {
        //        Id = c.x.Id,
        //        ProductName = c.x.ProductName,
        //        BrandId = c.x.BrandId.Value,
        //        BrandName = c.x.Brand.BrandName,
        //        CategoryId = c.x.CategoryId.Value,
        //        CategoryName = c.x.Category.CategoryName,
        //        OriginalPrice = c.x.OriginalPrice.Value.ToString("###,###"),
        //        Thumnail = c.x.Thumnail,
        //        Thumnail2 = c.x.Thumnail2,
        //        Code = c.x.Code,
        //        Color = c.x.Color,
        //        ShorDescription = c.x.ShorDescription,
        //        Status = c.x.Status,
        //        ViewCounts = c.x.ViewCounts,
        //        ProductImages = _db.ProductImages.Where(z => z.ProductId == c.x.Id).ToList(),
        //        ProductAttributes = _db.ProductAttribute.Where(z => z.ProductId == c.x.Id).Include(z => z.Attribute).ToList()
        //    }).FirstOrDefault();

        //    return View(relatedProducts);
        //}

        [HttpGet]
        public async Task<IActionResult> ProductReview(int productId)
        {
            /*            var reviews = (
                            from r in _db.Reviews
                            where r.ProductId == productId
                            select r
                            ).ToList();

                        switch(sortType)
                        {
                            case 1:
                                reviews.Sort((x, y) => DateTime.Compare(x.CreatedDate, y.CreatedDate));
                                break;
                            case 2:
                                reviews.Sort((x, y) => (x.LikedNumber >= y.LikedNumber ? 1 : 0));
                                break;
                            case 3:
                                reviews.Sort((x, y) => (x.LikedNumber >= y.LikedNumber ? 0 : 1));
                                break;
                            default:
                                break;
                        }*/
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser != null)
            {
                ViewBag.CurrentUserId = (await _userManager.GetUserAsync(User)).Id;
            } else
            {
                ViewBag.CurrentUserId = null;
            }
            ViewBag.ProductId = productId;
            return View();
        }
    }
}