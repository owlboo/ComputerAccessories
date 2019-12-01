using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            return View();
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
    }
}