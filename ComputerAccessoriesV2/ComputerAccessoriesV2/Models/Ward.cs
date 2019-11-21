using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Ward
    {
        public Ward()
        {
            UserAddress = new HashSet<UserAddress>();
        }

        public int WardId { get; set; }
        public string WardName { get; set; }
        public string WardType { get; set; }
        public int DistrictId { get; set; }

        public virtual Districts District { get; set; }
        public virtual ICollection<UserAddress> UserAddress { get; set; }
    }
}
