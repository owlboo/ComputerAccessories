using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProfileInfo()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Address()
        {
            int currentUserId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value,out currentUserId);
            var user = _db.AspNetUsers.Where(x => x.Id == currentUserId);
            if(user == null)
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
        [Route("Customer/ChangeAddress")]
        public async Task<IActionResult> ChangeAddress(UserAddress _userAddress)
        {
            int currentUserId = 0;
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);

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
        public IActionResult Orders()
        {
            return View();
        }

    }
}