using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Data
{
    public class MyUsers:IdentityUser<int>
    {
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public bool? IsActivated { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CodeConfirm { get; set; }
    }
}
