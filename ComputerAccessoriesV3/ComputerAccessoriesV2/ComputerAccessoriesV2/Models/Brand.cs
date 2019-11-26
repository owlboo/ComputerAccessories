using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public string BrandName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Logo { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
