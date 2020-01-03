using ComputerAccessoriesV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
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
    public class AccountDetails
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WarddId { get; set; }
        public string PlaceDetail { get; set; }
        public DateTime? CreatedDate { get; set; }
        
    }
}
