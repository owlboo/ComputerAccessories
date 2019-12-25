using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            CampaignDetails = new HashSet<CampaignDetails>();
        }

        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TypeId { get; set; }
        public bool? Status { get; set; }
        public string Description { get; set; }

        public virtual CampaignType Type { get; set; }
        public virtual ICollection<CampaignDetails> CampaignDetails { get; set; }
    }
}
