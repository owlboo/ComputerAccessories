using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessories.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessories.Areas.Customer.Controllers
{
    public class RegionController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        public RegionController(ComputerAccessoriesContext db)
        {
            _db = db;
        }

        [Route("/[controller]/GetDistrict")]
        [HttpGet]
        public JsonResult GetDistrict(int provinceId)
        {
            return Json(_db.TblDistrict.Where(x => x.ProvinceId == provinceId).ToList());
        }

        [Route("/[controller]/GetWard")]
        [HttpGet]
        public JsonResult GetWard(int districtId)
        {
            return Json(_db.TblWard.Where(x => x.DistrictId == districtId).ToList());
        }
    }
}