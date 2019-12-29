using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Reviews
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public double? Star { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LikedNumber { get; set; }
        public string GuestName { get; set; }

        public virtual Products Review { get; set; }
    }
}
