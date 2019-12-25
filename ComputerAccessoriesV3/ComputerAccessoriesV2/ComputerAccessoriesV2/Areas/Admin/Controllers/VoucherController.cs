using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController: Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly UserManager<MyUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public VoucherController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllVoucher()
        {
            return Json(_db.Vouchers.ToList());
        }

        public IActionResult CreateNewVoucher()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateNewVoucher(VoucherViewModel _params)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var voucher = new Vouchers
                    {
                        Value = _params.Value,
                        VoucherName = _params.VoucherName,
                        IsActive = _params.IsActive,
                        DateActive = _params.DateActive,
                        ExpiredDate = _params.ExpiredDate,
                        Max = _params.Max,
                        CreateDate = DateTime.Now
                    };

                    _db.Vouchers.Add(voucher);
                    _db.SaveChanges();

                    scope.Commit();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Tạo mới Voucher thành công!" });
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Lỗi khi tạo Voucher", message = e.Message });
                }
            }
        }

        public JsonResult GetVoucherInfo(int voucherId)
        {
            var voucher = _db.Vouchers.Where(x => x.VoucherId == voucherId).FirstOrDefault();
            if(voucher == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Không tìm thấy Voucher"});
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(voucher);
            }
        }

        public IActionResult EditVoucher(int voucherId)
        {
            if(_db.Vouchers.Any(v => v.VoucherId == voucherId))
            {
                ViewBag.VoucherId = voucherId;
                return View();
            }
            else
            {
                return NotFound();
            }
           
        }

        [HttpPost]
        public JsonResult EditVoucher(VoucherViewModel _params)
        {
            using(var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var voucher = _db.Vouchers.Where(x => x.VoucherId == _params.VoucherId).FirstOrDefault();
                    voucher.Value = _params.Value;
                    voucher.VoucherName = _params.VoucherName;
                    voucher.IsActive = _params.IsActive;
                    voucher.DateActive = _params.DateActive;
                    voucher.ExpiredDate = _params.ExpiredDate;
                    voucher.Max = _params.Max;

                    _db.SaveChanges();

                    scope.Commit();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Thay đổi thông tin Voucher thành công!" });
                } 
                catch(Exception e)
                {
                    scope.Rollback();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Lỗi khi thay đổi thông tin Voucher", message = e.Message });
                }
            }
        }
    }
}
