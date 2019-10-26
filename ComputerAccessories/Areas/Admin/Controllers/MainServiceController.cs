using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputerAccessories.Models;
using ComputerAccessories.ViewModels;
using System.IO;

namespace ComputerAccessories.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainServiceController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        [BindProperty]
        public CategoryViewModel CategoryVM { get; set; }
        [BindProperty]
        public AttributeViewModel AttributeVM { get; set; }
        [BindProperty]
        public BrandViewModel BrandVM { get; set; }
        public MainServiceController(ComputerAccessoriesContext db)
        {
            _db = db;
            CategoryVM = new CategoryViewModel
            {
                listParent = _db.TblCategory.ToList()
            };
            AttributeVM = new AttributeViewModel();
        }
        
        public IActionResult Index()
        {
            return View();
        }
        #region Category
        
        public PartialViewResult _GetCategory()
        {
            var listCategory = _db.TblCategory.ToList();
            ViewBag.lstCategory = listCategory;
            return PartialView("~/Views/Admin/_GetCategory.cshtml", listCategory);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(CategoryViewModel categoryView)
        {
            _db.TblCategory.Add(new TblCategory
            {
                CategoryName = categoryView.CategoryName,
                CreatedDate = DateTime.Now,
                ParentCateId = categoryView.ParentCateId,
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }

        

        public IActionResult Category()
        {
            var listCategory = _db.TblCategory.Where(x=>x.Id !=2).ToList();
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
        [Route("/[controller]/GetCategory")]
        [HttpGet]
        public JsonResult GetCategory()
        {
            return Json(_db.TblCategory.Where(x=>x.Id !=2).ToList());
        }

        public IActionResult CreateNewCategory()
        {

            return View(CategoryVM);
        }

        public IActionResult CreateNewAttribute()
        {
            var categories = _db.TblCategory.ToList();
            ViewBag.lstCategory = categories;
            return View(AttributeVM);
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

        [HttpPost]
        public async Task<IActionResult> EditCategory(TblCategory category)
        {
            if (category == null)
            {
                return NotFound();
            }
            var categoryFromDb = _db.TblCategory.Where(x => x.Id == category.Id).FirstOrDefault();
            categoryFromDb.CategoryName = category.CategoryName;
            categoryFromDb.ModifiedDate = DateTime.Now;
            categoryFromDb.ParentCateId = category.ParentCateId;
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction(nameof(Category));
            }
            return null;
        }
        
        #endregion
        #region Attribute
        [HttpGet]
        public IActionResult Attributes(string categoryId = null, string fromTime = null, string toTime = null)
        {
            ViewBag.controller = "Attributes";
            var listCategory = _db.TblCategory.ToList();
            ViewBag.lstCategory = listCategory;
            if (string.IsNullOrEmpty(categoryId))
            {
                if (string.IsNullOrEmpty(fromTime) || string.IsNullOrEmpty(toTime))
                {
                    var lstAttributes = _db.TblAttribute.Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
                    }).ToList();
                    return View(lstAttributes);
                }
                else
                {
                    DateTime from = DateTime.Parse(fromTime);
                    DateTime to = DateTime.Parse(toTime + " 23:59:59");
                    var lstAttributes = _db.TblAttribute.Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
                    }).ToList();
                    return View(lstAttributes);
                }
            }
            else
            {
                var categoryid = Convert.ToInt32(categoryId);
                if (string.IsNullOrEmpty(fromTime) || string.IsNullOrEmpty(toTime))
                {
                    var lstAttributes = _db.TblAttribute.Where(x => x.CategoryId == categoryid).Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
                    }).ToList();
                    return View(lstAttributes);
                }
                else
                {
                    DateTime from = DateTime.Parse(fromTime);
                    DateTime to = DateTime.Parse(toTime + "23:59:59");
                    var lstAttributes = _db.TblAttribute.Where(x => x.CreatedDate >= from && x.CreatedDate <= to && x.CategoryId == categoryid).Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
                    }).ToList();
                    return View(lstAttributes);

                }
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> CreateNewAttribute(AttributeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            Models.TblAttribute att = new Models.TblAttribute
            {
                AttributeName = model.AttributeName,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.Now
            };

            _db.TblAttribute.Add(att);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Attributes));
        }


        public IActionResult EditAttributes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var attributeFromDb = _db.TblAttribute.Where(x => x.Id == id).FirstOrDefault();
            if (attributeFromDb == null)
            {
                return NotFound();
            }
            var categories = _db.TblCategory.ToList();
            ViewBag.lstCategory = categories;
            AttributeVM.Id = attributeFromDb.Id;
            AttributeVM.CategoryId = attributeFromDb.CategoryId;
            AttributeVM.CreatedDate = attributeFromDb.CreatedDate;
            AttributeVM.ModifiedDate = attributeFromDb.ModifiedDate;
            return View(AttributeVM);
        }
        #endregion
        #region Brand
        public IActionResult Brand()
        {
            ViewBag.controller = "Brand";
            var listBrand = _db.TblBrand.ToList();
            return View(listBrand);
        }

        public IActionResult EditBrand(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandFromDb = _db.TblBrand.Where(x => x.Id == id).FirstOrDefault();
            return View(brandFromDb);
        }

        public IActionResult CreateNewBrand()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditBrand(TblBrand brand)
        {
            if (ModelState.IsValid)
            {
                String currentDir = Directory.GetCurrentDirectory();
                String webRootPath = Path.Combine(currentDir, "wwwroot");
                var file = HttpContext.Request.Form.Files;

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, @"image");
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    var currentBrandFromBd = _db.TblBrand.Where(x => x.Id == brand.Id).FirstOrDefault();
                    currentBrandFromBd.Logo = Path.GetFileName(fileName);
                    currentBrandFromBd.BrandName = brand.BrandName;
                    currentBrandFromBd.ModifiedDate = DateTime.Now;

                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        return RedirectToAction(nameof(Brand));
                    }
                    return null;
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBrand(TblBrand brand)
        {
            if (ModelState.IsValid)
            {
                String currentDir = Directory.GetCurrentDirectory();
                String webRootPath = Path.Combine(currentDir, "wwwroot");
                var file = HttpContext.Request.Form.Files;

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, @"image");
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    _db.TblBrand.Add(new TblBrand
                    {
                        BrandName = brand.BrandName,
                        CreatedDate = DateTime.Now,
                        Logo = Path.GetFileName(fileName)
                    });
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Brand));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }

        }
        #endregion
        #region AccountManger
        public IActionResult AccountManager(int? role)
        {
            if(role == null)
            {
                var listUser = (IEnumerable<AccountViewModel>)(from us in _db.TblUsers
                                                               join us_role in _db.TblUserRole
                                                               on us.Id equals us_role.UserId
                                                               select new AccountViewModel
                                                               {
                                                                   Id = us.Id,
                                                                   DisplayName = us.DisplayName,
                                                                   IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
                                                                   Email = us.Email,
                                                                   Role = us_role.RoleId,
                                                                   RoleName = _db.TblRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
                                                                   CreatedDate = us.CreatedDate
                                                               }).ToList();
                return View(listUser);
            }else
            {
                var listUser = (from us in _db.TblUsers
                                                               join us_role in _db.TblUserRole
                                                               on us.Id equals us_role.UserId
                                                               where us_role.RoleId == role
                                                               select new AccountViewModel
                                                               {
                                                                   Id = us.Id,
                                                                   DisplayName = us.DisplayName,
                                                                   IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
                                                                   Email = us.Email,
                                                                   Role = us_role.RoleId,
                                                                   RoleName = _db.TblRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
                                                                   CreatedDate = us.CreatedDate
                                                               }).AsQueryable();
                return View(listUser.ToList()); ;
            }
            
        }
        #endregion





    }
}