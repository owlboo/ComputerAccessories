using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Provinces
    {
        public Provinces()
        {
            Districts = new HashSet<Districts>();
            UserAddress = new HashSet<UserAddress>();
        }

        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceType { get; set; }

        public virtual ICollection<Districts> Districts { get; set; }
        public virtual ICollection<UserAddress> UserAddress { get; set; }
    }
}
