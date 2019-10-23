using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblCategory
    {
        public TblCategory()
        {
            TblAttribute = new HashSet<TblAttribute>();
            TblProduct = new HashSet<TblProduct>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<TblAttribute> TblAttribute { get; set; }
        public virtual ICollection<TblProduct> TblProduct { get; set; }
    }
}
