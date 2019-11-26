using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class AccountGridModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public string DisplayName { get; set; }
        public bool IsActivated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Address { get; set; }
    }
}
