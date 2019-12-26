using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    public class RegionController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        public RegionController(ComputerAccessoriesV2Context db)
        {
            _db = db;
        }

        [Route("/[controller]/GetProvinces")]
        [HttpGet]
        public JsonResult GetProvinces()
        {
            return Json(_db.Provinces.ToList());
        }

        [Route("/[controller]/GetDistricts")]
        [HttpGet]
        public JsonResult GetDistricts(int provinceId)
        {
            return Json(_db.Districts.Where(x => x.ProvinceId == provinceId).ToList());
        }

        [Route("/[controller]/GetWards")]
        [HttpGet]
        public JsonResult GetWards(int districtId)
        {
            return Json(_db.Ward.Where(x => x.DistrictId == districtId).ToList());
        }
    }
}