using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class Category
    {
        public Category()
        {
            TblAttribute = new HashSet<Attribute>();
            TblProduct = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Attribute> TblAttribute { get; set; }
        public virtual ICollection<Product> TblProduct { get; set; }
    }
}
