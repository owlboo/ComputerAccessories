using ComputerAccessoriesV2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels.DbQueryModels
{
    public class CampaignProduct
    {
        [ForeignKey(nameof(Products))]
        public int ProductId { get; set; }
        public String ProductName { get; set; }
        public int CampaignDetailId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? PromotionPrice { get; set; }
    }
}
