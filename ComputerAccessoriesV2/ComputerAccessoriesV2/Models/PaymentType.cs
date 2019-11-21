using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            TransactionHistory = new HashSet<TransactionHistory>();
        }

        public int PaymentId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TransactionHistory> TransactionHistory { get; set; }
    }
}
