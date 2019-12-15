using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CampaignController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly UserManager<MyUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public CampaignController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Policy = Policy.AdminModify)]
        [HttpGet]
        public JsonResult GetAllCampaign()
        {
            var listCampaign = _db.Campaign.ToList();
            return Json(listCampaign);
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult CreateNewCampaign()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        public JsonResult CreateNewCampain(CampaignViewModel _params)
        {
            
            return Json(null);
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult EditCampaign()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        public JsonResult EditCampaign(CampaignViewModel _params)
        {
            return Json(null);
        }

        //[Authorize(Policy = Policy.AdminAccess)]
        public JsonResult GetAllCampaignType()
        {
            return Json(_db.CampaignType.ToList());
        }

        public JsonResult GetAllProductForNewCompaign()
        {
            return(_db.Products.ToList());
        }
    }
}