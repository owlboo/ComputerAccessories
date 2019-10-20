using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputerAccessories.Models;

namespace ComputerAccessories.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainServiceController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        public MainServiceController(ComputerAccessoriesContext db)
        {
            _db = db;
        }
        #region Front
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Category()
        {
            var listCategory = _db.TblCategory.ToList();
            ViewBag.controller = "Category";
            return View(listCategory);
        }
        public IActionResult Brand()
        {
            return View();
        }
        public IActionResult CreateNewCategory()
        {
            return View();
        }
        public IActionResult EditCategory(int? id)
        {
            if(id == null)
            {
                    return NotFound();
            }
            var categoryFromDb = _db.TblCategory.Where(x => x.Id == id).FirstOrDefault();
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(Category category)
        {
            _db.TblCategory.Add(new Category
            {
                CategoryName = category.CategoryName,
                CreatedDate = DateTime.Now
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if(category == null)
            {
                return NotFound();
            }
            var categoryFromDb = _db.TblCategory.Where(x => x.Id == category.Id).FirstOrDefault();
            categoryFromDb.CategoryName = category.CategoryName;
            categoryFromDb.ModifiedDate = DateTime.Now;
            var result = await _db.SaveChangesAsync();
            if(result > 0)
            {
                return RedirectToAction(nameof(Category));
            }
            return null;
        }
    }
}