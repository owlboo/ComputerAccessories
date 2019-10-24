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

        public virtual TblAttribute Attribute { get; set; }
        public virtual TblProduct Product { get; set; }
    }
}
