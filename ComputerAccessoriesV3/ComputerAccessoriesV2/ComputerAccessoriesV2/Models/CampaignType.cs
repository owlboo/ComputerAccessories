using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class CampaignType
    {
        public CampaignType()
        {
            Campaign = new HashSet<Campaign>();
        }

        public int TypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Campaign> Campaign { get; set; }
    }
}
