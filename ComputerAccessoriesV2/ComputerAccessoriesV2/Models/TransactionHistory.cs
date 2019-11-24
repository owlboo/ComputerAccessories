using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class TransactionHistory
    {
        public int TransactionId { get; set; }
        public decimal? PaymentAmout { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UserId { get; set; }
        public int? BillId { get; set; }
        public int? PaymentType { get; set; }

        public virtual Bills Bill { get; set; }
        public virtual PaymentType PaymentTypeNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
