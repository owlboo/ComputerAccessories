using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Districts
    {
        public Districts()
        {
            UserAddress = new HashSet<UserAddress>();
            Ward = new HashSet<Ward>();
        }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictType { get; set; }
        public int? ProvinceId { get; set; }

        public virtual Provinces Province { get; set; }
        public virtual ICollection<UserAddress> UserAddress { get; set; }
        public virtual ICollection<Ward> Ward { get; set; }
    }
}
