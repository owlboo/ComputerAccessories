using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessories.ViewModels
{
    public class AttributeViewModel
    {
        public int Id { get; set; }
        public string AttributeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
