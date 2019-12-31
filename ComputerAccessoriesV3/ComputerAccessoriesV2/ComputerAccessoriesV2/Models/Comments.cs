using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Comments
    {
        public int CommentId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public double Star { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LikedNumber { get; set; }

        public virtual Products Comment { get; set; }
    }
}
