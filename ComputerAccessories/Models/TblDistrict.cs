using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblDistrict
    {
        public TblDistrict()
        {
            TblUserAddress = new HashSet<TblUserAddress>();
            TblWard = new HashSet<TblWard>();
        }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictType { get; set; }
        public int ProvinceId { get; set; }

        public virtual TblProvince Province { get; set; }
        public virtual ICollection<TblUserAddress> TblUserAddress { get; set; }
        public virtual ICollection<TblWard> TblWard { get; set; }
    }
}
