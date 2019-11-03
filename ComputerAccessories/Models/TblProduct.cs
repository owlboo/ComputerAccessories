using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblProduct
    {
        public TblProduct()
        {
            TblProductAttributes = new HashSet<TblProductAttributes>();
            TblProductImages = new HashSet<TblProductImages>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public decimal? Price { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public decimal? PromotionPrice { get; set; }
        public string Thumnail { get; set; }
        public bool IsAvailable { get; set; }

        public virtual TblBrand Brand { get; set; }
        public virtual TblCategory Category { get; set; }
        public virtual ICollection<TblProductAttributes> TblProductAttributes { get; set; }
        public virtual ICollection<TblProductImages> TblProductImages { get; set; }
    }
}
