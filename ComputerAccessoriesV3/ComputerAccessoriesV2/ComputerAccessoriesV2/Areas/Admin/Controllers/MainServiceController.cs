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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            AttributeVM = new AttributeViewModel();
            _userManager = userManager;
            _roleManager = roleManager;
            CategoryVM = new CategoryViewModel();
            //AccountVM.Roles = _db.AspNetRoles.ToList();
        }

        //[Authorize(Roles =SD.Customer)]
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
        [Route("/[controller]/CreateNewCategory")]
        public async Task<IActionResult> CreateNewCategory(Category model)
        {
            string returnUrl = "/";
            _db.Category.Add(new Category
            {
                CategoryName = model.CategoryName,
                CreatedDate = DateTime.Now,
                ParendId = model.ParendId,
                Status = model.Status
            });
            
            if(await _db.SaveChangesAsync() > 0)
            {
                returnUrl = "/Admin/MainService/Category";
                return Json(new { code = 1, url = returnUrl });
            }
            else
            {
                return Json(new { code = 0, url = returnUrl });
            }
            //return RedirectToAction(nameof(Category));
        }

        [Route("/[controller]/GetCategories")]
        public JsonResult GetCategories()
        {

            var listCategory = _db.Category.ToList();

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
                        Id = item.Id,
                        ParentName = "",
                        Status = item.Status == null ? false : item.Status.Value
                    };
                    lstCategory.Add(category);
                }

            }
            return Json(lstCategory);

        }


        public IActionResult Category()
        {                    
            return View();
        }
        [Route("/[controller]/GetCategory")]
        [HttpGet]
        public JsonResult GetCategory()
        {
            return Json(_db.Category.Select(x => new Category
            {
                Id = x.Id,
                CategoryName = x.CategoryName
            }).ToList());
        }

        

        public IActionResult CreateNewCategory()
        {
            return View();
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
        [Route("/[controller]/updateCategory")]
        public async Task<IActionResult> SaveUpdateCategory(Category category)
        {
            string returnUrl = "/";
            if (category == null || !ModelState.IsValid)
            {
                return Json(new { code = 0, err = "Có lỗi xảy ra trong quá trình cập nhật",url=returnUrl });
            }
            var categoryFromDb = _db.Category.Where(x => x.Id == category.Id).FirstOrDefault();
            categoryFromDb.CategoryName = category.CategoryName;
            categoryFromDb.ModifiedDate = DateTime.Now;
            categoryFromDb.ParendId = category.ParendId;
            categoryFromDb.Status = category.Status;
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                returnUrl = "/Admin/MainService/Category";
                return Json(new { code = 1, err = "", url = returnUrl });
            }
            return null;
        }

        #endregion
        #region Attribute
        [HttpGet]
        [Route("/[controller]/Attributes")]
        public JsonResult GetAttributes(string categoryId = null, string fromTime = null, string toTime = null)
        {
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
                    return Json(lstAttributes);
                }
                else
                {
                    DateTime from = DateTime.Parse(fromTime).ToLocalTime();
                    DateTime to = DateTime.Parse(toTime + " 23:59:59").ToLocalTime();
                    var lstAttributes = _db.Attributes.Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
                    }).ToList();
                    return Json(lstAttributes);
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
                    return Json(lstAttributes);
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
                    return Json(lstAttributes);

                }
            }
        }
        public IActionResult Attributes()
        {
            //ViewBag.controller = "Attributes";
            //var listCategory = _db.Category.ToList();
            //ViewBag.lstCategory = listCategory;
            
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
        [Route("/[controller]/GetRoles")]
        public JsonResult GetRoles()
        {
            return Json(_db.AspNetRoles.ToArray());
        }
        public IActionResult AccountManager(string role)
        {

            //List<AspNetRoles> lstRoles = new List<AspNetRoles>();

            //List<Provinces> listProvinces = new List<Provinces>();

            //lstRoles =_db.AspNetRoles.ToList();

            //listProvinces = _db.Provinces.ToList();

            //if (lstRoles.Count > 0)
            //{
            //    ViewBag.listRole = lstRoles;
            //}
            //else
            //{
            //    var r = new AspNetRoles
            //    {
            //        Name = "",
            //        Id = 0
            //    };
            //    lstRoles.Add(r);
            //    ViewBag.listRole = lstRoles;

            //}

            //ViewBag.controller = "CustomerAccount";
            //ViewBag.listProvinces = listProvinces;

            //if (role == null)
            //{
            //    var listUser = (IEnumerable<AccountViewModel>)(from us in _db.AspNetUsers
            //                                                   join us_role in _db.AspNetUserRoles
            //                                                   on us.Id equals us_role.UserId
            //                                                   select new AccountViewModel
            //                                                   {
            //                                                       Id = us.Id,
            //                                                       DisplayName = us.DisplayName,
            //                                                       IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
            //                                                       Email = us.Email,
            //                                                       Role = us_role.RoleId,
            //                                                       RoleName = _db.AspNetRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
            //                                                       CreatedDate = us.CreatedDate                                                                  
            //                                                   }).ToList();
            //    return View(listUser);
            //}
            //else
            //{
            //    var listUser = (from us in _db.AspNetUsers
            //                    join us_role in _db.AspNetUserRoles
            //                    on us.Id equals us_role.UserId
            //                    join r in _db.AspNetRoles
            //                    on us_role.RoleId equals r.Id
            //                    where r.Name.Equals(role)
            //                    select new AccountViewModel
            //                    {
            //                        Id = us.Id,
            //                        DisplayName = us.DisplayName,
            //                        IsActivated = us.IsActivated.Value == true ? "Kích hoạt" : "Khóa/Chưa kích hoạt",
            //                        Email = us.Email,
            //                        Role = us_role.RoleId,
            //                        RoleName = _db.AspNetRoles.Where(x => x.Id == us_role.RoleId).Select(x => x.NormalizedName).FirstOrDefault(),
            //                        CreatedDate = us.CreatedDate
            //                    }).AsQueryable();
            //    return View(listUser.ToList()); ;
            //}
            return View();

        }

        [Route("/[controller]/GetProvinces")]
        public JsonResult GetProvinces()
        {
            return Json(_db.Provinces.Select(x => new Provinces
            {
                ProvinceId = x.ProvinceId,
                ProvinceName = x.ProvinceName
            }).ToList());
        }


        [Route("/[controller]/GetDistricts")]
        public JsonResult GetDistricts(int provinceId)
        {
            return Json(_db.Districts.Where(x=>x.ProvinceId == provinceId).Select(x => new Districts
            {
                DistrictId = x.DistrictId,
                DistrictName = x.DistrictName
            }).ToList());
        }


        [Route("/[controller]/GetWards")]
        public JsonResult GetWards(int provinceId, int districtId)
        {
            return Json(_db.Ward.Where(x => x.DistrictId == districtId).Select(x=>new Ward { 
                WardId = x.WardId,
                WardName =x.WardName
            }).ToList());
        }
        public string GetFullAddressByUserId (int userId)
        {
            string provinceName = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.Province.ProvinceName).FirstOrDefault();
            string districtName = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.District.DistrictName).FirstOrDefault();
            string wardNAme = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.Ward.WardName).FirstOrDefault();
            return _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.PlaceDetails).FirstOrDefault() + " " + wardNAme + ", " + districtName + ", " + provinceName;
        }

        public string GetRoleNameByUserID(int userid)
        {
            var roleid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x=>x.RoleId).FirstOrDefault();
            return _db.AspNetRoles.Where(x => x.Id == roleid).Select(x => x.Name).FirstOrDefault();
        }

        [Route("/MainService/GetAccount")]
        public JsonResult GetAccount (int? typeAccount = null)
        {
            if(typeAccount == null)
            {
                var listUser = _db.AspNetUsers.Select(x => new AccountGridModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    DisplayName = x.DisplayName,
                    CreatedDate = x.CreatedDate ?? x.CreatedDate.Value,
                    RoleId = _db.AspNetUserRoles.Where(z => z.UserId == x.Id).Select(z => z.RoleId).FirstOrDefault(),
                    RoleName = _db.AspNetUserRoles.Where(z => z.UserId == x.Id).Join(_db.AspNetRoles, ur=>ur.RoleId, r=>r.Id,(ur,r)=>new { ur, r }).Select(z=>z.r.Name).FirstOrDefault(),
                    IsActivated = x.IsActivated ?? x.IsActivated.Value,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address
                }).ToList();
                return Json(listUser);
            }
            else
            {
                return Json(_db.AspNetUsers.Join(_db.AspNetUserRoles,u=>u.Id,y=>y.UserId,(u,y)=>new { u, y }).Select(x => new AccountGridModel
                {
                    Id = x.u.Id,
                    Email = x.u.Email,
                    DisplayName = x.u.DisplayName,
                    CreatedDate = x.u.CreatedDate ?? x.u.CreatedDate.Value,
                    RoleId = x.y.RoleId,
                    RoleName = _db.AspNetRoles.Where(r=>r.Id == x.y.RoleId).Select(r=>r.Name).FirstOrDefault(),
                    IsActivated = x.u.IsActivated ?? x.u.IsActivated.Value,
                    PhoneNumber = x.u.PhoneNumber,
                    Address = x.u.Address

                }).ToList());
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
