using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblUserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual TblRoles Role { get; set; }
        public virtual TblUsers User { get; set; }
    }
}
