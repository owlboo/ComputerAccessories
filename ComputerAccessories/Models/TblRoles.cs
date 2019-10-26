using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblRoles
    {
        public TblRoles()
        {
            TblUserRole = new HashSet<TblUserRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual ICollection<TblUserRole> TblUserRole { get; set; }
    }
}
