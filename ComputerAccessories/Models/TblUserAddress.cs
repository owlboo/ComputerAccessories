using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblUserAddress
    {
        public int UserId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string PlaceDetail { get; set; }

        public virtual TblDistrict District { get; set; }
        public virtual TblProvince Province { get; set; }
        public virtual TblUsers User { get; set; }
        public virtual TblWard Ward { get; set; }
    }
}
