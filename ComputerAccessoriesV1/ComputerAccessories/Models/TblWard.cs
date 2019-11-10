using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblWard
    {
        public TblWard()
        {
            TblUserAddress = new HashSet<TblUserAddress>();
        }

        public int WardId { get; set; }
        public string WardName { get; set; }
        public string WardType { get; set; }
        public int DistrictId { get; set; }

        public virtual TblDistrict District { get; set; }
        public virtual ICollection<TblUserAddress> TblUserAddress { get; set; }
    }
}
