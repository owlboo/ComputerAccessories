using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductHomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly UserManager<MyUsers> _userManager;

        public ProductHomeController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult ProductDetails(int productId)
        {
            ProductGridModel products = _db.Products.Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y }).Where(c => c.x.Id == productId).Select(c => new ProductGridModel
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
                ProductImages = _db.ProductImages.Where(z => z.ProductId == productId).ToList(),
                ProductAttributes = _db.ProductAttribute.Where(z => z.ProductId == productId).Include(z => z.Attribute).ToList(),
                FullDescription = c.x.FullDescription

            }).FirstOrDefault();


            List<Products> relatedProducts = _db.Products
                .Where(x => x.CategoryId == products.CategoryId || x.BrandId == products.BrandId).ToList();

            ViewBag.productName = products.ProductName;
            ViewBag.relatedProduct = relatedProducts;
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
            ViewBag.CurrentUserId = (await _userManager.GetUserAsync(User)).Id;
            ViewBag.ProductId = productId;
            return View();
        }
    }
}