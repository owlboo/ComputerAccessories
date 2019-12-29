using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Helpers;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyUsers> _userManager;
        private readonly SignInManager<MyUsers> _signInManager;
        private readonly ILogger<LoginViewModel> _logger;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public AccountController(ComputerAccessoriesV2Context db, IHttpContextAccessor httpContextAccessor, UserManager<MyUsers> userManager, SignInManager<MyUsers> signInManager, ILogger<LoginViewModel> logger, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
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

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            var currentEmailAddress= email;
            if(email == null)
            {
                var currentLoginUser = await _userManager.GetUserAsync(User);
                if(currentLoginUser != null)
                {
                    email = currentLoginUser.Email;
                }
            }

            var userDb = _db.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
            if (code == null)
            {
                ViewBag.Email = email;
                return View();
            }
            else
            {
                if (userDb.CodeConfirm == code)
                {
                    userDb.IsActivated = true;
                    userDb.LockoutEnabled = false;
                    _db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Có lỗi xảy ra lúc xác nhận Email";
                    return View();
                }
            }
        }

        [HttpPost]
        public JsonResult ConfirmEmail(ConfirmEmailViewModel _params)
        {
            var user = _db.AspNetUsers.Where(x => x.Email == _params.Email).FirstOrDefault();
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Không tìm thấy tài khoản với địa chỉ Email đó" });
            }
            else if (user.CodeConfirm == _params.ConfirmCode)
            {
                user.IsActivated = true;
                user.LockoutEnabled = false;
                _db.SaveChanges();

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { notify = "Xác nhận Email thành công" });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Sai Mã xác nhận, Vui lòng thử lại!" });
            }
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ForgetPassword(String email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var sercureCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                var host = Request.Host.ToUriComponent();
                var protocal = Request.Scheme;
                
                var result = EmailHelpers.SendForgetPasswordEmail(user, sercureCode, host, protocal);

                if (result)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Một Email reset lại mật khẩu đã được gửi đến địa chỉ Email của bạn" });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Lỗi trong quá trình gửi Email" });
                }

            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Không tìm thấy Email!!" });
            }
        }

        public async Task<IActionResult> ResetPassword(string email, string sercureCode)
        {
            ViewBag.sercureCode = HttpUtility.UrlEncode(sercureCode);
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ResetNewPassword(string Email, string SercureCode, string NewPassword, string ConfirmNewPassword)
        {
            if (NewPassword.Equals(ConfirmNewPassword))
            {   
                var user = await _userManager.FindByEmailAsync(Email);

                var decode = HttpUtility.UrlDecode(SercureCode);
                    
                var resultResetPassword = await _userManager.ResetPasswordAsync(user, decode, NewPassword);
                if (resultResetPassword.Succeeded)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Thay đổi mật khẩu thành công!" });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Thay đổi mật khẩu thất bại, có thể bạn đã để thời gian chờ quá lâu!" });
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Mật khẩu mới và xác nhận mật khẩu mới phải giống nhau!" });
            }
        }

        [HttpPost]
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
                    return Json(new { code = 0, err = "Email của bạn đã tồn tại. Nhấn vào <a href='/Customer/Account/ForgetPassword' >Quên mật khẩu</a> để lấy lại mật khẩu" });
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
                    if (!await _roleManager.RoleExistsAsync(model.RoleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>
                        {
                            Name = model.RoleName
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
                    str.Append("</head>");
                    str.Append("<body>");
                    str.Append("<div class='text-center'> ");
                    str.Append(" <h3 class='text-left'>Cảm ơn bạn đã đăng ký tài khoản</h3>");
                    str.Append("<p class='text-left'> Mã kích hoạt tài khoản của bạn là: <strong style='font-size:20px'>" + userFromDb.CodeConfirm + "</strong></p>");
                    str.Append("</div>");
                    str.Append("</body>");
                    str.Append("</html>");
                    EmailHelpers.SendConfirmEmail(userFromDb, "Thông báo", "Mã kích hoạt", str.ToString());
                    #endregion
                    return Json(new { code = 1, returnUrl = "/Customer/Account/ConfirmEmail?email=" + userFromDb.Email, email = userFromDb.Email });
                }
                else
                {
                    return Json(new { code = 0, err = "Có lỗi xảy ra trong quá trình khởi tạo, vui lòng thử lại" });
                }

            }
            return Json(new { code = 0, err = "Có lỗi xảy ra trong quá trình khởi tạo, vui lòng thử lại" });
        }
        public IActionResult SignIn(LoginViewModel model = null, string err = null, string returnUrl = null)
        {
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
                if (userFromDb == null)
                {
                    return RedirectToAction("SignIn", "Account", new { err = "Không tìm thấy tài khoản!" });
                }

                var result = await _signInManager.PasswordSignInAsync(userFromDb.Email, model.Password, model.isRemember, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    #region CookieLogin
                    string cookie = Request.Cookies[$"CookieLogin{MySecurity.Base64Encode(userFromDb.Id.ToString())}"];
                    if (cookie != null)
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
        [Route("/[controller]/AddToCart")]
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {

            //if (!_signInManager.IsSignedIn(User))
            //{
            //    return RedirectToAction("Account", "SignIn");
            //}
            var user = User.FindFirst(ClaimTypes.NameIdentifier);

            var productFromDb = _db.Products.Where(x => x.Id == productId).FirstOrDefault();
            var ListShoppingCart = new List<ShoppingCartViewModel>();
            int sum = 0;

            string cookie1 = Request.Cookies[$"CookieLogin"];
            if (cookie1 == null)
            {
                Response.Cookies.Append($"CookieLogin", "1", new CookieOptions { Expires = DateTime.Now.AddMinutes(60) });
            }
            try
            {
                if (user == null)
                {
                    //write cookie for customer who do not log on to system
                    string cookieKey = "shopping";
                    var cookie = Request.Cookies[$"Cookie_{MySecurity.Base64Decode(cookieKey)}"];
                    if (cookie == null)
                    {
                        var obj = new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                            Quantity = quantity
                        };
                        ListShoppingCart.Add(obj);
                        sum = ListShoppingCart.Count;
                        CookieOptions options = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(30),
                            IsEssential = true,
                            HttpOnly = true

                        };
                        Response.Cookies.Append($"Cookie_{MySecurity.Base64Decode(cookieKey)}", JsonConvert.SerializeObject(ListShoppingCart), options);
                        //_response.Append(cookieKey, JsonConvert.SerializeObject(ListShoppingCart), options);
                    }
                    else
                    {
                        ListShoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                        var obj = new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                            Quantity = quantity
                        };
                        ListShoppingCart.Add(obj);
                        Response.Cookies.Append(cookieKey, JsonConvert.SerializeObject(ListShoppingCart), new CookieOptions { Expires = DateTime.Now.AddMinutes(30), IsEssential = true });
                    }

                    //return Json(new { code = 0, returnUrl = "/Customer/Account/SignIn" });
                }
                else
                {
                    #region write session
                    var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    string sskey = "SessionSP_" + currentUser;
                    if (_session.GetString(sskey) == null)
                    {
                        var obj = new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                            Quantity = quantity
                        };
                        ListShoppingCart.Add(obj);
                        sum = ListShoppingCart.Count;
                        _session.SetString(sskey, JsonConvert.SerializeObject(ListShoppingCart));
                    }
                    else
                    {
                        ListShoppingCart = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(sskey));
                        if (!ListShoppingCart.Any(x => x.Products.Id == productId))
                        {
                            ListShoppingCart.Add(new ShoppingCartViewModel
                            {
                                Products = _db.Products.Where(x => x.Id == productId).FirstOrDefault(),
                                Quantity = quantity
                            });
                        }
                        else
                        {
                            foreach (var item in ListShoppingCart)
                            {
                                if (item.Products.Id == productId)
                                {
                                    item.Quantity += quantity;
                                }
                            }

                        }
                        sum = ListShoppingCart.Count;

                        _session.SetString(sskey, JsonConvert.SerializeObject(ListShoppingCart));
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 0, returnUrl = "/Customer/Account/SignIn" });
                throw;
            }

            return Json(new { code = 1, Name = productFromDb.ProductName, quantity = quantity, sum = sum });

        }
    }
}
#endregion