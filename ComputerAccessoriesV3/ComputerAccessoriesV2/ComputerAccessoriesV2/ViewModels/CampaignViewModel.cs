using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class CampaignViewModel
    {
        public int CampaignId { get; set; }
        public String CampaignName { get; set; }
        public int CampaignType { get; set; }
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String Description { get; set; }
        public List<int> ListProduct { get; set; }

    }
}
