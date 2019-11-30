using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class AspNetUserClaims
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
