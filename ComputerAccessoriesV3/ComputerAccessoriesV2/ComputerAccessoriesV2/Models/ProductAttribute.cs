using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class ProductAttribute
    {
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }

        public virtual Attributes Attribute { get; set; }
        public virtual Products Product { get; set; }
    }
}
