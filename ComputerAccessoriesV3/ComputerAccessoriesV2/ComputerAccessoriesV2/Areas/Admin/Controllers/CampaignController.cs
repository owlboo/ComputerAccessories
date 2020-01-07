using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CampaignController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly UserManager<MyUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public CampaignController(ComputerAccessoriesV2Context db, UserManager<MyUsers> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpGet]
        public JsonResult GetAllCampaign()
        {
            var listCampaign = _db.Campaign.ToList();
            return Json(listCampaign);
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult CreateNewCampaign()
        {
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        public JsonResult CreateNewCampaign(CampaignViewModel _params)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var newCampaign = new Campaign
                    {
                        CampaignName = _params.CampaignName,
                        TypeId = _params.CampaignType,
                        Status = true,
                        StartDate = _params.StartDate,
                        EndDate = _params.EndDate,
                        CreatedDate = DateTime.Now,
                        Description = _params.Description
                    };

                    _db.Campaign.Add(newCampaign);
                    _db.SaveChanges();

                    foreach (var item in _params.ListProduct)
                    {
                        _db.CampaignDetails.Add(new CampaignDetails
                        {
                            CampaignId = newCampaign.CampaignId,
                            ProductId = item,
                        });

                        _db.SaveChanges();
                    }

                    scope.Commit();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { newCampaignId = newCampaign.CampaignId });
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Error when insert new Campaign", message = e.Message });
                }
            }
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult EditCampaign(int id)
        {
            if (_db.Campaign.Any(o => o.CampaignId == id))
            {
                ViewBag.CampaignId = id;
                return View();
            }
            else
            {
                return NotFound();
            }

        }


        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        public JsonResult EditCampaign(CampaignViewModel _params)
        {
            var campaign = _db.Campaign.Where(x => x.CampaignId == _params.CampaignId).FirstOrDefault();
            if (campaign == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Không tìm thấy Campaign" });
            }
            else
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var changeStatus = campaign.Status != _params.Status;

                        campaign.CampaignName = _params.CampaignName;
                        campaign.Description = _params.Description;
                        campaign.StartDate = _params.StartDate;
                        campaign.EndDate = _params.EndDate;
                        campaign.TypeId = _params.CampaignType;
                        campaign.Status = _params.Status;

                        _db.SaveChanges();

                        if (changeStatus)
                        {
                            if (_params.Status)
                            {
                                var listAffectProduct = (
                                from p in _db.Products
                                join cd in _db.CampaignDetails on p.Id equals cd.ProductId
                                where cd.CampaignId == _params.CampaignId
                                select new
                                {
                                    p,
                                    cd
                                }
                                ).ToList();

                                listAffectProduct.ForEach(item =>
                                {
                                    item.p.PromotionPrice = item.cd.PromotionPrice;
                                    _db.SaveChanges();
                                });
                            }
                            else
                            {
                                var listAffectProduct = (
                                from p in _db.Products
                                join cd in _db.CampaignDetails on p.Id equals cd.ProductId
                                join c in _db.Campaign on cd.CampaignId equals c.CampaignId
                                where c.CampaignId == _params.CampaignId
                                select p
                                ).ToList();

                                listAffectProduct.ForEach(p =>
                                {
                                    p.PromotionPrice = null;
                                    _db.SaveChanges();
                                });
                            }

                        }
                        scope.Commit();

                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new { notify = "Thay đổi Campaign Info thành công!" });
                    }
                    catch
                    {
                        scope.Rollback();
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { notify = "Lỗi khi update thông tin campaign" });
                    }
                }
            }
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetAllCampaignType()
        {
            return Json(_db.CampaignType.ToList());
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetAllProductForNewCampaign()
        {
            var context = new QueryDbContext();
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append(@"select p.Id as ProductId, p.ProductName
                            from dbo.Products p
                            where Id not in
                            (select distinct p.Id
                            from dbo.Products p
                            join dbo.CampaignDetails cd on cd.ProductId = p.Id
                            join dbo.Campaign c on c.CampaignId = cd.CampaignId
                            where c.EndDate > CURRENT_TIMESTAMP and c.Status = 1)");

            var listAvailableProduct = context.ProductAvailableForCompaign.FromSqlRaw(sqlQuery.ToString()).ToList();
            return (Json(listAvailableProduct));
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetAllProductInCampaign(int campaignId)
        {
            var context = new QueryDbContext();
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append(@"select p.ProductName, cd.PromotionPrice, p.OriginalPrice, cd.ProductId, cd.CampaignDetailId
                                from dbo.CampaignDetails cd, dbo.Products p
                                where cd.CampaignId = @campaignId and p.Id = cd.ProductId");

            var listProduct = context.CampaignProducts.FromSqlRaw(sqlQuery.ToString(), new SqlParameter("campaignId", campaignId)).ToList();
            return (Json(listProduct));
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult UpdateProductInCampaignDetail([FromQuery(Name = "models")] string models)
        {
            var list = ParseListCampaginDetail(models);
            if (list == null || list.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(models);
            }
            else
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in list)
                        {
                            var compainDetailDb = _db.CampaignDetails.Where(x => x.CampaignDetailId == item.campaignDetailId).FirstOrDefault();
                            var product = _db.Products.Where(x => x.Id == item.productId).FirstOrDefault();

                            compainDetailDb.PromotionPrice = item.promotionPrice;
                            product.PromotionPrice = item.promotionPrice;
                            _db.SaveChanges();
                        }
                        scope.Commit();

                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(list);
                    }
                    catch (Exception e)
                    {
                        scope.Rollback();
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { notify = "Err", message = e.Message });
                    }
                }
            }
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpGet]
        public JsonResult RemoveProductInCampaignDetail([FromQuery(Name = "models")] string models)
        {
            var list = ParseListCampaginDetail(models);
            if (list == null || list.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(models);
            }
            else
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in list)
                        {
                            var compainDetailDb = _db.CampaignDetails.Where(x => x.CampaignDetailId == item.campaignDetailId).FirstOrDefault();
                            var productDb = _db.Products.Where(x => x.Id == item.productId).FirstOrDefault();

                            _db.Remove(compainDetailDb);
                            productDb.PromotionPrice = null;

                            _db.SaveChanges();
                        }
                        scope.Commit();

                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(list);
                    }
                    catch (Exception e)
                    {
                        scope.Rollback();
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { notify = "Err", message = e.Message });
                    }
                }
            }
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        public JsonResult InsertProductToExistCampaign(CampaignDetailViewModel _params)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    if (_db.CampaignDetails.Any(cd => cd.CampaignId == _params.campaignId && cd.ProductId == _params.productId))
                    {
                        throw new Exception(message: "Đã tồn tại sản phẩm trong Campaign!");
                    }

                    _db.CampaignDetails.Add(new CampaignDetails
                    {
                        ProductId = _params.productId,
                        PromotionPrice = _params.promotionPrice,
                        CampaignId = _params.campaignId
                    });

                    _db.SaveChanges();

                    var product = _db.Products.Where(x => x.Id == _params.productId).FirstOrDefault();
                    product.PromotionPrice = _params.promotionPrice;

                    _db.SaveChanges();

                    scope.Commit();

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Thêm sản phẩm vào Campaign thành công!" });
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = e.Message, err = e.Message });
                }
            }
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetCampaignInfo(int id)
        {
            var campaignDb = _db.Campaign.Where(x => x.CampaignId == id).FirstOrDefault();
            return Json(campaignDb);
        }

        private List<CampaignDetailViewModel> ParseListCampaginDetail(String jsonString)
        {
            return JsonConvert.DeserializeObject<List<CampaignDetailViewModel>>(jsonString);
        }
    }
}