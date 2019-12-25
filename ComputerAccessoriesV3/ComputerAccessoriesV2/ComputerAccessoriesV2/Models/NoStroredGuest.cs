using System;
using System.Collections.Generic;

namespace ComputerAccessoriesV2.Models
{
    public partial class NoStroredGuest
    {
        public NoStroredGuest()
        {
            Bills = new HashSet<Bills>();
        }

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public int? WardId { get; set; }
        public string PlaceDetail { get; set; }
        public int? UserId { get; set; }

        public virtual ICollection<Bills> Bills { get; set; }
    }
}
