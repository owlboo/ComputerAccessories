using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblProductImages
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string ImageUrl { get; set; }

        public virtual TblProduct Product { get; set; }
    }
}
