using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class BrandViewModel
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Logo { get; set; }
        public bool Status { get; set; }
    }
    public class BrandPartialModel
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public int ProductCount { get; set; }
    }
}
