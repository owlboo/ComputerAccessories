using ComputerAccessories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComputerAccessories.Custom
{
    public class UserManagerCustom
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ComputerAccessoriesContext _db;
        public UserManager<TblUsers> _userManager;
        public UserManagerCustom(IHttpContextAccessor httpContextAccessor, UserManager<TblUsers> userManager, ComputerAccessoriesContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
        }

        public int GetUserId()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            //var userID = await _userManager.GetUserId(_httpContextAccessor.HttpContext.User.);
            var userId = _db.TblUsers.Where(x => x.Email.Equals(userName)).Select(x=>x.Id).FirstOrDefault();
            return userId;
        }
        public TblUsers FindById(int id)
        {
            var userId = _db.TblUsers.Where(x => x.Id == id).FirstOrDefault();
            return userId;
        }

        public TblUsers FindByEmail(string email)
        {
            var user =  _db.TblUsers.Where(x => x.Email == email).SingleOrDefault();
            return user;
        }


    }
}
