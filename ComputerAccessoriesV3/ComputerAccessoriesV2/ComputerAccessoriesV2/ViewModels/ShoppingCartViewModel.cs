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
        public Decimal UniPrice { get; set; }
    }
    public class ShoppingCartPreview
    {
        public List<ShoppingCartViewModel> ListProducts { get; set; }
        public Decimal TotalPrice { get; set; }
    }

    public class ShoppingBillModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string PlaceDetail { get; set; }
        public string Note { get; set; }
    }
}
