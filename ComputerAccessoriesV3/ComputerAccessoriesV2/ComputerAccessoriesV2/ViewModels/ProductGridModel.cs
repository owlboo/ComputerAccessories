using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class ProductGridModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string ShorDescription { get; set; }
        public string FullDescription { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Thumnail { get; set; }

        public int Status { get; set; }
    }
}
