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
        }

        public int BillId { get; set; }
        public string BillName { get; set; }
        public int? CustomerId { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? SaleId { get; set; }
        public decimal? LastPrice { get; set; }
        public string Note { get; set; }
        public bool? IncludedVoucher { get; set; }
        public int? GuestAnonyId { get; set; }
        public string Voucher { get; set; }
        public int? ShipperId { get; set; }
        public int? Status { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public virtual AspNetUsers Customer { get; set; }
        public virtual NoStroredGuest GuestAnony { get; set; }
        public virtual BillStatus StatusNavigation { get; set; }
        public virtual ICollection<BillDetails> BillDetails { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistory { get; set; }
    }
}
