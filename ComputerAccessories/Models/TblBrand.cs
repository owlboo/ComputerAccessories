using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblBrand
    {
        public TblBrand()
        {
            TblProduct = new HashSet<TblProduct>();
        }

        public int Id { get; set; }
        public string BrandName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<TblProduct> TblProduct { get; set; }
    }
}
