using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputerAccessories.Models;
using ComputerAccessories.ViewModels;

namespace ComputerAccessories.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainServiceController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        [BindProperty]
        public CategoryViewModel CategoryVM { get; set; }
        public MainServiceController(ComputerAccessoriesContext db)
        {
            _db = db;
            CategoryVM = new CategoryViewModel
            {
                listParent = _db.TblCategory.ToList()
            };
        }
        #region Front
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Category()
        {
            var listCategory = _db.TblCategory.ToList();
            List<CategoryViewModel> lstCategory = new List<CategoryViewModel>();

            foreach (var item in listCategory)
            {
                if(item.ParentCateId != null)
                {
                    var parentName = _db.TblCategory.Where(x => x.Id == item.ParentCateId).Select(x => x.CategoryName).FirstOrDefault();

                    CategoryViewModel category = new CategoryViewModel()
                    {
                        CategoryName = item.CategoryName,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        ParentName = parentName,
                        Id = item.Id
                    };
                    lstCategory.Add(category);
                }
                else
                {
                    CategoryViewModel category = new CategoryViewModel()
                    {
                        CategoryName = item.CategoryName,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        Id = item.Id
                    };
                    lstCategory.Add(category);
                }
                
            }
            ViewBag.controller = "Category";
            return View(lstCategory);
        }
        public IActionResult Brand()
        {
            return View();
        }
        public IActionResult CreateNewCategory()
        {

            return View(CategoryVM);
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
            CategoryVM.CategoryName = categoryFromDb.CategoryName;
            CategoryVM.ParentCateId = categoryFromDb.ParentCateId;
            CategoryVM.CreatedDate = categoryFromDb.CreatedDate;
            CategoryVM.ModifiedDate = categoryFromDb.ModifiedDate;
            CategoryVM.Id = categoryFromDb.Id;
 
            return View(CategoryVM);
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(CategoryViewModel categoryView)
        {
            _db.TblCategory.Add(new Category
            {
                CategoryName = categoryView.CategoryName,
                CreatedDate = DateTime.Now,
                ParentCateId = categoryView.ParentCateId,
            }) ;
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
            categoryFromDb.ParentCateId = category.ParentCateId;
            var result = await _db.SaveChangesAsync();
            if(result > 0)
            {
                return RedirectToAction(nameof(Category));
            }
            return null;
        }
    }
}