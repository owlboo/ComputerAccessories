using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductHomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        public ProductHomeController(ComputerAccessoriesV2Context db)
        {
            _db = db;
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
                OriginalPrice = c.x.OriginalPrice.Value,
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

            List<ProductGridModel> relatedProducts = _db.Products.Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y }).Where(c => c.x.BrandId == products.BrandId || c.x.CategoryId == products.CategoryId).Select(c => new ProductGridModel
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
                ProductImages = _db.ProductImages.Where(z => z.ProductId == products.Id).ToList(),
                ProductAttributes = _db.ProductAttribute.Where(z => z.ProductId == products.Id).Include(z => z.Attribute).ToList()
            }).Distinct().ToList();

            ViewBag.productName = products.ProductName;
            ViewBag.relatedProduct = relatedProducts;
            return View(products);
        }

        public IActionResult RelatedProducts(int categoryId, int brandId)
        {
            ProductGridModel relatedProducts = _db.Products.Join(_db.ProductImages, x => x.Id, y => y.ProductId, (x, y) => new { x, y }).Where(c => c.x.BrandId == brandId || c.x.CategoryId == categoryId).Select(c => new ProductGridModel
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
                ProductImages = _db.ProductImages.Where(z => z.ProductId == c.x.Id).ToList(),
                ProductAttributes = _db.ProductAttribute.Where(z => z.ProductId == c.x.Id).Include(z => z.Attribute).ToList()
            }).FirstOrDefault();

            return View(relatedProducts);
        }


    }
}