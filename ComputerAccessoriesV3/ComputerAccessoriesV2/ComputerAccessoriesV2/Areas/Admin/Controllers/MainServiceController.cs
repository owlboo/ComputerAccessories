using System;
using System.Collections;
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
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainServiceController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly ApplicationDbContext _ctx;
        [BindProperty]
        public CategoryViewModel CategoryVM { get; set; }
        [BindProperty]
        public AttributeViewModel AttributeVM { get; set; }
        [BindProperty]
        public BrandViewModel BrandVM { get; set; }
        private readonly UserManager<MyUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public AccountViewModel AccountVM { get; set; }
        public MainServiceController(ComputerAccessoriesV2Context db, ApplicationDbContext ctx, UserManager<MyUsers> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            AttributeVM = new AttributeViewModel();
            _userManager = userManager;
            _roleManager = roleManager;
            CategoryVM = new CategoryViewModel();
            _ctx = ctx;
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult Index()
        {
            return View();
        }
        #region Category
        [Authorize(Policy = Policy.AdminAccess)]
        public PartialViewResult _GetCategory()
        {
            var listCategory = _db.Category.ToList();
            ViewBag.lstCategory = listCategory;
            return PartialView("~/Views/Admin/_GetCategory.cshtml", listCategory);
        }

        [Authorize(Policy = Policy.AdminModify)]
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

        [Authorize(Policy = Policy.AdminAccess)]
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

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult Category()
        {                    
            return View();
        }

        [Authorize(Policy = Policy.AdminAccess)]
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

        [Route("/[controller]/GetCategoryDrop")]
        [HttpGet]
        public JsonResult GetCategoryDrop()
        {
            return Json(_db.Category.Select(x => new Category
            {
                Id = x.Id,
                CategoryName = x.CategoryName
            }).ToList());
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult CreateNewCategory()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult CreateNewAttribute()
        {
            var categories = _db.Category.ToList();
            ViewBag.lstCategory = categories;
            return View(AttributeVM);
        }

        [Authorize(Policy = Policy.AdminModify)]
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

        [Authorize(Policy = Policy.AdminModify)]
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
        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        [Route("/[controller]/Attributes")]
        public JsonResult GetAttributes(string categoryId = null, string fromTime = null, string toTime = null)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                if (string.IsNullOrEmpty(fromTime) || string.IsNullOrEmpty(toTime))
                {
                    var lstAttributes = _db.Attributes.Select(x => new AttributeViewModel
                    {
                        Id = x.Id,
                        CategoryId = x.Id,
                        AttributeName = x.AttributeName,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        CategoryName = x.Category.CategoryName
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

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult Attributes()
        {
            //ViewBag.controller = "Attributes";
            //var listCategory = _db.Category.ToList();
            //ViewBag.lstCategory = listCategory;
            
            return View();

        }

        [Authorize(Policy = Policy.AdminModify)]
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

        [Authorize(Policy = Policy.AdminAccess)]
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
        [Authorize(Policy = Policy.AdminModify)]
        [Route("/[controller]/GetRoles")]
        public JsonResult GetRoles()
        {
            return Json(_db.AspNetRoles.ToArray());
        }

        [Authorize(Policy = Policy.AdminAccess)]
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

        [Authorize(Policy = Policy.AdminAccess)]
        public string GetFullAddressByUserId (int userId)
        {
            string provinceName = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.Province.ProvinceName).FirstOrDefault();
            string districtName = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.District.DistrictName).FirstOrDefault();
            string wardNAme = _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.Ward.WardName).FirstOrDefault();
            return _db.UserAddress.Where(x => x.UserId == userId).Select(x => x.PlaceDetails).FirstOrDefault() + " " + wardNAme + ", " + districtName + ", " + provinceName;
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public string GetRoleNameByUserID(int userid)
        {
            var roleid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x=>x.RoleId).FirstOrDefault();
            return _db.AspNetRoles.Where(x => x.Id == roleid).Select(x => x.Name).FirstOrDefault();
        }

        public class AccountFilter
        {
            public string accountEmail { get; set; }
            public string accountPhone { get; set; }
            public int? accountRole { get; set; }
            public int? accountProvince { get; set; }
            public int? accountDistrict { get; set; }
            public int? accountWard { get; set; }
            public string fromTime { get; set; }
            public string toTime { get; set; }
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [Route("/MainService/GetAccount")]
        public JsonResult GetAccount (AccountFilter model)
        {
            var query = _db.AspNetUsers.Include("AspNetUserRoles").Include("UserAddress").AsNoTracking().AsQueryable();
            var predicate = PredicateBuilder.New<AspNetUsers>();
            var from = new DateTime();
            var to = new DateTime();
            if (!String.IsNullOrEmpty(model.accountEmail)){
                predicate = predicate.And(x => x.Email.Equals(model.accountEmail));
            }
            if (!String.IsNullOrEmpty(model.accountPhone))
            {
                predicate = predicate.And(x => x.PhoneNumber.Equals(model.accountPhone));
            }
            if (model.accountRole.HasValue)
            {
                predicate = predicate.And(x => x.AspNetUserRoles.FirstOrDefault().RoleId == model.accountRole.Value);
            }

            if (model.accountProvince.HasValue)
            {
                predicate = predicate.And(x => x.UserAddress.Where(y=>y.ProvinceId ==model.accountProvince.Value).FirstOrDefault()!=null);
            }

            if (model.accountDistrict.HasValue)
            {
                predicate = predicate.And(x => x.UserAddress.Where(y=>y.DistrictId == model.accountDistrict.Value)!=null);
            }

            if (model.accountWard.HasValue)
            {
                predicate = predicate.And(x => x.UserAddress.Where(y=>y.WardId == model.accountWard.Value).FirstOrDefault()!=null);
            }

            if (!String.IsNullOrEmpty(model.fromTime))
            {
                from = DateTime.Parse(model.fromTime);
                predicate = predicate.And(x => x.CreatedDate >= from);
            }

            if (!String.IsNullOrEmpty(model.toTime))
            {
                to = DateTime.Parse(model.toTime);
                predicate = predicate.And(x => x.CreatedDate <= to);
            }
            else
            {
                predicate = predicate.And(x => x.CreatedDate <= DateTime.Now);
            }

            var listUser = query.Where(predicate).Select(x => new AccountGridModel
            {
                Id = x.Id,
                Email = x.Email,
                DisplayName = x.DisplayName,
                CreatedDate = x.CreatedDate ?? x.CreatedDate.Value,
                RoleId = _db.AspNetUserRoles.Where(z => z.UserId == x.Id).Select(z => z.RoleId).FirstOrDefault(),
                RoleName = _db.AspNetUserRoles.Where(z => z.UserId == x.Id).Join(_db.AspNetRoles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r }).Select(z => z.r.Name).FirstOrDefault(),
                IsActivated = x.IsActivated ?? x.IsActivated.Value,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address
            }).OrderByDescending(x=>x.CreatedDate).ToList();


            return Json(listUser);
        }

        private string GetFullAddress(int provinceId, int districtId, int wardId)
        {
            var provinceName = _db.Provinces.Where(x => x.ProvinceId == provinceId).Select(x => x.ProvinceName).FirstOrDefault();
            var districtName = _db.Districts.Where(x => x.DistrictId == districtId).Select(x => x.DistrictName).FirstOrDefault();
            var wardName = _db.Ward.Where(x => x.WardId == wardId).Select(x => x.WardName).FirstOrDefault();

            return wardName + ", " + districtName + ", " + provinceName;
        }

        [Authorize(Policy = Policy.AdminModify)]
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
                string fullAddress = model.PlaceDetail+" "+ GetFullAddress(model.ProvinceId, model.DistrictId, model.WardId);
                var user = new MyUsers
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CodeConfirm = AccountHelpers.GenerateCodeConfirm(),
                    IsActivated = true,
                    LockoutEnabled = false,
                    CreatedDate = DateTime.Now,
                    DisplayName = model.FullName,
                    Address = fullAddress
                };
                var roleInDb = _db.AspNetRoles.Where(x => x.Name == model.RoleName).FirstOrDefault();
                
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    var useraddress = new UserAddress
                    {
                        UserId = user.Id,
                        WardId = model.WardId,
                        ProvinceId = model.ProvinceId,
                        DistrictId = model.DistrictId,
                        PlaceDetails = model.PlaceDetail
                    };
                    _db.UserAddress.Add(useraddress);
                    await _db.SaveChangesAsync();
                    #region Assign to Role, default Customer
                    var resultRole = new IdentityResult();
                    if (roleInDb == null)
                    {
                        if (!await _roleManager.RoleExistsAsync(model.RoleName))
                        {
                            await _roleManager.CreateAsync(new IdentityRole<int> { Name = model.RoleName });
                        }

                        resultRole = await _userManager.AddToRoleAsync(user, model.RoleName);

                        if (resultRole.Succeeded)
                        {
                            return Json(new
                            {
                                code = 1
                            });
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

        [Authorize(Policy = Policy.AdminModify)]
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



        #region campain
        public IActionResult Campaign()
        {
            return View();
        }
        #endregion

        [HttpGet]
        public JsonResult GetAccountReport(String startDate, string endDate)
        {
            if (startDate == null || endDate == null)
            {
                return Json(null);
            }

            var filterStartDate = DateTime.Parse(startDate);
            var filterEndDate = DateTime.Parse(endDate);
            var timeFilterType = filterEndDate.Subtract(filterStartDate).Days < 90 ? 1 : 2;

            var accountList = _db.AspNetUsers
                .Where(x => x.CreatedDate.Value >= filterStartDate && x.CreatedDate.Value <= filterEndDate)
                .ToList();
            var result = new ArrayList();

            if(timeFilterType == 1)
            {
                for (DateTime day = filterStartDate; filterEndDate.CompareTo(day) > 0; day = day.AddDays(1.0))
                {
                    result.Add(new LineChartItem
                    {
                        Value = accountList.Where(x => x.CreatedDate.Value.Date == day.Date).Count(),
                        Date = day.ToString("dd/MM")
                    }); ;
                }
            } 
            else if (timeFilterType == 2)
            {
                for (DateTime month = filterStartDate.Date; filterEndDate.CompareTo(month) >= 0; month = month.AddMonths(1))
                {
                    result.Add(new LineChartItem
                    {
                        Value = accountList.Where(x => x.CreatedDate.Value.Month == month.Month).Count(),
                        Date = month.ToString("MM/yy")
                    }); ;
                }
            }
            
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetSalesReport(String startDate, string endDate)
        {
            if (startDate == null || endDate == null)
            {
                return Json(null);
            }

            var filterStartDate = DateTime.Parse(startDate);
            var filterEndDate = DateTime.Parse(endDate);
            var timeFilterType = filterEndDate.Subtract(filterStartDate).Days < 30 ? 1 : 2;

            var billList = _db.Bills
                .Where(x => x.CreateDate.Value >= filterStartDate && x.CreateDate.Value <= filterEndDate)
                .ToList();
            var result = new ArrayList();

            if (timeFilterType == 1)
            {
                for (DateTime day = filterStartDate; filterEndDate.CompareTo(day) > 0; day = day.AddDays(1.0))
                {
                    result.Add(new LineChartItem
                    {
                        Value = billList.Where(x => x.CreateDate.Value.Date == day.Date).Count(),
                        TotalRevenue = (int)Math.Round(billList.Where(x => x.CreateDate.Value.Date == day.Date).Sum(x => x.TotalPrice).Value/1000),
                        Date = day.ToString("dd/MM")
                    }); ;
                }
            }
            else if (timeFilterType == 2)
            {
                for (DateTime month = filterStartDate.Date; filterEndDate.CompareTo(month) >= 0; month = month.AddMonths(1))
                {
                    result.Add(new LineChartItem
                    {
                        Value = billList.Where(x => x.CreateDate.Value.Month == month.Month).Count(),
                        Date = month.ToString("MM/yy"),
                        TotalRevenue = (int)Math.Round(billList.Where(x => x.CreateDate.Value.Month == month.Month).Sum(x => x.TotalPrice).Value/1000)
                    }); ;
                }
            }

            return Json(result);
        }

        [Route("/[controller]/GetUsers")]
        public JsonResult GetUsers()
        {
            return Json(_db.AspNetUsers.ToList());

        }

        private string GetRoleByUserId(int? UserId)
        {
            if (UserId.HasValue)
            {
                var role = _db.AspNetUserRoles.Where(x=>x.UserId == UserId.Value).Select(x=>x.Role.Name).FirstOrDefault();
                return role;
            }
            return String.Empty;
        }
        public IActionResult EditAccount(int accountId)
        {
            var userInfo = _db.AspNetUsers.Where(x => x.Id == accountId).Select(x => new AccountDetails
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                CreatedDate = x.CreatedDate,
                RoleId = x.AspNetUserRoles.Where(z => z.UserId == accountId).Select(z => z.RoleId).FirstOrDefault(),
                RoleName = GetRoleByUserId(accountId),
                ProvinceId = x.UserAddress.Where(z => z.UserId == accountId).Select(z => z.ProvinceId).FirstOrDefault(),
                DistrictId = x.UserAddress.Where(z => z.UserId == accountId).Select(z => z.DistrictId).FirstOrDefault(),
                WarddId = x.UserAddress.Where(z => z.UserId == accountId).Select(z => z.WardId).FirstOrDefault(),
                PlaceDetail = x.UserAddress.Where(z => z.UserId == accountId).Select(z => z.PlaceDetails).FirstOrDefault(),
                Status = (x.IsActivated.HasValue && x.IsActivated.Value) ? "Đã kích hoạt" : "Khóa/Chưa kích hoạt"
            }).FirstOrDefault();

            ViewBag.userInfo = userInfo;
            return View();
        }



        [HttpPost]
        [Route("/[controller]/UpdateAccount")]
        public async Task<IActionResult> UpdateAccount(AccountDetails model)
        {
            var userFromDb = _db.AspNetUsers.Where(x => x.Id == model.Id).FirstOrDefault();
            if(userFromDb == null)
            {
                return Json(new { code = 0, message = "Tài khoản không tồn tại" });
            }
            Regex regex = new Regex(@"0|84[0-9]{9}");
            if (!regex.IsMatch(model.PhoneNumber))
            {
                return Json(new { code = 0, message = "Sai format số điện thoại" });
            }
            var userAddress = _db.UserAddress.Where(x => x.UserId == model.Id).FirstOrDefault();
            userFromDb.DisplayName = model.DisplayName;
            userFromDb.PhoneNumber = model.PhoneNumber;
            userAddress.DistrictId = model.DistrictId;
            userAddress.ProvinceId = model.ProvinceId;
            userAddress.WardId = model.WarddId;
            userAddress.PlaceDetails = model.PlaceDetail;

            await _db.SaveChangesAsync();
            return Json(new { code = 1 });
        }

        [Route("/[controller]/ResetPassword")]
        public async Task<IActionResult> ResetPassword(int UserId)
        {
            try
            {
                //var userFromDb = JsonConvert.SerializeObject(_db.AspNetUsers.Where(x => x.Id == UserId).AsNoTracking().FirstOrDefault());

                //var user = JsonConvert.DeserializeObject<MyUsers>(userFromDb);

                var user = await _userManager.FindByIdAsync(UserId.ToString());

                var tokenResetPwd = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (user == null)
                {
                    return Json(new { code = 0, message = "Tài khoản không tồn tại" });
                }
                var newPwd = AccountHelpers.Guild(6);
                var result = await _userManager.ResetPasswordAsync(user, tokenResetPwd, newPwd);
                if (result.Succeeded)
                {
                    return Json(new { code = 1, pwd = newPwd });
                }
                else
                {
                    return Json(new { code = 0, message = "Có lỗi xảy ra" });
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 0, message = e.ToString() });

            }
            
        }
    }
}
