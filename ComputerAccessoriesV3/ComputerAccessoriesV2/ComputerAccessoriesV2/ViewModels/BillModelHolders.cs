using ComputerAccessoriesV2.ViewModels.DbQueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.ViewModels
{
    public class BillModelHolders
    {
        public int billId { get; set; }
        public string billName { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public int customerId { get; set; }
        public DateTime? createdDate { get; set; }
        public string shippingAddress { get; set; }
        public Decimal totalPrice { get; set; }
        public string note { get; set; }
        public string phoneNumber { get; set; }
        public int status { get; set; }
        public string statusCode { get; set; }
        public ShipperModel shipper { get; set; }
        public DateTime? DeliveredDate { get; set; }


    }
    public class BillDetailModel
    {

        public ShoppingCartPreview shoppingCart { get; set; }
        public UserInformationModel userInfo { get; set; }
        public int StatusId { get; set; }
        public string BillStatus { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public string VoucherApplied { get; set; }
        public Decimal FinalPrice { get; set; }
        public string FullAddress { get; set; }
    }
}
