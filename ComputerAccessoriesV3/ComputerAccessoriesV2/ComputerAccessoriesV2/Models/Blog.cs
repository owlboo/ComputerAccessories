using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public string BlogName { get; set; }
        public string ShortUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public string Content { get; set; }

        public virtual AspNetUsers CreatedUser { get; set; }
    }
}
