using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int ProvinceId { get; set; }
        public int DistricId { get; set; }
        public int WardId { get; set; }

        public string PlaceDetail { get; set; }
        //public string ConfirmPwd { get; set; }
    }
}
