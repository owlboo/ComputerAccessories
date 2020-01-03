using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Helpers
{
    
    public class ExcelHelpers
    {
        public ExcelHelpers()
        {
                
        }
        public ExcelPackage ExportBills(List<ExportBillModel> dataExports, DateTime from, DateTime to)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = null;

            int totalProducts = 0;
            Decimal totalPrice = 0;

            ws = pck.Workbook.Worksheets.Add("Báo cáo sản lượng");

            #region head
            ws.Cells["E2:G2"].Merge = true;
            ws.Cells["E2:G2"].Value = "BÁO CÁO SẢN LƯỢNG HÓA ĐƠN";
            ws.Cells["E2:G2"].Style.Font.Size = 16;
            ws.Cells["E2:G2"].Style.Font.Bold = true;
            ws.Cells["E2:G2"].AutoFitColumns();

            ws.Cells["E4"].Value = "Từ ngày";
            ws.Cells["F4"].Value = from.ToLocalTime().ToString();

            ws.Cells["E5"].Value = "Đến ngày";
            ws.Cells["F5"].Value = to.ToLocalTime().ToString();

            #endregion

            string[] title = new string[] { "STT", "Mã hóa đơn", "Tên khách hàng", "Email", "Địa chỉ giao hàng", "Trạng thái", "Số lượng sản phẩm", "Giá trị hóa đơn", "Voucher", "Giá trị voucher", "Tổng tiền" };
            string col = "ABCDEFGHIJK";
            int row = 7;
            int index = 0;
            foreach (var item in col)
            {
                ws.Cells[item + row.ToString()].Value = title[index];
                ws.Cells[item + row.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ws.Cells[item + row.ToString()].Style.Font.Bold = true;
                ws.Cells[item + row.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[item + row.ToString()].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ws.Cells[item + row.ToString()].AutoFitColumns();
                index++;
            }
            row++;
            int stt = 0;
            foreach (var data in dataExports)
            {
                ws.Cells["A" + row.ToString()].Value = ++stt;
                ws.Cells["B" + row.ToString()].Value = data.BillCode;

                ws.Cells["C" + row.ToString()].Value = data.CustomerName;
                ws.Cells["C" + row.ToString()].AutoFitColumns();

                ws.Cells["D" + row.ToString()].Value = data.CustomerEmail;
                ws.Cells["D" + row.ToString()].AutoFitColumns();

                ws.Cells["E" + row.ToString()].Value = data.ShippingAddress;
                ws.Cells["E" + row.ToString()].AutoFitColumns();

                ws.Cells["F" + row.ToString()].Value = data.ShippingStatus;
                ws.Cells["G" + row.ToString()].Value = data.ProductQuantity;
                ws.Cells["H" + row.ToString()].Value = data.FirstPrice;
                ws.Cells["I" + row.ToString()].Value = data.Voucher;
                ws.Cells["J" + row.ToString()].Value = data.VoucherValue;
                ws.Cells["K" + row.ToString()].Value = data.LastPrice;

                totalProducts += data.ProductQuantity;
                totalPrice += data.LastPrice;
                row++;
            }
            ws.Cells["G" + row.ToString()].Value = totalProducts;
            ws.Cells["K" + row.ToString()].Value = totalPrice;
            return pck;
        }
        public ExcelPackage ExportProducts(List<ExportProducts> dataExports, DateTime from, DateTime to)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = null;

            ws = pck.Workbook.Worksheets.Add("Báo cáo sản lượng sản phẩm");

            #region head
            ws.Cells["E2:G2"].Merge = true;
            ws.Cells["E2:G2"].Value = "BÁO CÁO SẢN PHẨM";
            ws.Cells["E2:G2"].Style.Font.Size = 16;
            ws.Cells["E2:G2"].Style.Font.Bold = true;
            ws.Cells["E2:G2"].AutoFitColumns();

            ws.Cells["E4"].Value = "Từ ngày";
            ws.Cells["F4"].Value = from.ToLocalTime().ToString();

            ws.Cells["E5"].Value = "Đến ngày";
            ws.Cells["F5"].Value = to.ToLocalTime().ToString();

            #endregion

            string[] title = new string[] { "STT", "Tên sản phẩm", "Mã sản phẩm", "Ngày tạo", "Số lượng còn lại", "Thương hiệu", "Danh mục", "Xuất xứ", "Lượt xem", "Lượt mua", "Lượt đánh giá","Trạng thái","Đơn giá" };
            string col = "ABCDEFGHIJKLM";
            int row = 7;
            int index = 0;
            foreach (var item in col)
            {
                ws.Cells[item + row.ToString()].Value = title[index];
                ws.Cells[item + row.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ws.Cells[item + row.ToString()].Style.Font.Bold = true;
                ws.Cells[item + row.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[item + row.ToString()].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ws.Cells[item + row.ToString()].AutoFitColumns();
                index++;
            }

            row++;
            int stt = 0;
            foreach (var data in dataExports)
            {
                ws.Cells["A" + row.ToString()].Value = ++stt;

                ws.Cells["B" + row.ToString()].Value = data.ProductName;
                ws.Cells["B" + row.ToString()].AutoFitColumns();

                ws.Cells["C" + row.ToString()].Value = data.ProductCode;

                ws.Cells["D" + row.ToString()].Value = data.CreatedDate.ToLocalTime().ToString();
                ws.Cells["D" + row.ToString()].AutoFitColumns();

                ws.Cells["E" + row.ToString()].Value = data.RemainedProducts;

                ws.Cells["F" + row.ToString()].Value = data.BrandName;

                ws.Cells["G" + row.ToString()].Value = data.CategoryName;

                ws.Cells["H" + row.ToString()].Value = data.Original;

                ws.Cells["I" + row.ToString()].Value = data.ViewCounts;

                ws.Cells["J" + row.ToString()].Value = data.BuyCounts;

                ws.Cells["K" + row.ToString()].Value = data.ReviewCounts;

                ws.Cells["L" + row.ToString()].Value = data.Status;

                ws.Cells["M" + row.ToString()].Value = data.UnitPrice;
                row++;
            }
            return pck;
        }

        public ExcelPackage ExportAccounts(List<AccountGridModel> dataExports, DateTime from, DateTime to)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = null;

            ws = pck.Workbook.Worksheets.Add("Báo cáo sản lượng sản phẩm");

            #region head
            ws.Cells["C2:E2"].Merge = true;
            ws.Cells["C2:E2"].Value = "BÁO CÁO TÀI KHOẢN";
            ws.Cells["C2:E2"].Style.Font.Size = 16;
            ws.Cells["C2:E2"].Style.Font.Bold = true;
            ws.Cells["C2:E2"].AutoFitColumns();

            ws.Cells["C4"].Value = "Từ ngày";
            ws.Cells["D4"].Value = from.ToLocalTime().ToString();

            ws.Cells["C5"].Value = "Đến ngày";
            ws.Cells["D5"].Value = to.ToLocalTime().ToString();

            #endregion

            string[] title = new string[] { "STT", "Tên khách hàng", "Email", "Số điện thoại", "Ngày tạo", "Trạng thái", "Vai trò", "Địa chỉ"};
            string col = "ABCDEFGH";
            int row = 7;
            int index = 0;
            foreach (var item in col)
            {
                ws.Cells[item + row.ToString()].Value = title[index];
                ws.Cells[item + row.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ws.Cells[item + row.ToString()].Style.Font.Bold = true;
                ws.Cells[item + row.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[item + row.ToString()].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ws.Cells[item + row.ToString()].AutoFitColumns();
                index++;
            }

            row++;
            int stt = 0;
            foreach (var data in dataExports)
            {
                ws.Cells["A" + row.ToString()].Value = ++stt;
                ws.Cells["A" + row.ToString()].AutoFitColumns();

                ws.Cells["B" + row.ToString()].Value = data.DisplayName;
                ws.Cells["B" + row.ToString()].AutoFitColumns();

                ws.Cells["C" + row.ToString()].Value = data.Email;
                ws.Cells["C" + row.ToString()].AutoFitColumns();

                ws.Cells["D" + row.ToString()].Value = data.PhoneNumber;
                ws.Cells["D" + row.ToString()].AutoFitColumns();

                ws.Cells["E" + row.ToString()].Value = data.CreatedDate.ToLocalTime().ToString();
                ws.Cells["E" + row.ToString()].AutoFitColumns();

                ws.Cells["F" + row.ToString()].Value = data.IsActivated ? "Đã kích hoat": "Khóa/chưa kích hoạt";
                ws.Cells["F" + row.ToString()].AutoFitColumns();

                ws.Cells["G" + row.ToString()].Value = data.RoleName;
                ws.Cells["G" + row.ToString()].AutoFitColumns();

                ws.Cells["H" + row.ToString()].Value = data.Address;
                ws.Cells["H" + row.ToString()].AutoFitColumns();
                row++;
            }
            return pck;
        }

        public ExcelPackage ExportBillDetails(ExportBillDetailModel dataExport)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = null;

            ws = pck.Workbook.Worksheets.Add("Chi tiết hóa đơn");


            #region Format Header
            ws.Cells["C2:D2"].Merge = true;
            ws.Cells["C2:D2"].Value = "CHI TIẾT HÓA ĐƠN";
            ws.Cells["C2:D2"].Style.Font.Size = 16;
            ws.Cells["C2:D2"].Style.Font.Bold = true;
            ws.Cells["C2:D2"].AutoFitColumns();

            ws.Cells["C4"].Value = "Mã hóa đơn";
            ws.Cells["C4"].Style.Font.Bold = true;
            ws.Cells["C4"].AutoFitColumns();
            ws.Cells["D4"].Value = dataExport.BillCode;
            ws.Cells["D4"].AutoFitColumns();

            ws.Cells["C5"].Value = "Tên khách hàng";
            ws.Cells["C5"].Style.Font.Bold = true;
            ws.Cells["C5"].AutoFitColumns();
            ws.Cells["D5"].Value = dataExport.CustomerName;
            ws.Cells["D5"].AutoFitColumns();

            ws.Cells["C6"].Value = "Số điện thoại";
            ws.Cells["C6"].AutoFitColumns();
            ws.Cells["C6"].Style.Font.Bold = true;
            ws.Cells["D6"].Value = dataExport.CustomerPhone;
            ws.Cells["D6"].AutoFitColumns();

            ws.Cells["C7"].Value = "Email khách hàng";
            ws.Cells["C7"].Style.Font.Bold = true;
            ws.Cells["C7"].AutoFitColumns();
            ws.Cells["D7"].Value = dataExport.CustomerEmail;

            ws.Cells["C8"].Value = "Ngày thanh toán";
            ws.Cells["C8"].Style.Font.Bold = true;
            ws.Cells["C8"].AutoFitColumns();
            ws.Cells["D8"].Value = dataExport.CreatedDate.ToLocalTime().ToString();

            ws.Cells["C9"].Value = "Địa chỉ giao hàng";
            ws.Cells["C9"].Style.Font.Bold = true;
            ws.Cells["C9"].AutoFitColumns();
            ws.Cells["D9"].Value = dataExport.ShippingAddress;

            #endregion

            int row = 11;
            Decimal totalPrice = 0;
            int index = 0;
            string[] title = new string[] { "STT", "Tên sản phẩm", "Mã sản phẩm", "Giá gốc", "Giá bán", "Số lượng", "Tổng tiền" };
            string col = "ABCDEFG";

            foreach (var c in col)
            {
                ws.Cells[c + row.ToString()].Value = title[index];
                ws.Cells[c + row.ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ws.Cells[c + row.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[c + row.ToString()].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                ws.Cells[c + row.ToString()].Style.Font.Bold = true;
                index++;
            }

            int stt = 0;
            int totalProduct = 0;
            row++;
            foreach (var data in dataExport.ListProducts)
            {
                ws.Cells["A" + row.ToString()].Value = ++stt;
                ws.Cells["B" + row.ToString()].Value = data.ProductName;
                ws.Cells["B" + row.ToString()].AutoFitColumns();
                ws.Cells["C" + row.ToString()].Value = data.ProductCode;
                ws.Cells["D" + row.ToString()].Value = data.OriginPrice;
                ws.Cells["E" + row.ToString()].Value = data.SellPrice;
                ws.Cells["F" + row.ToString()].Value = data.Quantity;
                ws.Cells["G" + row.ToString()].Value = data.SellPrice * data.Quantity;
                totalPrice+= data.SellPrice * data.Quantity;
                totalProduct += data.Quantity;
                row++;
            }

            ws.Cells["F" + row.ToString()].Value = totalProduct;
            ws.Cells["F" + row.ToString()].Style.Font.Bold = true;
            ws.Cells["G" + row.ToString()].Value = totalPrice;
            ws.Cells["G" + row.ToString()].Style.Font.Bold = true;

            return pck;

        }
    }

    
}
