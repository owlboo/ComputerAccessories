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

        public UserManagerCustom(IHttpContextAccessor httpContextAccessor, ComputerAccessoriesContext db)
        {
            _httpContextAccessor = httpContextAccessor;
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
        public string GetUserRole(TblUsers user)
        {
            return (string)(from u in _db.TblUsers
                            join
                            us_role in _db.TblUserRole
                            on u.Id equals us_role.UserId
                            join role in _db.TblRoles
                            on us_role.RoleId equals role.Id
                            where u.Id == user.Id
                            select role.Name).FirstOrDefault();
        }

    }
}
