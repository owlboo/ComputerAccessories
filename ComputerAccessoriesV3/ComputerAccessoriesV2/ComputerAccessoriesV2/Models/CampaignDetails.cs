using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class CampaignDetails
    {
        public int CampaignId { get; set; }
        public int ProductId { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int CampaignDetailId { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Products Product { get; set; }
    }
}
