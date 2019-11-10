using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Category
    {
        public Category()
        {
            Attributes = new HashSet<Attributes>();
            InverseParend = new HashSet<Category>();
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ParendId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? Status { get; set; }

        public virtual Category Parend { get; set; }
        public virtual ICollection<Attributes> Attributes { get; set; }
        public virtual ICollection<Category> InverseParend { get; set; }
        public virtual ICollection<Products> Products { get; set; }
    }
}
