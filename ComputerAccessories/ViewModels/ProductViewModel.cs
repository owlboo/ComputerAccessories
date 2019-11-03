using ComputerAccessories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessories.ViewModels
{
    public class ProductViewModel
    {
        public TblProduct Product { get; set; }
        public IEnumerable<TblBrand> Brands { get; set; }
        public IEnumerable<TblCategory> Categories { get; set; }
        public List<TblProductImages> ProductImages { get; set; }
    }
}
