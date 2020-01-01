using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class UserInformationModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string ProvinceName { get; set; }
        public int ProvinceId { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public string WardName { get; set; }
        public int WardId { get; set; }
        public string PhoneNumber { get; set; }
        public string PlaceDetails { get; set; }
        //public string FullAddress { get; set; }
        public string Email { get; set; }
    }
}
