using ComputerAccessoriesV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class ShoppingCartViewModel
    {
        public Products Products { get; set; }
        public int Quantity { get; set; }
    }
    public class ShoppingCartPreview
    {
        public List<ShoppingCartViewModel> ListProducts { get; set; }
        public Decimal TotalPrice { get; set; }
    }
}
