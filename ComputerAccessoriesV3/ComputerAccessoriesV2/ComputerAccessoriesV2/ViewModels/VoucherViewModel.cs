using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class VoucherViewModel
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? DateActive { get; set; }
        public bool? IsActive { get; set; }
        public int? Value { get; set; }

        public int? Max { get; set; }
    }
}
