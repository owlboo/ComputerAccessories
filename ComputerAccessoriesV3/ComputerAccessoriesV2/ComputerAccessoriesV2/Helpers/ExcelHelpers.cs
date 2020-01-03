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
                index++;
            }
            row++;
            int stt = 0;
            foreach (var data in dataExports)
            {
                ws.Cells["A" + row.ToString()].Value = ++stt;
                ws.Cells["B" + row.ToString()].Value = data.BillCode;
                ws.Cells["C" + row.ToString()].Value = data.CustomerName;
                ws.Cells["D" + row.ToString()].Value = data.CustomerEmail;
                ws.Cells["E" + row.ToString()].Value = data.ShippingAddress;
                ws.Cells["F" + row.ToString()].Value = data.ShippingStatus;
                ws.Cells["G" + row.ToString()].Value = data.ProductQuantity;
                ws.Cells["H" + row.ToString()].Value = data.FirstPrice;
                ws.Cells["I" + row.ToString()].Value = data.Voucher;
                ws.Cells["J" + row.ToString()].Value = data.VoucherValue;
                ws.Cells["K" + row.ToString()].Value = data.LastPrice;

                totalProducts += data.ProductQuantity;
                totalPrice += data.LastPrice;
            }

            return pck;
        }
    }

    
}
