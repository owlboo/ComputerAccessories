using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class Attribute
    {
        public Attribute()
        {
            TblProductAttributes = new HashSet<TblProductAttributes>();
        }

        public int Id { get; set; }
        public string AttributeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<TblProductAttributes> TblProductAttributes { get; set; }
    }
}
