using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public double? Star { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LikedNumber { get; set; }
        public string GuestName { get; set; }
    }
}
