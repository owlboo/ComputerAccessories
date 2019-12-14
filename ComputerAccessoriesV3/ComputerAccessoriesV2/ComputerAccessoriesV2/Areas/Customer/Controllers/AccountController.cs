using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Helpers;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyUsers> _userManager;
        private readonly SignInManager<MyUsers> _signInManager;
        private readonly ILogger<LoginViewModel> _logger;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public AccountController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, SignInManager<MyUsers> signInManager, ILogger<LoginViewModel> logger, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        //[Route("/[controller]/SignUp")]
        public IActionResult SignUp()
        {

            var provinces = _db.Provinces.ToList();
            ViewBag.lstProvince = provinces;
            return View();

        }

        public IActionResult AccessDeny()
        {
            return View();
        }

        public IActionResult ConfirmEmail(string Email=null)
        {
            ViewBag.email = Email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmInfo)
        {
            var user = _db.AspNetUsers.Where(x => x.Email == confirmInfo.Email).FirstOrDefault();
            if (user == null)
            {
                ViewBag.ErrorMes = "Không tìm thấy Email!";
                return View();
            }
            else if (user.CodeConfirm == confirmInfo.ConfirmCode)
            {
                user.IsActivated = true;
                user.LockoutEnabled = false;
                _db.SaveChanges();
                return RedirectToAction("Index", "HomeController", new { userId = user.Id });
            }
            else
            {
                ViewBag.ErrorMes = "Không đúng mã code xác nhận!";
                return View();
            }
        }

        [HttpPost]
        [Route("/[controller]/SignUp")]
        
        public async Task<IActionResult> SignUp(RegisterViewModel model)
        {
            ViewBag.Error = "";
            if (ModelState.IsValid)
            {

                var host = Request.Host;
                var action = Request.Path.Value;
                var protocol = Request.Protocol;
                //var area = ControllerContext.RouteData.DataTokens["area"].ToString();
                var area = "/Customer";
                //Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
                //Match match = regex.Match(model.Email.Trim().ToLower());
                //if (!match.Success)
                //{
                //    return new JsonResult(new { code = 0, Err = "" });
                //}
                var checkUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
                if (checkUser != null)
                {
                    return Json(new { code = 0 , err="Email của bạn đã tồn tại. Nhấn vào <a href='/Account/ForgetPassword' >Quên mật khẩu</a> để lấy lại mật khẩu"});
                }

                var user = new MyUsers
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.FullName,
                    LockoutEnabled = false,
                    IsActivated = false,
                    CreatedDate = DateTime.Now,
                    PhoneNumber = model.PhoneNumber,
                    CodeConfirm = AccountHelpers.GenerateCodeConfirm()

                };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(true);

                //var result = CustomRepository.CreateUser(model.Email, model.PhoneNumber, model.Password,  model.FullName, model.ProvinceId, model.DistricId, model.WardId, model.PlaceDetail);
                if (result.Succeeded)
                {
                    //var userFromDb = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
                    if(!await _roleManager.RoleExistsAsync(model.RoleName))
                    {
                       await _roleManager.CreateAsync(new IdentityRole<int> { 
                           Name=model.RoleName
                       });
                    }
                    await _userManager.AddToRoleAsync(user, model.RoleName);

                    
                    var userAddress = new UserAddress
                    {
                        UserId = user.Id,
                        WardId = model.WardId,
                        ProvinceId = model.ProvinceId,
                        DistrictId = model.DistrictId,
                        PlaceDetails = model.PlaceDetail
                    };
                    _db.UserAddress.Add(userAddress);
                    //update dia chi
                    var userFromDb = _db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();
                    userFromDb.Address = model.PlaceDetail + " " + _db.Ward.Find(model.WardId).WardName + "," + _db.Districts.Find(model.DistrictId).DistrictName + "," + _db.Provinces.Find(model.ProvinceId).ProvinceName;
                    

                    await _db.SaveChangesAsync();
                    #region Send Mail Confirm
                    StringBuilder str = new StringBuilder();
                    str.Append("<!DOCTYPE html>");
                    str.Append("<head>");
                    str.Append("<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css'>");
                    str.Append("</head>");
                    str.Append("<body>");
                    str.Append("<div class='text-center'> ");
                    str.Append(" <h3 class='text-left'>Cảm ơn bạn đã đăng ký tài khoản</h3>");
                    str.Append("<p class='text-left'> Mã kích hoạt tài khoản của bạn là: <strong style='font-size:20px'>" + userFromDb.CodeConfirm + "</strong></p>");
                    str.Append("<a class='btn btn-primary' href='"+protocol + host+area + action + "'>Click vào đây để kích hoạt</a>");
                    str.Append("</div>");
                    str.Append("<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js'></script>");
                    str.Append("<script src='https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js'></script>");
                    str.Append("<script src='https://maxcdn.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js'></script>");
                    str.Append("</body>");
                    str.Append("</html>");
                    EmailHelpers.SendConfirmEmail(userFromDb,"Thông báo","Mã kích hoạt", str.ToString());
                    #endregion
                    return Json(new { code = 1, returnUrl = "/Customer/Account/ConfirmEmail?email="+userFromDb.Email, email=userFromDb.Email });
                }
                else
                {
                    return Json(new { code = 0, err = "Có lỗi xảy ra trong quá trình khởi tạo, vui lòng thử lại" });
                }

            }
            return Json(new { code = 0, err = "Có lỗi xảy ra trong quá trình khởi tạo, vui lòng thử lại" });
        }
        public IActionResult SignIn(LoginViewModel model=null, string err = null, string returnUrl=null)
        {
            //string cookie = Request.Cookies[$"CookieSignIn{MySecurity.Base64Encode(user.Id.ToString())}"];
            //if (cookie != null)
            //{
            //    CookieOptions cookieOptions = new CookieOptions();
            //    cookieOptions.Expires = DateTime.Now.AddMinutes(-60);
            //    Response.Cookies.Append($"CookieSignIn{MySecurity.Base64Encode(user.Id.ToString())}", MySecurity.EncryptPassword(user.Email), cookieOptions);
            //    return RedirectToAction("Index", "Home", new { userId = user.Id });
            //}
            //HttpContextAccessor httpContextAccessor = new HttpContextAccessor();


            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUser != null)
            {
                var userFromDb = _db.AspNetUsers.Where(x => x.Id == Int32.Parse(currentUser.Value)).FirstOrDefault();
                #region ghi cookie
                var cookie = Request.Cookies[$"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}"];
                if (cookie == null)
                {
                    Response.Cookies.Append($"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}", MySecurity.DecryptPassword(userFromDb.Email), new CookieOptions { Expires = DateTime.Now.AddMinutes(60) });
                }
                else
                {
                    Response.Cookies.Append($"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}", MySecurity.DecryptPassword(userFromDb.Email), new CookieOptions { Expires = DateTime.Now.AddMinutes(60) });
                }
            }

            if (!String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Customer/Home/Index";
            }
            ViewBag.error = err;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var userFromDb = _db.AspNetUsers.Where(x => x.Email == model.Email).SingleOrDefault();
                if(userFromDb == null)
                {
                    return RedirectToAction("SignIn", "Account", new { err = "Không tìm thấy tài khoản!" });
                }

                var result = await _signInManager.PasswordSignInAsync(userFromDb.Email, model.Password, model.isRemember, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    #region CookieLogin
                    string cookie = Request.Cookies[$"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}"];
                    if(cookie != null)
                    {
                        Response.Cookies.Append($"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}", MySecurity.DecryptPassword(userFromDb.Email), new CookieOptions { Expires = DateTime.Now.AddMinutes(60) });
                    }
                    Response.Cookies.Append($"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}", MySecurity.DecryptPassword(userFromDb.Email), new CookieOptions { Expires = DateTime.Now.AddMinutes(60) });
                    #endregion
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("SignIn", "Account", new { err = "Có lỗi xảy ra, vui lòng thử lại" });
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Account");
        }    
    }
}
#endregion