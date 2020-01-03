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
}
