using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyUsers> _userManager;
        private readonly SignInManager<MyUsers> _signInManager;

        public ProfileController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, SignInManager<MyUsers> signInManager)
        {
            _signInManager = signInManager;
            _db = db;
            _userManager = userManager;
        }

        [Authorize(Policy = Policy.ProfileModify)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = Policy.ProfileModify)]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = Policy.ProfileModify)]
        public async Task<IActionResult> ProfileInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ProfileModify)]
        [Route("/Profile/ChangeProfileInfo")]
        public async Task<IActionResult> ChangeProfileInfo(ProfileUpdateInfo _params)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserDb = _db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();

            currentUserDb.DisplayName = _params.UserName;
            currentUserDb.PhoneNumber = _params.PhoneNumber;

            var result = await _db.SaveChangesAsync();

            if(result > 0)
            {
                return Json(new { StatusCode = (int)HttpStatusCode.OK });
            }
            else
            {
                return Json(new { StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }

        [HttpGet]
        [Authorize(Policy = Policy.ProfileModify)]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("/Profile/ChangePassword")]
        [Authorize(Policy = Policy.ProfileModify)]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel _params)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(currentUser, _params.OldPassword, _params.NewPassword);

            if(result.Succeeded)
            {
                return Json(new { StatusCode = (int)HttpStatusCode.OK });
            }
            else
            {
                return Json(new { StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }


        [HttpGet]
        [Authorize(Policy = Policy.ProfileModify)]
        public async Task<IActionResult> Address()
        {
            var user = await _userManager.GetUserAsync(User);
            int currentUserId = 0;
            currentUserId = user.Id;

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                UserAddress userAddress = _db.UserAddress.Where(x => x.UserId == currentUserId).First();
                ViewBag.userAddress = userAddress;
                return View();
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.ProfileModify)]
        [Route("Customer/ChangeAddress")]
        public async Task<IActionResult> ChangeAddress(UserAddress _userAddress)
        {
            var user = await _userManager.GetUserAsync(User);
            int currentUserId = 0;
            currentUserId = user.Id;

            if (currentUserId != 0)
            {
                var userAddress = _db.UserAddress.Where(x => x.UserId == currentUserId).FirstOrDefault();
                userAddress.ProvinceId = _userAddress.ProvinceId;
                userAddress.DistrictId = _userAddress.DistrictId;
                userAddress.WardId = _userAddress.WardId;
                userAddress.PlaceDetails = _userAddress.PlaceDetails;

                var result = await _db.SaveChangesAsync();
                if(result > 0)
                {
                    return RedirectToAction("Index");
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

        [HttpGet]
        [Authorize(Policy = Policy.ProfileModify)]
        public IActionResult Orders()
        {
            return View();
        }

    }
}