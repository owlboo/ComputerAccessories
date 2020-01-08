using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BillController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly QueryDbContext _context;
        public BillController(ComputerAccessoriesV2Context db,QueryDbContext context)
        {
            _db = db;
            _context = context;
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult BillManagement()
        {
            return View();
        }

        [Route("/[controller]/GetBills")]
        [HttpGet]
        public JsonResult GetBills(string customerEmail, string customerPhone, string billCode, int?billStatus, string fromTime, string toTime)
        {


            var predicate = PredicateBuilder.True<Bills>();

            var query = _db.Bills.Include(x => x.Customer).Include(x => x.GuestAnony).AsNoTracking().AsQueryable();
            if (!String.IsNullOrEmpty(customerEmail))
            {
                var user = _db.AspNetUsers.Where(x => x.Email.Equals(customerEmail)).FirstOrDefault();
                if (user != null)
                {
                    predicate= predicate.And(x => x.CustomerId == user.Id);
                }
            }

            if (!String.IsNullOrEmpty(customerPhone))
            {
                predicate= predicate.And(x => x.Customer.PhoneNumber.Equals(customerPhone) || x.GuestAnony.PhoneNumber.Equals(customerPhone));
            }

            if (!String.IsNullOrEmpty(billCode))
            {
                predicate= predicate.And(x => x.BillName.Equals(billCode.ToLower()));
            }

            if (billStatus.HasValue)
            {
                predicate= predicate.And(x => x.Status == billStatus.Value);
            }

            if (!String.IsNullOrEmpty(fromTime)&&!String.IsNullOrEmpty(toTime))
            {
                var from = DateTime.Parse(fromTime);
                var to = DateTime.Parse(toTime + " 23:59:59");
                predicate.And(x => x.CreateDate >= from && x.CreateDate <= to);
            }

            return Json(query.Where(predicate).Select(x => new BillModelHolders
            {
                billId = x.BillId,
                billName = x.BillName,
                customerName = x.GuestAnonyId.HasValue ? x.GuestAnony.CustomerName : x.Customer.DisplayName,
                customerEmail = x.GuestAnonyId.HasValue ? x.GuestAnony.Email : x.Customer.Email,
                phoneNumber = x.GuestAnonyId.HasValue ? x.GuestAnony.PhoneNumber : x.Customer.PhoneNumber,
                createdDate = x.CreateDate,
                totalPrice = x.LastPrice.Value,
                shippingAddress = x.ShippingAddress,
                note = x.Note,
                status = x.OrderStatusLog.OrderBy(log => log.ModifyDate).Last().NewStatus,
                statusCode = x.Status.HasValue ? x.StatusNavigation.CodeName : "",
                DeliveredDate = x.DeliveredDate,
                shipper = new ViewModels.DbQueryModels.ShipperModel()
                {
                    shipperId = x.ShipperId.Value,
                    shipperName = x.ShipperId.HasValue ? _db.AspNetUsers.Where(z => z.Id == x.ShipperId).Select(z => z.DisplayName).FirstOrDefault() : ""
                }

            }).ToList());

        }

        [Route("/[controller]/GetShipper")]
        public IActionResult GetShipper()
        {

            var query = @"SELECT u.Id'shipperId',u.DisplayName'shipperName' FROM dbo.AspNetUsers u JOIN dbo.AspNetUserRoles ur ON ur.UserId = u.Id WHERE ur.RoleId=4";
            var listShipper = _context.Shippers.FromSqlRaw(query);
            return Json(listShipper);
        }

        [Route("/[controller]/UpdateBillInformation")]
        public async Task<IActionResult> UpdateBillInformation(string models)
        {
            //models = models.Replace(@"/", "");
            
            var billModel = JsonConvert.DeserializeObject<BillModelHolders>(models);

            if (billModel.shipper.shipperId == 0)
            {
                var billFromDb = _db.Bills.Where(x => x.BillId == billModel.billId).FirstOrDefault();

                billFromDb.Note = billModel.note;
                await _db.SaveChangesAsync();
                return Json(new { res = "Đã cập nhật ghi chú cho đơn hàng:"+billFromDb.BillName });
            }
            else
            {
                var billFromDb = _db.Bills.Where(x => x.BillId == billModel.billId).FirstOrDefault();
                var curShipper = _db.AspNetUsers.Where(x => x.Id == billFromDb.ShipperId).Select(x => x.DisplayName).FirstOrDefault();
                if (billFromDb.ShipperId.HasValue)
                {
                    billFromDb.Note = billModel.note;
                    billFromDb.ShipperId = billModel.shipper.shipperId;
                    billFromDb.Status = 2;
                    await _db.SaveChangesAsync();

                    _db.OrderStatusLog.Add(new OrderStatusLog
                    {
                        BillId = billFromDb.BillId,
                        NewStatus = 2,
                        ModifyDate = DateTime.Now,
                        ModifyUserId = 0,
                        Note = "Gán đơn hàng cho shipper: " + billModel.shipper.shipperName
                    });
                    await _db.SaveChangesAsync();

                    return Json(new { res = $"Đơn hàng: {billFromDb.BillName } đã được thay đổi từ {curShipper} sang {billModel.shipper.shipperName}" });
                }
                else
                {
                    billFromDb.Note = billModel.note;
                    billFromDb.ShipperId = billModel.shipper.shipperId;
                    billFromDb.Status = 2;
                    await _db.SaveChangesAsync();

                    _db.OrderStatusLog.Add(new OrderStatusLog
                    {
                        BillId = billFromDb.BillId,
                        NewStatus = 2,
                        ModifyDate = DateTime.Now,
                        ModifyUserId = 0,
                        Note = "Gán đơn hàng cho shipper: " + billModel.shipper.shipperName
                    });
                    await _db.SaveChangesAsync();

                    return Json(new { res = "Shipper : " + billModel.shipper.shipperName + " đã được gán vào đơn hàng : " + billFromDb.BillName });
                }
            }
        }

        [Route("/[controller]/GetBillStatus")]
        public IActionResult GetBillStatus()
        {
            return Json(_db.BillStatus.ToList());
        }


        public IActionResult BillDetails(int? billId)
        {
            BillDetailModel model = new BillDetailModel();

            if (billId == null)
            {
                return NotFound();
            }
            else
            {
                
                var userInfo = new UserInformationModel();
                var billFromDb = _db.Bills.Where(x => x.BillId == billId).FirstOrDefault();
                ShoppingCartPreview cart = new ShoppingCartPreview();
                cart.ListProducts = new List<ShoppingCartViewModel>();
                ShoppingCartViewModel shoppingVM = new ShoppingCartViewModel();

                if (billFromDb.GuestAnonyId.HasValue)
                {
                    var guest = _db.NoStroredGuest.Where(x => x.Id == billFromDb.GuestAnonyId).FirstOrDefault();
                    var fullAddress = guest.PlaceDetail+" "+ GetFullAddress(guest.ProvinceId.Value, guest.DistrictId.Value, guest.WardId.Value);
                    userInfo = _db.NoStroredGuest.Where(x => x.Id == billFromDb.GuestAnonyId).Select(x => new UserInformationModel
                    {
                        DisplayName = x.CustomerName,
                        PhoneNumber = x.PhoneNumber,
                        Email = x.Email
                    }).FirstOrDefault();

                    var billDetails = _db.BillDetails.Where(x => x.BillId == billId).ToList();
                    foreach (var item in billDetails)
                    {
                        cart.ListProducts.Add(new ShoppingCartViewModel { 
                            Products = _db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault(),
                            Quantity = item.Quantity.Value,
                            UniPrice = item.UnitPrice.Value
                        });

                    }
                    cart.TotalPrice = billFromDb.LastPrice.Value;
                    
                    model.shoppingCart = cart;
                    model.ShipperId = billFromDb.ShipperId.HasValue?billFromDb.ShipperId.Value:0;
                    model.ShipperName = billFromDb.ShipperId.HasValue ? _db.AspNetUsers.Where(x => x.Id == billFromDb.CustomerId).Select(x => x.DisplayName).FirstOrDefault() : "";
                    model.BillStatus = _db.BillStatus.Where(x => x.Id == billFromDb.Status).Select(x => x.CodeName).FirstOrDefault();
                    model.userInfo = userInfo;
                    model.VoucherApplied = billFromDb.Voucher;
                    model.FinalPrice = billFromDb.LastPrice.Value;
                    model.FullAddress = billFromDb.ShippingAddress;
                }
                else
                {
                    var userFrom = _db.AspNetUsers.Where(x => x.Id == billFromDb.CustomerId).FirstOrDefault();
                    var userAdress = _db.UserAddress.Where(x => x.UserId == userFrom.Id).FirstOrDefault();
                    var fullAdress = userAdress.PlaceDetails+" "+ GetFullAddress(userAdress.ProvinceId, userAdress.DistrictId, userAdress.WardId);                   
                    userInfo.DisplayName = userFrom.DisplayName;
                    userInfo.Email = userFrom.Email;
                    userInfo.PhoneNumber = userFrom.PhoneNumber;


                    var billDetails = _db.BillDetails.Where(x => x.BillId == billId).ToList();
                    foreach (var item in billDetails)
                    {
                        cart.ListProducts.Add(new ShoppingCartViewModel
                        {
                            Products = _db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault(),
                            Quantity = item.Quantity.Value,
                            UniPrice = item.UnitPrice.Value
                        });

                    }
                    cart.TotalPrice = billFromDb.LastPrice.Value;
                    model.shoppingCart = cart;
                    model.ShipperId = billFromDb.ShipperId.HasValue ? billFromDb.ShipperId.Value : 0;
                    model.ShipperName = billFromDb.ShipperId.HasValue ? _db.AspNetUsers.Where(x => x.Id == billFromDb.CustomerId).Select(x => x.DisplayName).FirstOrDefault() : "";
                    model.BillStatus = _db.BillStatus.Where(x => x.Id == billFromDb.Status).Select(x => x.CodeName).FirstOrDefault();
                    model.userInfo = userInfo;
                    model.VoucherApplied = billFromDb.Voucher;
                    model.FinalPrice = billFromDb.LastPrice.Value;
                    model.FullAddress = billFromDb.ShippingAddress;
                }
            }
            return View(model);
        }

        private string GetFullAddress(int provinceId, int districtId, int wardId)
        {
            var provinceName = _db.Provinces.Where(x => x.ProvinceId == provinceId).Select(x => x.ProvinceName).FirstOrDefault();
            var districtName = _db.Districts.Where(x => x.DistrictId == districtId).Select(x => x.DistrictName).FirstOrDefault();
            var wardName = _db.Ward.Where(x => x.WardId == wardId).Select(x => x.WardName).FirstOrDefault();

            return wardName + ", " + districtName + ", " + provinceName;
        }

    }
}