using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Helpers;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly SignInManager<MyUsers> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ShoppingController(ComputerAccessoriesV2Context db, SignInManager<MyUsers> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductListFilter(int? categoryId , int? brandId,int?sortKey)
        {
            var context = new QueryDbContext();           
            var listProducts = _db.Products.Where(x => x.CategoryId == categoryId || x.BrandId == brandId).Select(x =>
                new ProductGridModel
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId ?? x.CategoryId.Value,
                    CategoryName = x.Category.CategoryName,
                    BrandId = x.BrandId ?? x.BrandId.Value,
                    BrandName = x.Brand.BrandName,
                    OriginalPrice = x.OriginalPrice.Value.ToString("###,###"),
                    PromotionPrice = x.PromotionPrice.HasValue ? x.PromotionPrice.Value.ToString("###,###") : "0",
                    ProductImages = _db.ProductImages.Where(z => z.ProductId == z.Id).ToList(),
                    Quantity = x.Quantity.Value,
                    Thumnail = x.Thumnail,
                    Thumnail2 = x.Thumnail2,
                    ShorDescription = x.ShorDescription,
                    ProductName =x.ProductName
                }).ToList();
            if (sortKey.HasValue)
            {
                switch (sortKey.Value)
                {
                    case 1:
                        listProducts=listProducts.OrderBy(x => x.OriginalPrice).ToList();
                        break;
                    case 2:
                        listProducts=listProducts.OrderByDescending(x => x.OriginalPrice).ToList();
                        break;
                    case 3:
                        listProducts = listProducts.OrderByDescending(x => x.ViewCounts).ToList();
                        break;
                }
            }
            string str =
                @"SELECT c.Id,c.CategoryName,(SELECT Count(id) FROM dbo.Products WHERE CategoryId = c.Id) 'ProductQuantity' FROM dbo.Category c WHERE c.Status = 1";

            var listCategory = context.CategoryShoppingModel.FromSqlRaw(str).Select(x => new CategoryShoppingModel
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                ProductQuantity = x.ProductQuantity.HasValue ? x.ProductQuantity.Value : 0
            }).ToList();

            string brandStr = @"SELECT b.Id,b.BrandName,(SELECT COUNT(Id) FROM dbo.Products WHERE BrandId=b.Id)'ProductCount' FROM dbo.Brand b WHERE 1=1";
            var brandList = context.BrandPartials.FromSqlRaw(brandStr).ToList();

            if (categoryId.HasValue&&categoryId.Value !=0)
                ViewBag.categoryId = categoryId.Value;
            else
                ViewBag.categoryId = 0;
            if (brandId.HasValue&&brandId.Value!=0)
                ViewBag.brandId = brandId.Value;
            else
                ViewBag.brandId = 0;



            ViewBag.category = listCategory;
            ViewBag.brands = brandList;
            return View(listProducts);
        }

        [Route("/[controller]/GetCategories")]
        public IActionResult GetCategories()
        {
            var context = new QueryDbContext();
            string str =
                @"SELECT c.Id,c.CategoryName,(SELECT Count(id) FROM dbo.Products WHERE CategoryId = c.Id) 'ProductQuantity' FROM dbo.Category c WHERE c.Status = 1";

            var listCategory = context.CategoryShoppingModel.FromSqlRaw(str).Select(x=> new CategoryShoppingModel
            {
                Id=x.Id,
                CategoryName = x.CategoryName,
                ProductQuantity = x.ProductQuantity.HasValue ? x.ProductQuantity.Value : 0
            }).ToList();

            return View(listCategory);
        }

        public IActionResult CheckOut()
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value).ToString();
            var context = new QueryDbContext();
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            ShoppingCartPreview previewCart = new ShoppingCartPreview();
            Decimal totalPrice = 0;
            if (!_signInManager.IsSignedIn(User))
            {
                var cookieKey = "CookieShopping";
                string cookie = Request.Cookies[cookieKey];
                if(cookie == null)
                {
                    return View();
                }
                listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                previewCart.ListProducts = listProducts;
                foreach (var item in previewCart.ListProducts)
                {
                    var product = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                    totalPrice += item.Quantity * (product.PromotionPrice.HasValue ? product.PromotionPrice.Value : product.OriginalPrice.Value);
                }

                previewCart.TotalPrice = totalPrice;
                ViewBag.listProduct = previewCart;
                return View();
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.Append(
                @"SELECT u.Id 'UserId',u.DisplayName,u.PhoneNumber,ua.ProvinceId,u.Email,(SELECT ProvinceName FROM dbo.Provinces WHERE ProvinceId=ua.ProvinceId)'ProvinceName',ua.DistrictId,(SELECT DistrictName FROM dbo.Districts WHERE DistrictId = ua.DistrictId) 'DistrictName', ua.WardId, (SELECT WardName FROM dbo.Ward WHERE WardId = ua.WardId) 'WardName',ua.PlaceDetails FROM dbo.AspNetUsers u LEFT JOIN dbo.UserAddress ua ON ua.UserId = u.Id WHERE u.Id = @userId");

            var userInfo = context.UserInformationModels.FromSqlRaw(sqlQuery.ToString(), new SqlParameter("userId", userId)).FirstOrDefault();

            ViewBag.UserInfo = userInfo;


            //Get session list product order
            string key = "SessionSP_" + userId;

            if (_session.GetString(key) == null)
            {
                return View(previewCart);
            }
            else
            {
                listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                previewCart.ListProducts = listProducts;
                foreach (var item in previewCart.ListProducts)
                {
                    //var productPrice = _db.Products.Where(x => x.Id == item.Products.Id).Select(x => x.OriginalPrice).FirstOrDefault();
                    //totalPrice += item.Quantity * (productPrice.HasValue ? productPrice.Value : 0);
                    var products = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                    totalPrice += item.Quantity * (products.PromotionPrice.HasValue ? products.PromotionPrice.Value : products.OriginalPrice.Value);
                }

                previewCart.TotalPrice = totalPrice;
            }

            ViewBag.listProduct = previewCart;

            return View();
        }

        [Route("/[controller]/GetVoucherValue")]
        [HttpGet]
        public JsonResult GetVoucherValue(string code)
        {
            var voucher = _db.Vouchers.Where(x => x.VoucherName.Equals(code) &x.IsActive.Value& x.Used<x.Max).FirstOrDefault();
            if(voucher == null)
            {
                return Json(new { valid = false });
            }
            
            return Json(new { valid = true, value = voucher.Value});
        }

        [Route("/[controller]/AddToBillWithNoLogin")]
        [HttpPost]
        public async Task<IActionResult> AddToBillWithNoLogin(ShoppingBillModel model, string voucher)
        {
            _db.Database.SetCommandTimeout(99999);
            List<ShoppingCartViewModel> listProducts = new List<ShoppingCartViewModel>();
            decimal totalPrice = 0;
            try
            {
                if (model.UserId == 0)
                {
                    string key = "CookieShopping";
                    var cookie = Request.Cookies[key];

                    if (cookie == null)
                    {
                        return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                    }
                    listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(cookie);
                    if (listProducts.Count == 0)
                    {
                        return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                    }
                    foreach (var item in listProducts)
                    {
                        if (item.Quantity >= item.Products.Quantity)
                        {
                            return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                        }
                    }
                    var guest = new NoStroredGuest
                    {
                        CustomerName = model.Name,
                        Email = model.Email,
                        DistrictId = model.DistrictId,
                        ProvinceId = model.ProvinceId,
                        WardId = model.WardId,
                        PhoneNumber = model.PhoneNumber,
                        PlaceDetail = model.PlaceDetail,
                        UserId = model.UserId
                    };
                    _db.NoStroredGuest.Add(guest);
                    await _db.SaveChangesAsync();
                    if (String.IsNullOrEmpty(voucher))
                    {
                        foreach (var item in listProducts)
                        {
                            totalPrice += (item.Products.PromotionPrice.HasValue?item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value) * item.Quantity;
                        }
                        var billObj = new Bills
                        {
                            BillName = AccountHelpers.Guild(6),
                            CreateDate = DateTime.Now,
                            Note = model.Note,
                            GuestAnonyId = guest.Id,
                            TotalPrice = totalPrice,
                            LastPrice = totalPrice,
                            Status=1,
                            ShippingAddress=model.PlaceDetail+" "+GetFullAddress(model.ProvinceId,model.DistrictId,model.WardId)
                        };
                        _db.Bills.Add(billObj);
                        await _db.SaveChangesAsync();

                        var trans = new TransactionHistory
                        {
                            BillId = billObj.BillId,
                            PaymentAmout = totalPrice,
                            CreatedDate = DateTime.Now
                        };
                        _db.TransactionHistory.Add(trans);
                        await _db.SaveChangesAsync();

                        foreach (var item in listProducts)
                        {
                            var billDetail = new BillDetails
                            {
                                BillId = billObj.BillId,
                                ProductId = item.Products.Id,
                                Quantity = item.Quantity,
                                UnitPrice = item.Products.PromotionPrice.HasValue ? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                            };
                            _db.BillDetails.Add(billDetail);
                            var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                            productFromDb.Quantity += item.Quantity;
                            await _db.SaveChangesAsync();
                        }
                        Response.Cookies.Delete(key);
                        return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });
                    }
                    else
                    {
                        foreach (var item in listProducts)
                        {
                            totalPrice +=(item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value: item.Products.OriginalPrice.Value) * item.Quantity;
                        }

                        var voucherDb = _db.Vouchers.Where(x => x.VoucherName == voucher && x.Used < x.Max).FirstOrDefault();
                        decimal lPrice = 0;
                        if (voucher != null)
                        {
                            lPrice = (Decimal)(totalPrice * voucherDb.Value) / 100;
                            voucherDb.Used = voucherDb.Used + 1;
                            if (voucherDb.Used == voucherDb.Max)
                            {
                                voucherDb.IsActive = false;
                            }
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            lPrice = 0;
                        }
                        var billObj = new Bills
                        {
                            BillName = AccountHelpers.Guild(6),
                            CreateDate = DateTime.Now,
                            Note = model.Note,
                            GuestAnonyId = guest.Id,
                            TotalPrice = totalPrice,
                            LastPrice = totalPrice-lPrice,
                            IncludedVoucher = true,
                            Voucher = voucher,
                            ShippingAddress = model.PlaceDetail + " " + GetFullAddress(model.ProvinceId, model.DistrictId, model.WardId),
                            Status=1
                        };
                        _db.Bills.Add(billObj);
                        await _db.SaveChangesAsync();

                        var trans = new TransactionHistory
                        {
                            BillId = billObj.BillId,
                            PaymentAmout = totalPrice,
                            CreatedDate = DateTime.Now
                        };
                        _db.TransactionHistory.Add(trans);
                        await _db.SaveChangesAsync();

                        foreach (var item in listProducts)
                        {
                            var billDetail = new BillDetails
                            {
                                BillId = billObj.BillId,
                                ProductId = item.Products.Id,
                                Quantity = item.Quantity,
                                UnitPrice = item.Products.PromotionPrice.HasValue ? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                            };
                            _db.BillDetails.Add(billDetail);
                            var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                            productFromDb.Quantity -= item.Quantity;
                            await _db.SaveChangesAsync();
                        }
                        Response.Cookies.Delete(key);
                        return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });
                    }

                }
                else
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    string key = "SessionSP_" + userId;
                    if (String.IsNullOrEmpty(_session.GetString(key)))
                    {
                        return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                    }
                    else
                    {
                        listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                        if (listProducts.Count == 0)
                        {
                            return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                        }
                        foreach (var item in listProducts)
                        {
                            if (item.Quantity >= item.Products.Quantity)
                            {
                                return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                            }
                        }
                        if (String.IsNullOrEmpty(voucher))
                        {
                            foreach (var item in listProducts)
                            {
                                totalPrice +=(item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value: item.Products.OriginalPrice.Value) * item.Quantity;
                            }

                            var guest = new NoStroredGuest
                            {
                                CustomerName = model.Name,
                                Email = model.Email,
                                DistrictId = model.DistrictId,
                                ProvinceId = model.ProvinceId,
                                WardId = model.WardId,
                                PhoneNumber = model.PhoneNumber,
                                PlaceDetail = model.PlaceDetail,
                                UserId = model.UserId
                            };

                            _db.NoStroredGuest.Add(guest);
                            await _db.SaveChangesAsync();
                            var billObj = new Bills
                            {
                                BillName = AccountHelpers.Guild(6),
                                CreateDate = DateTime.Now,
                                Note = model.Note,
                                CustomerId = userId,
                                TotalPrice = totalPrice,
                                LastPrice = totalPrice,
                                GuestAnonyId = guest.Id,
                                Status=1,
                                ShippingAddress = model.PlaceDetail + " " + GetFullAddress(model.ProvinceId, model.DistrictId, model.WardId)
                            };
                            _db.Bills.Add(billObj);
                            await _db.SaveChangesAsync();

                            var trans = new TransactionHistory
                            {
                                BillId = billObj.BillId,
                                PaymentAmout = totalPrice,
                                CreatedDate = DateTime.Now
                            };
                            _db.TransactionHistory.Add(trans);
                            await _db.SaveChangesAsync();

                            foreach (var item in listProducts)
                            {
                                var billDetail = new BillDetails
                                {
                                    BillId = billObj.BillId,
                                    ProductId = item.Products.Id,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.Products.PromotionPrice.HasValue ? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                                };
                                _db.BillDetails.Add(billDetail);
                                var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                                productFromDb.Quantity -= item.Quantity;
                                await _db.SaveChangesAsync();
                            }
                            _session.Remove(key);
                            return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });

                        }
                        else
                        {
                            foreach (var item in listProducts)
                            {
                                totalPrice +=(item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value: item.Products.OriginalPrice.Value) * item.Quantity;
                            }

                            var guest = new NoStroredGuest
                            {
                                CustomerName = model.Name,
                                Email = model.Email,
                                DistrictId = model.DistrictId,
                                ProvinceId = model.ProvinceId,
                                WardId = model.WardId,
                                PhoneNumber = model.PhoneNumber,
                                PlaceDetail = model.PlaceDetail,
                                UserId = model.UserId
                            };

                            _db.NoStroredGuest.Add(guest);
                            await _db.SaveChangesAsync();
                            var voucherDb = _db.Vouchers.Where(x => x.VoucherName == voucher && x.Used < x.Max).FirstOrDefault();
                            decimal lPrice = 0;
                            if (voucher != null)
                            {
                                lPrice = (Decimal)(totalPrice * voucherDb.Value) / 100;
                                voucherDb.Used = voucherDb.Used + 1;
                                if (voucherDb.Used == voucherDb.Max)
                                {
                                    voucherDb.IsActive = false;
                                }
                                await _db.SaveChangesAsync();
                            }
                            else
                            {
                                lPrice = 0;
                            }
                            
                            var billObj = new Bills
                            {
                                BillName = AccountHelpers.Guild(6),
                                CreateDate = DateTime.Now,
                                Note = model.Note,
                                CustomerId = userId,
                                TotalPrice = totalPrice,
                                LastPrice = totalPrice-lPrice,
                                IncludedVoucher = true,
                                Voucher = voucher,
                                GuestAnonyId= guest.Id,
                                Status=1,
                                ShippingAddress = model.PlaceDetail + " " + GetFullAddress(model.ProvinceId, model.DistrictId, model.WardId)
                            };
                            _db.Bills.Add(billObj);
                            await _db.SaveChangesAsync();

                            var trans = new TransactionHistory
                            {
                                BillId = billObj.BillId,
                                PaymentAmout = totalPrice-lPrice,
                                CreatedDate = DateTime.Now
                            };
                            _db.TransactionHistory.Add(trans);
                            await _db.SaveChangesAsync();

                            foreach (var item in listProducts)
                            {
                                var billDetail = new BillDetails
                                {
                                    BillId = billObj.BillId,
                                    ProductId = item.Products.Id,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.Products.PromotionPrice.HasValue ? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                                };
                                var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                                productFromDb.Quantity -= item.Quantity;
                                _db.BillDetails.Add(billDetail);
                                await _db.SaveChangesAsync();
                            }
                            _session.Remove(key);
                            return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });
                        }
                    }

                }
            }
            catch (Exception e)
            {
                return  Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
            }
            //ShoppingCartPreview previewCart = new ShoppingCartPreview();          
        }

        [HttpPost]
        [Route("/[controller]/AddToBillWithLogin")]
        public async Task<JsonResult> AddToBillWithLogin(int UserId, string code)
        {
            var checkUser = _db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
            if(checkUser == null)
            {
                return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
            }
            try
            {
                var userAddress = _db.UserAddress.Where(x => x.UserId == UserId).FirstOrDefault();
                string key = "SessionSP_" + UserId;
                var listProducts = JsonConvert.DeserializeObject<List<ShoppingCartViewModel>>(_session.GetString(key));
                if (listProducts.Count == 0)
                    return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                Decimal totalPrice = 0;
                if (String.IsNullOrEmpty(code))
                {
                    foreach (var item in listProducts)
                    {
                        totalPrice +=(item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value: item.Products.OriginalPrice.Value) * item.Quantity;
                        if (item.Quantity > item.Products.Quantity)
                        {
                            return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                        }
                    }

                    var billObj = new Bills
                    {
                        CustomerId = UserId,
                        BillName = AccountHelpers.Guild(6),
                        CreateDate = DateTime.Now,
                        TotalPrice = totalPrice,
                        LastPrice = totalPrice,
                        Status = 1,
                        ShippingAddress = userAddress.PlaceDetails + " " + GetFullAddress(userAddress.ProvinceId, userAddress.DistrictId, userAddress.WardId)
                    };
                    _db.Bills.Add(billObj);
                    await _db.SaveChangesAsync();

                    foreach (var item in listProducts)
                    {
                        var objBillDt = new BillDetails
                        {
                            BillId = billObj.BillId,
                            ProductId = item.Products.Id,
                            Quantity = item.Quantity,
                            UnitPrice = item.Products.PromotionPrice.HasValue ? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                        };
                        var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                        productFromDb.Quantity -= item.Quantity;
                        _db.BillDetails.Add(objBillDt);
                        await _db.SaveChangesAsync();
                    }

                    var trans = new TransactionHistory()
                    {
                        UserId = UserId,
                        PaymentAmout = totalPrice,
                        CreatedDate = DateTime.Now
                    };
                    _db.TransactionHistory.Add(trans);
                    await _db.SaveChangesAsync();
                    _session.Remove(key);
                    return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });
                }
                else
                {
                    
                    foreach (var item in listProducts)
                    {
                        totalPrice +=(item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value: item.Products.OriginalPrice.Value) * item.Quantity;
                        if (item.Quantity > item.Products.Quantity)
                        {
                            return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
                        }
                    }

                    var voucher = _db.Vouchers.Where(x => x.VoucherName == code && x.Used < x.Max).FirstOrDefault();
                    decimal lPrice = 0;
                    if (voucher == null)
                    {
                        lPrice = 0;
                    }
                    else
                    {
                        lPrice = (Decimal)(totalPrice * voucher.Value) / 100;
                        voucher.Used = voucher.Used + 1;
                        if (voucher.Used == voucher.Max)
                        {
                            voucher.IsActive = false;
                        }
                        await _db.SaveChangesAsync();
                    }

                    var billObj = new Bills
                    {
                        CustomerId = UserId,
                        BillName = AccountHelpers.Guild(6),
                        CreateDate = DateTime.Now,
                        TotalPrice = totalPrice,
                        LastPrice = totalPrice-lPrice,
                        IncludedVoucher=true,
                        Voucher = voucher.VoucherName,
                        Status = 1,
                        ShippingAddress = userAddress.PlaceDetails + " " + GetFullAddress(userAddress.ProvinceId, userAddress.DistrictId, userAddress.WardId)
                    };
                    _db.Bills.Add(billObj);
                    await _db.SaveChangesAsync();

                    foreach (var item in listProducts)
                    {
                        var objBillDt = new BillDetails
                        {
                            BillId = billObj.BillId,
                            ProductId = item.Products.Id,
                            Quantity = item.Quantity,
                            UnitPrice = item.Products.PromotionPrice.HasValue? item.Products.PromotionPrice.Value : item.Products.OriginalPrice.Value
                        };
                        var productFromDb = _db.Products.Where(x => x.Id == item.Products.Id).FirstOrDefault();
                        productFromDb.Quantity -= item.Quantity;
                        _db.BillDetails.Add(objBillDt);
                        await _db.SaveChangesAsync();
                    }

                    var trans = new TransactionHistory()
                    {
                        UserId = UserId,
                        PaymentAmout = totalPrice-lPrice,
                        CreatedDate = DateTime.Now
                    };
                    _db.TransactionHistory.Add(trans);
                    await _db.SaveChangesAsync();
                    _session.Remove(key);
                    return Json(new { code = 1, billCode = billObj.BillName, returnUrl = "/Customer/Home/Index" });
                }
            }
            catch (Exception e)
            {

                return Json(new { code = 0, returnUrl = "/Customer/Home/Index" });
            }
            
        }

        private string GetFullAddress(int provinceId,int districtId, int wardId)
        {
            var provinceName = _db.Provinces.Where(x => x.ProvinceId == provinceId).Select(x => x.ProvinceName).FirstOrDefault();
            var districtName = _db.Districts.Where(x => x.DistrictId == districtId).Select(x => x.DistrictName).FirstOrDefault();
            var wardName = _db.Ward.Where(x => x.WardId == wardId).Select(x => x.WardName).FirstOrDefault();

            return wardName + ", " + districtName + ", " + provinceName;
        }
    }
}