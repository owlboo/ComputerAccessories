using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class UserAddress
    {
        public int UserId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string PlaceDetails { get; set; }

        public virtual Districts District { get; set; }
        public virtual Provinces Province { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual Ward Ward { get; set; }
    }
}
