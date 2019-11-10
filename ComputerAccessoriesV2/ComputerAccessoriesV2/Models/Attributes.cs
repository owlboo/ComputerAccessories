using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Attributes
    {
        public Attributes()
        {
            ProductAttribute = new HashSet<ProductAttribute>();
        }

        public int Id { get; set; }
        public string AttributeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttribute { get; set; }
    }
}
