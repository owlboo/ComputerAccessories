using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessories.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string IsActivated { get; set; }
        public int Role { get; set; }
        public string RoleName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
