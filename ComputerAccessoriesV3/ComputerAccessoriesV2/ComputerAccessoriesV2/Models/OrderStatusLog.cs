using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class OrderStatusLog
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int NewStatus { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUserId { get; set; }
        public string Note { get; set; }

        public virtual Bills Bill { get; set; }
        public virtual BillStatus NewStatusNavigation { get; set; }
    }
}
