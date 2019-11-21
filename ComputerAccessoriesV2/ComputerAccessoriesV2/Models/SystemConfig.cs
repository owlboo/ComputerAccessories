using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class SystemConfig
    {
        public int Id { get; set; }
        public string ShopName { get; set; }
        public string SystemEmail { get; set; }
        public string SystemPhone { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ShopAddress { get; set; }
        public string Skype { get; set; }
        public string Facebook { get; set; }
        public string Zalo { get; set; }
    }
}
