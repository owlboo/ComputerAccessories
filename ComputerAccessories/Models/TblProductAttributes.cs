using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblProductAttributes
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? AttributeId { get; set; }
        public string Value { get; set; }

        public virtual Attribute Attribute { get; set; }
        public virtual Product Product { get; set; }
    }
}
