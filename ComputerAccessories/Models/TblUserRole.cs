using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual TblRoles Role { get; set; }
        public virtual TblUsers User { get; set; }
    }
}
