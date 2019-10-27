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
                //var user = new TblUsers
                //{
                //    UserName = model.UserName,
                //    Email = model.Email,
                //    PhoneNumber = model.PhoneNumber,
                //    Password = model.Password
                //    //DisplayName = model.FullName                   
                //};
                var result = CustomRepository.CreateUser(model.Email, model.Password,  model.FullName);
                if (result == true)
                {

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
                    if (result == SignInStatus.Success)
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