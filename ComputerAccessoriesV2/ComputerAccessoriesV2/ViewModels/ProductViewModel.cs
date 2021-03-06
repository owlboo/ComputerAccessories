﻿using ComputerAccessoriesV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public List<ProductImages> ProductImages { get; set; }
    }
}
