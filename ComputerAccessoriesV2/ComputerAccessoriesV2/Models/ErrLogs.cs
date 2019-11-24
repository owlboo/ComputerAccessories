using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class ErrLogs
    {
        public int Id { get; set; }
        public string Exception { get; set; }
        public int? UserId { get; set; }
        public int? Type { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
