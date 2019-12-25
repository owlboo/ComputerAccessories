using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Vouchers
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? DateActive { get; set; }
        public bool? IsActive { get; set; }
        public int? Value { get; set; }
        public int Used { get; set; }
        public int? Max { get; set; }
    }
}
