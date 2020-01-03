using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Helpers;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Route("/[controller]/[action]")]
    public class ExportsController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly QueryDbContext _ctx;
        private IHostingEnvironment _hostingEnvironment;
        private UserManager<MyUsers> _userManager;
        public ExportsController(ComputerAccessoriesV2Context db,UserManager<MyUsers> userManager, QueryDbContext ctx,IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _ctx = ctx;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> ExportsBill(string fromDate, string toDate)
        {
            
            try
            {
                var from = new DateTime();
                var to = new DateTime();

                var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                string webRootPath = _hostingEnvironment.WebRootPath;

                from = DateTime.Parse(fromDate);
                to = DateTime.Parse(toDate);

                var listBill = _db.Bills.Include(x => x.Customer).Include(x => x.StatusNavigation).Include(x => x.GuestAnony).Where(x => x.CreateDate >= from && x.CreateDate <= to).ToList();
                List<ExportBillModel> dataExports = new List<ExportBillModel>();
                foreach (var item in listBill)
                {
                    dataExports.Add(new ExportBillModel
                    {
                        CustomerEmail = item.GuestAnonyId.HasValue ? item.GuestAnony.Email : item.Customer.Email,
                        CustomerName = item.GuestAnonyId.HasValue ? item.GuestAnony.CustomerName : item.Customer.DisplayName,
                        BillCode = item.BillName,
                        //BillStatus = item.Status.Value,
                        ShippingStatus = item.Status.HasValue ? item.StatusNavigation.CodeName : "Mới tạo",
                        ShippingAddress = item.ShippingAddress,
                        FirstPrice = item.TotalPrice.Value,
                        LastPrice = item.LastPrice.Value,
                        ProductQuantity = _db.BillDetails.Where(x => x.BillId == item.BillId).Sum(x => x.Quantity).Value,
                        Voucher = item.Voucher,
                        VoucherValue = String.IsNullOrEmpty(item.Voucher) ? 0 : _db.Vouchers.Where(x => x.VoucherName == item.Voucher).FirstOrDefault().Value.Value
                    });
                }

                if (!Directory.Exists(webRootPath + @"/exports"))
                {
                    Directory.CreateDirectory(webRootPath + @"/exports");
                }

                string fileName = userID.ToString() + "_Export_" + fromDate.Replace(@"/", String.Empty) + "_" + toDate.Replace(@"/", String.Empty) + ".xlsx";
                var path = Path.Combine(webRootPath + @"/exports", fileName);

                

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(webRootPath + @"/exports"+"/"+fileName);
                }
                

                FileInfo file = new FileInfo(path);
                ExcelHelpers excelHelpers = new ExcelHelpers();

                excelHelpers.ExportBills(dataExports, from, to).SaveAs(file);

                var memory = new MemoryStream();
                using (var fileStream = new FileStream(path, FileMode.Open))
                {

                    await fileStream.CopyToAsync(memory);
                    fileStream.Close();
                }
                
                memory.Position = 0;
                return File(memory, GetContentType(path), fileName);
            }
            catch (Exception e)
            {
                return Json(e.ToString());
                throw;
            }


            

        }

        public async Task<IActionResult> ExportProducts(string fromDate, string toDate)
        {
            try
            {

                var from = new DateTime();
                var to = new DateTime();

                var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                string webRootPath = _hostingEnvironment.WebRootPath;

                from = DateTime.Parse(fromDate);
                to = DateTime.Parse(toDate);

                var listProducts = _db.Products.Include(x => x.Brand).Include(x => x.Category).Include(x => x.Reviews).Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new ExportProducts
                {
                    ProductName = x.ProductName,
                    ProductCode = x.Code,
                    CreatedDate = x.CreatedDate.Value,
                    RemainedProducts = x.Quantity.Value,
                    BrandName = x.Brand.BrandName,
                    CategoryName = x.Category.CategoryName,
                    Original = x.Origin,
                    UnitPrice = x.OriginalPrice.Value,
                    Status = x.Status == 0 ? "Mới tạo" : "Đã cập nhật thuộc tính",
                    ViewCounts = x.ViewCounts,
                    ReviewCounts = x.Reviews.Where(c => c.ProductId == x.Id).Count(),
                    BuyCounts = _db.BillDetails.Where(c => c.ProductId == x.Id).Sum(c => c.Quantity).Value
                }).ToList();


                if (!Directory.Exists(webRootPath + @"/exports"))
                {
                    Directory.CreateDirectory(webRootPath + @"/exports");
                }

                string fileName = userID.ToString() + "_ExportProducts_" + fromDate.Replace(@"/", String.Empty) + "_" + toDate.Replace(@"/", String.Empty) + ".xlsx";
                var path = Path.Combine(webRootPath + @"/exports", fileName);



                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(webRootPath + @"/exports" + "/" + fileName);
                }
                FileInfo file = new FileInfo(path);
                ExcelHelpers excelHelpers = new ExcelHelpers();
                excelHelpers.ExportProducts(listProducts, from, to).SaveAs(file);
                var memory = new MemoryStream();
                using (var fileStream = new FileStream(path, FileMode.Open))
                {

                    await fileStream.CopyToAsync(memory);
                    fileStream.Close();
                }

                memory.Position = 0;
                return File(memory, GetContentType(path), fileName);


            }
            catch (Exception e)
            {
                return Json(e.ToString());
                throw;
            }
        }


        public async Task<IActionResult> ExportAccount(string fromDate,string toDate)
        {
            try
            {
                var from = new DateTime();
                var to = new DateTime();

                var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                string webRootPath = _hostingEnvironment.WebRootPath;

                from = DateTime.Parse(fromDate);
                to = DateTime.Parse(toDate);

                var listUsers = _db.AspNetUsers.Include(x => x.AspNetUserRoles).Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new AccountGridModel
                {
                    Id = x.Id,
                    PhoneNumber = x.PhoneNumber,
                    DisplayName = x.DisplayName,
                    IsActivated = x.IsActivated.Value,
                    Email = x.Email,
                    Address = x.Address,
                    RoleId = x.AspNetUserRoles.Where(z => z.UserId == x.Id).FirstOrDefault().RoleId,
                    RoleName = _db.AspNetUserRoles.Where(z => z.UserId == x.Id).Join(_db.AspNetRoles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r }).Select(z => z.r.Name).FirstOrDefault(),
                    CreatedDate=x.CreatedDate.Value

                }).ToList();


                if (!Directory.Exists(webRootPath + @"/exports"))
                {
                    Directory.CreateDirectory(webRootPath + @"/exports");
                }

                string fileName = userID.ToString() + "_ExportAccounts_" + fromDate.Replace(@"/", String.Empty) + "_" + toDate.Replace(@"/", String.Empty) + ".xlsx";
                var path = Path.Combine(webRootPath + @"/exports", fileName);



                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(webRootPath + @"/exports" + "/" + fileName);
                }
                FileInfo file = new FileInfo(path);
                ExcelHelpers excelHelpers = new ExcelHelpers();
                excelHelpers.ExportAccounts(listUsers, from, to).SaveAs(file);
                var memory = new MemoryStream();
                using (var fileStream = new FileStream(path, FileMode.Open))
                {

                    await fileStream.CopyToAsync(memory);
                    fileStream.Close();
                }

                memory.Position = 0;
                return File(memory, GetContentType(path), fileName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}