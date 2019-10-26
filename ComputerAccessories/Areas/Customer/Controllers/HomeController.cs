using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessories.Custom;
using ComputerAccessories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessories.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(ComputerAccessoriesContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(int? userId)
        {
            UserManagerCustom _userManager = new UserManagerCustom(_httpContextAccessor,_db);
            if (userId == null)
            {
                ViewBag.User = null;
            }
            else
            {
                ViewBag.IsAdmin = 0;
                var userFromDb = _db.TblUsers.Where(x => x.Id == userId).FirstOrDefault();
                ViewBag.User = userFromDb;
                if (_userManager.GetUserRole(userFromDb) == SD.ConstantsHolder.SupperAdmin || _userManager.GetUserRole(userFromDb) == SD.ConstantsHolder.Sale)
                {
                    ViewBag.IsAdmin = 1;                   
                }
            }
           
            return View();
        }
    }
}