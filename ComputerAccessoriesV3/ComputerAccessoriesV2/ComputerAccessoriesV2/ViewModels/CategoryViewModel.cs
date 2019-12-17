using ComputerAccessoriesV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ParentName { get; set; }
        public bool Status { get; set; }
    }

    public class CategoryHomeModel
    {
        public Category MainCategory { get; set; }
        public List<CategoryHomeModel> ListChildrenNode { get; set; }

    }

    public class CategoryShoppingModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? ProductQuantity { get; set; }
    }
}
