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
    
    public class HomeController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        public HomeController(ComputerAccessoriesV2Context db)
        {
            _db = db;
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
        public IActionResult QuickView(int id)
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
            ViewBag.products = products;
            return View();
        }

    }
}