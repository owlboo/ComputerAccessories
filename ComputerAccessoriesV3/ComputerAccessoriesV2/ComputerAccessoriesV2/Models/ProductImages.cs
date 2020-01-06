using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class ProductImages
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string ImageUrl { get; set; }

        public virtual Products Product { get; set; }
    }
}
