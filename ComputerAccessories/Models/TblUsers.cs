using System;
using System.Collections.Generic;

namespace ComputerAccessories.Models
{
    public partial class TblUsers
    {
        public TblUsers()
        {
            TblUserRole = new HashSet<TblUserRole>();
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string Password { get; set; }
        public bool? IsActivated { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<TblUserRole> TblUserRole { get; set; }
    }
}
