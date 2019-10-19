using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class Brand
    {
        public Brand()
        {
            TblProduct = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string BrandName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<Product> TblProduct { get; set; }
    }
}
