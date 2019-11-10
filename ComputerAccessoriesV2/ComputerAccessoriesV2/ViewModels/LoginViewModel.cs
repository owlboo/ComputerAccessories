using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Password { get; set; }
        public bool isRemember { get; set; }
    }
}
