using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Products
    {
        public Products()
        {
            BillDetails = new HashSet<BillDetails>();
            CampaignDetails = new HashSet<CampaignDetails>();
            ProductAttribute = new HashSet<ProductAttribute>();
            ProductImages = new HashSet<ProductImages>();
            Reviews = new HashSet<Reviews>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string ShorDescription { get; set; }
        public string FullDescription { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string Thumnail { get; set; }
        public int Status { get; set; }
        public bool? IsNew { get; set; }
        public int ViewCounts { get; set; }
        public string Thumnail2 { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<BillDetails> BillDetails { get; set; }
        public virtual ICollection<CampaignDetails> CampaignDetails { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttribute { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
