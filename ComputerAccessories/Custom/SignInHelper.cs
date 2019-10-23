using ComputerAccessories.Helpers;
using ComputerAccessories.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComputerAccessories.Custom
{
    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure,
        NotActivated
    }

    public class SignInHelper
    {
        //public UserManager<TblUsers> _userManager { get; set; }
        public UserManagerCustom _userManagerCustom { get; set; }
        public SignInStatus PasswordSignInAsync(TblUsers user, string password, bool isPersistent, bool shouldLockout )
        {
            //var user = _userManagerCustom.FindByEmail(email);
            if(user == null)
            {
                return SignInStatus.Failure;
            }
            if (user.LockoutEnabled == true)
            {
                return SignInStatus.LockedOut;
            }
            if (!user.IsActivated.Value)
            {
                return SignInStatus.NotActivated;
            }
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                if (CustomRepository.VerifyHashedPassword(user, password))
                {
                    return SignInStatus.Success;
                }
            }
            else
            {
                if(CustomRepository.ValidateUser(user.Email, user.Password))
                {
                    if (CustomRepository.VerifyHashedPassword(user, password))
                        return SignInStatus.Success;
                    return SignInStatus.Failure;
                }
                else
                {
                    return SignInStatus.Failure;
                }
                
            }
            return SignInStatus.Failure;
        }
    }
}
