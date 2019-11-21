using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Bills
    {
        public Bills()
        {
            BillDetails = new HashSet<BillDetails>();
            TransactionHistory = new HashSet<TransactionHistory>();
            Vouchers = new HashSet<Vouchers>();
        }

        public int BillId { get; set; }
        public string BillName { get; set; }
        public int? CustomerId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string CreateDate { get; set; }
        public int? SaleId { get; set; }
        public int? VoucherId { get; set; }
        public decimal? LastPrice { get; set; }

        public virtual AspNetUsers Customer { get; set; }
        public virtual ICollection<BillDetails> BillDetails { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistory { get; set; }
        public virtual ICollection<Vouchers> Vouchers { get; set; }
    }
}
