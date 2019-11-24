using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Helpers;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainServiceController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        [BindProperty]
        public CategoryViewModel CategoryVM { get; set; }
        [BindProperty]
        public AttributeViewModel AttributeVM { get; set; }
        [BindProperty]
        public BrandViewModel BrandVM { get; set; }
        private readonly UserManager<MyUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public AccountViewModel AccountVM { get; set; }
        public MainServiceController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            CategoryVM = new CategoryViewModel
            {
                listParent = _db.Category.ToList()
            };
            AttributeVM = new AttributeViewModel();
            _userManager = userManager;
            _roleManager = roleManager;
            //AccountVM.Roles = _db.AspNetRoles.ToList();
        }

        public IActionResult Index()
        {
            return View();
        }
        #region Category

        public PartialViewResult _GetCategory()
        {
            var listCategory = _db.Category.ToList();
            ViewBag.lstCategory = listCategory;
            return PartialView("~/Views/Admin/_GetCategory.cshtml", listCategory);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(CategoryViewModel categoryView)
        {
            _db.Category.Add(new Category
            {
                CategoryName = categoryView.CategoryName,
                CreatedDate = DateTime.Now,
                ParendId = categoryView.ParentCateId,
                Status = categoryView.Status
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }



        public IActionResult Category()
        {
            var listCategory = _db.Category.Where(x => x.Id != 2).ToList();
            List<CategoryViewModel> lstCategory = new List<CategoryViewModel>();

            foreach (var item in listCategory)
            {
                if (item.ParendId != null)
                {
                    var parentName = _db.Category.Where(x => x.Id == item.ParendId).Select(x => x.CategoryName).FirstOrDefault();

                    CategoryViewModel category = new CategoryViewModel()
                    {
                        CategoryName = item.CategoryName,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        ParentName = parentName,
                        Id = item.Id,
                        Status = item.Status == null ? false : item.Status.Value
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
            return Json(_db.Category.Where(x => x.Id != 2).ToList());
        }

        public IActionResult CreateNewCategory()
        {
            return View(CategoryVM);
        }

        public IActionResult CreateNewAttribute()
        {
            var categories = _db.Category.ToList();
            ViewBag.lstCategory = categories;
            return View(AttributeVM);
        }
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryFromDb = _db.Category.Where(x => x.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            CategoryVM.CategoryName = categoryFromDb.CategoryName;
            CategoryVM.ParentCateId = categoryFromDb.ParendId;
            CategoryVM.CreatedDate = categoryFromDb.CreatedDate;
            CategoryVM.ModifiedDate = categoryFromDb.ModifiedDate;
            CategoryVM.Id = categoryFromDb.Id;
            CategoryVM.Status = categoryFromDb.Status == null ? false : categoryFromDb.Status.Value;

            return View(CategoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (category == null)
            {
                return NotFound();
            }
            var categoryFromDb = _db.Category.Where(x => x.Id == category.Id).FirstOrDefault();
            categoryFromDb.CategoryName = category.CategoryName;
            categoryFromDb.ModifiedDate = DateTime.Now;
            categoryFromDb.ParendId = category.ParendId;
            categoryFromDb.Status = category.Status;
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
            var listCategory = _db.Category.ToList();
            ViewBag.lstCategory = listCategory;
            if (string.IsNullOrEmpty(categoryId))
            {
                if (string.IsNullOrEmpty(fromTime) || string.IsNullOrEmpty(toTime))
                {
                    var lstAttributes = _db.Category.Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.Id,
                        AttributeName = x.Attributes.FirstOrDefault().AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.CategoryName
                    }).ToList();
                    return View(lstAttributes);
                }
                else
                {
                    DateTime from = DateTime.Parse(fromTime);
                    DateTime to = DateTime.Parse(toTime + " 23:59:59");
                    var lstAttributes = _db.Attributes.Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new AttributeViewModel
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
                    var lstAttributes = _db.Attributes.Where(x => x.CategoryId == categoryid).Select(x => new AttributeViewModel
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
                    var lstAttributes = _db.Attributes.Where(x => x.CreatedDate >= from && x.CreatedDate <= to && x.CategoryId == categoryid).Select(x => new AttributeViewModel
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

            Models.Attributes att = new Models.Attributes
            {
                AttributeName = model.AttributeName,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.Now
            };

            _db.Attributes.Add(att);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Attributes));
        }


        public IActionResult EditAttributes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var attributeFromDb = _db.Attributes.Where(x => x.Id == id).FirstOrDefault();
            if (attributeFromDb == null)
            {
                return NotFound();
            }
            var categories = _db.Category.ToList();
            ViewBag.lstCategory = categories;
            AttributeVM.Id = attributeFromDb.Id;
            AttributeVM.CategoryId = attributeFromDb.CategoryId;
            AttributeVM.CreatedDate = attributeFromDb.CreatedDate;
            AttributeVM.ModifiedDate = attributeFromDb.ModifiedDate;
            return View(AttributeVM);
        }
        #endregion
        #region AccountManger
        [Route("MainService/GetRoles")]
        public JsonResult GetRoles()
        {
            return Json(_db.AspNetRoles.ToArray());
        }
        public IActionResult AccountManager(string role)
        {

            List<AspNetRoles> lstRoles = new List<AspNetRoles>();

            List<Provinces> listProvinces = new List<Provinces>();

            lstRoles =_db.AspNetRoles.ToList();

            listProvinces = _db.Provinces.ToList();
           
            if (lstRoles.Count > 0)
            {
                ViewBag.listRole = lstRoles;
            }
            else
            {
                var r = new AspNetRoles
                {
                    Name = "",
                    Id = 0
                };
                lstRoles.Add(r);
                ViewBag.listRole = lstRoles;
                
            }

            ViewBag.controller = "CustomerAccount";
            ViewBag.listProvinces = listProvinces;

            if (role == null)
            {
                var listUser = (IEnumerable<AccountViewModel>)(from us in _db.AspNetUsers
                                                               join us_role in _db.AspNetUserRoles
                                                               on us.Id equals us_role.UserId
                                                               select new AccountViewModel
                                                               {
                                                                   Id = us.Id,
                                                                   DisplayName = us.DisplayName,
                                                                   IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
                                                                   Email = us.Email,
                                                                   Role = us_role.RoleId,
                                                                   RoleName = _db.AspNetRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
                                                                   CreatedDate = us.CreatedDate                                                                  
                                                               }).ToList();
                return View(listUser);
            }
            else
            {
                var listUser = (from us in _db.AspNetUsers
                                join us_role in _db.AspNetUserRoles
                                on us.Id equals us_role.UserId
                                join r in _db.AspNetRoles
                                on us_role.RoleId equals r.Id
                                where r.Name.Equals(role)
                                select new AccountViewModel
                                {
                                    Id = us.Id,
                                    DisplayName = us.DisplayName,
                                    IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
                                    Email = us.Email,
                                    Role = us_role.RoleId,
                                    RoleName = _db.AspNetRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
                                    CreatedDate = us.CreatedDate
                                }).AsQueryable();
                return View(listUser.ToList()); ;
            }

        }

        [HttpPost]
        [Route("MainService/CreateNewUser")]
        public async Task<IActionResult> CreateNewUser([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
                Match match = regex.Match(model.Email.Trim().ToLower());
                if (!match.Success)
                {
                    return new JsonResult(new { code = 0, Err = "" });
                }
                
                var user = new MyUsers
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CodeConfirm = AccountHelpers.GenerateCodeConfirm(),
                    IsActivated = true,
                    LockoutEnabled = false,
                    CreatedDate = DateTime.Now,
                    DisplayName = model.FullName
                };
                var roleInDb = _db.AspNetRoles.Where(x => x.Id == Int32.Parse(model.RoleId)).FirstOrDefault();
                
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(true);
                if (result.Succeeded)
                {

                    var useraddress = new UserAddress
                    {
                        UserId = user.Id,
                        WardId = model.WardId,
                        ProvinceId = model.ProvinceId,
                        DistrictId = model.DistrictId,
                        PlaceDetails = model.PlaceDetail + _db.Ward.Find(model.WardId).WardName + "," + _db.Districts.Find(model.DistrictId).DistrictName + "," + _db.Provinces.Find(model.ProvinceId).ProvinceName
                    };
                    _db.UserAddress.Add(useraddress);
                    await _db.SaveChangesAsync();
                    #region Assign to Role, default Customer
                    var resultRole = new IdentityResult();
                    if (roleInDb == null)
                    {
                        if (!await _roleManager.RoleExistsAsync(SD.Customer))
                        {
                            await _roleManager.CreateAsync(new IdentityRole<int> { Name = SD.Customer });
                        }
                        resultRole = await _userManager.AddToRoleAsync(user, SD.Customer);
                        if (resultRole.Succeeded)
                        {

                            return RedirectToAction(nameof(AccountManager));
                        }
                        else
                        {
                            return Json(new { code = 0, Err = "Không thể gán role cho user:" + user.UserName + "" });
                        }
                    }
                    else
                    {
                        resultRole = await _userManager.AddToRoleAsync(user, roleInDb.Name);
                        if (resultRole.Succeeded)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            return Json(new { code = 1 });
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return Json(new { code = 0, Err = "Không thể gán role cho user:" + user.UserName + "" });
                        }
                    }
                                                        
                    #endregion
                }

            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new JsonResult(new { code = 0, Err = "*Có lỗi xảy ra, vui lòng thử lại" });
        }

        public IActionResult EditCustomerAccount(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var accountFromDb = _db.AspNetUsers.Join(_db.AspNetUserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { Users = u, UserRole = ur }).Join(_db.AspNetRoles, f => f.UserRole.RoleId, r => r.Id, (f, r) => new { UserRole = f, Role = r }).Where(z => z.UserRole.Users.Id == id).Select(z=>new AccountViewModel { 
                Id = z.UserRole.Users.Id,
                DisplayName =z.UserRole.Users.DisplayName,
                Email =z.UserRole.Users.Email,
                IsActivated = z.UserRole.Users.IsActivated.Value ==true? "Kích hoạt" : "Ẩn/Chưa kích hoạt",
                CreatedDate = z.UserRole.Users.CreatedDate,
                RoleName = z.Role.Name
            }).FirstOrDefault();
            if(accountFromDb == null)
            {
                return NotFound();
            }
            return View(accountFromDb);
        }
        #endregion
    }
}
