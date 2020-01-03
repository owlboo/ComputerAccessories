using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class ExportModels
    {
    }

    public class ExportBillModel
    {
        public string BillCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingStatus { get; set; }
        public int BillStatus { get; set; }
        public int ProductQuantity { get; set; }
        public Decimal FirstPrice { get; set; }
        public string Voucher { get; set; }
        public int VoucherValue { get; set; }
        public Decimal LastPrice { get; set; }
    }

    public class ExportProducts
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Original { get; set; }
        public int RemainedProducts { get; set; }
        public int ViewCounts { get; set; }
        public int ReviewCounts { get; set; }
        public int BuyCounts { get; set; }
        public string Status { get; set; }
        public Decimal UnitPrice { get; set; }
    }
}
