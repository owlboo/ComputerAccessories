using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComputerAccessories.Custom;
using ComputerAccessories.Helpers;
using ComputerAccessories.Models;
using ComputerAccessories.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SignInHelper = ComputerAccessories.Custom.SignInHelper;

namespace ComputerAccessories.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        public AccountController(ComputerAccessoriesContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            return View();
        }
        //[Route("/[controller]/SignUp")]
        public IActionResult SignUp()
        {

            var provinces = _db.TblProvince.ToList();
            ViewBag.lstProvince = provinces;
            return View();

        }

        public IActionResult ConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmInfo)
        {
            var user = _db.TblUsers.Where(x => x.Email == confirmInfo.Email).FirstOrDefault();
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
        [ValidateAntiForgeryToken]
        //[Route("/[controller]/SignUpPost")]

        public async Task<IActionResult> SignUp(RegisterViewModel model)
        {
            ViewBag.Error = "";
            if (ModelState.IsValid)
            {
                Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
                Match match = regex.Match(model.Email.Trim().ToLower());
                if (!match.Success)
                {
                    return new JsonResult(new { code = 0, Err = "" });
                }

                var result = CustomRepository.CreateUser(model.Email, model.PhoneNumber, model.Password,  model.FullName, model.ProvinceId, model.DistricId, model.WardId, model.PlaceDetail);
                if (result != null)
                {
                    EmailHelper.SendConfirmEmail(result);
                    var user = _db.TblUsers.Where(x => x.Email.Equals(model.Email)).FirstOrDefault();
                    var re = await CustomRepository.AddUserToRoleAsync(user, 3);
                    if (re == true)
                    {
                        return RedirectToAction("SignIn","Account",new { Email=user.Email});
                    }
                }
                else
                {
                    ViewBag.Error = "Người dùng đã tồn tại";
                    return RedirectToAction(nameof(SignUp));
                }

            }
            return new JsonResult(new { code = 0, Err = "*Có lỗi xảy ra, vui lòng thử lại" });
        }
        public IActionResult SignIn(LoginViewModel model,string err=null)
        {
            ViewBag.error = err;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInHelper signInHelper = new SignInHelper();
                var user = _db.TblUsers.Where(x => x.Email == model.Email).FirstOrDefault();
                if(user != null)
                {
                    var result = signInHelper.PasswordSignInAsync( user, model.Password, true, shouldLockout: false);
                    if (result == Custom.SignInStatus.Success)
                    {
                        return RedirectToAction("Index", "Home", new {userId = user.Id});
                    }
                    else
                    {
                        return RedirectToAction("SignIn", "Account", new { err = "Đăng nhập không thành công!" });
                    }
                }
                else
                {
                    return RedirectToAction("SignIn", "Account", new { err = "Người dùng không tồn tại" });
                }
                
            }
            return RedirectToAction("SignIn", "Account", new { err = "Có lỗi xảy ra, vui lòng thử lại" });
        }
     
           
    }
}