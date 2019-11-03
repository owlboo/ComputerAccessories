using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblProvince
    {
        public TblProvince()
        {
            TblDistrict = new HashSet<TblDistrict>();
            TblUserAddress = new HashSet<TblUserAddress>();
        }

        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceType { get; set; }

        public virtual ICollection<TblDistrict> TblDistrict { get; set; }
        public virtual ICollection<TblUserAddress> TblUserAddress { get; set; }
    }
}
