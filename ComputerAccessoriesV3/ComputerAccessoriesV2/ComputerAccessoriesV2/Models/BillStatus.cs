using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class BillStatus
    {
        public BillStatus()
        {
            Bills = new HashSet<Bills>();
        }

        public int Id { get; set; }
        public string CodeName { get; set; }

        public virtual ICollection<Bills> Bills { get; set; }
    }
}
