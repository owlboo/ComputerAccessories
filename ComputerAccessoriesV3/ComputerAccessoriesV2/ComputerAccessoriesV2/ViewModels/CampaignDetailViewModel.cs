using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class CampaignDetailViewModel
    {
        public int campaignDetailId { get; set; }
        public int productId { get; set; }
        public String productName { get; set; }
        public int originalPrice { get; set; }
        public int? promotionPrice { get; set; }
        public int campaignId { get; set; }
    }
}
