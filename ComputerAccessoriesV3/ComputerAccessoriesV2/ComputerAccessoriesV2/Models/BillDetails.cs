using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class BillDetails
    {
        public int ProductId { get; set; }
        public int BillId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

        public virtual Bills Bill { get; set; }
        public virtual Products Product { get; set; }
    }
}
