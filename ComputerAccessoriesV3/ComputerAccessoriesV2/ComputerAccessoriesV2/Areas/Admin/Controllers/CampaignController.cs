using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Data;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Policy = Policy.AdminModify)]
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
        public async Task<JsonResult> CreateNewCampaign(CampaignViewModel _params)
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

            var resultInsert = await _db.SaveChangesAsync();

            if (resultInsert <= 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Error when insert new Campaign" });

            }
            else
            {
                foreach (var item in _params.ListProduct)
                {
                    _db.CampaignDetails.Add(new CampaignDetails
                    {
                        CampaignId = newCampaign.CampaignId,
                        ProductId = item,
                    });
                }

                var resultInsertListProduct = await _db.SaveChangesAsync();
                if (resultInsertListProduct <= 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Error when insert list Product to CampaignDetail" });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { newCampaignId = newCampaign.CampaignId });
                }
            }
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult EditCampaign(int id)
        {
            ViewBag.CampaignId = id;
            return View();
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        public JsonResult EditCampaign(CampaignViewModel _params)
        {
            return Json(null);
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetAllCampaignType()
        {
            return Json(_db.CampaignType.ToList());
        }

        [HttpGet]
        public JsonResult GetAllProductForNewCampaign()
        {
            var now = DateTime.Now;
            var listAllProduct = _db.Products.ToList();
            var listCompaigningProduct =
                (from product in _db.Products
                 join campaignDetail in _db.CampaignDetails on product.Id equals campaignDetail.ProductId
                 join campaign in _db.Campaign on campaignDetail.CampaignId equals campaign.CampaignId 
                    where campaign.Status.Value == true && campaign.EndDate < now 
                 select product).ToList();
            var listAvailableForNewCompaign = listAllProduct.Except(listCompaigningProduct);
            return (Json(listAvailableForNewCompaign));
        }

        [HttpGet]
        public JsonResult GetAllProductInCampaign(int campaignId)
        {
            try
            {
                var listProduct = (from product in _db.Products
                                   join campaignDetail in _db.CampaignDetails on product.Id equals campaignDetail.ProductId
                                   join campaign in _db.Campaign on campaignId equals campaign.CampaignId
                                   select new
                                   {
                                       ProductId = product.Id,
                                       Id = campaignDetail.CampaignDetailId,
                                       productName = product.ProductName,
                                       originPrice = product.OriginalPrice,
                                       promotionPrice = campaignDetail.PromotionPrice
                                   }).ToList();

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(listProduct);
            } catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.ToString() });
            }
        }

        [HttpGet]
        public async Task<JsonResult> UpdateProductInCampaignDetail([FromQuery(Name ="models")] string models)
        {
            var list = ParseListCampaginDetail(models);
            if(list == null || list.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(models);
            } else
            {
                foreach (var item in list)
                {
                    var compainDetailDb = _db.CampaignDetails.Where(x => x.CampaignDetailId == item.id).FirstOrDefault();
                    var product = _db.Products.Where(x => x.Id == item.productId).FirstOrDefault();

                    compainDetailDb.PromotionPrice = item.promotionPrice;
                    product.PromotionPrice = item.promotionPrice;
                }

                var result = await _db.SaveChangesAsync();

                if(result == list.Count)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(list);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Err" });
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> RemoveProductInCampaignDetail([FromQuery(Name = "models")] string models)
        {
            var list = ParseListCampaginDetail(models);
            if (list == null || list.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(models);
            }
            else
            {
                foreach (var item in list)
                {
                    var compainDetailDb = _db.CampaignDetails.Where(x => x.CampaignDetailId == item.id).FirstOrDefault();
                    var productDb = _db.Products.Where(x => x.Id == item.id).FirstOrDefault();

                    _db.Remove(compainDetailDb);
                    productDb.PromotionPrice = null;
                }

                var result = await _db.SaveChangesAsync();

                if (result == list.Count*2)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(list);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Err" });
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> InsertProductToExitCampaign([FromQuery(Name = "models")] string models)
        {
            var list = ParseListCampaginDetail(models);
            if (list == null || list.Count == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(models);
            }
            else
            {
                foreach (var item in list)
                {
                    _db.CampaignDetails.Add(new CampaignDetails
                    {
                        ProductId = item.productId,
                        PromotionPrice = item.promotionPrice,
                        CampaignId = item.campaignId
                    });
                }

                int result = await _db.SaveChangesAsync();

                if (result == list.Count)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(list);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Err" });
                }
            }
        }

        private List<CampaignDetailViewModel> ParseListCampaginDetail (String jsonString)
        {
            return JsonConvert.DeserializeObject<List<CampaignDetailViewModel>>(jsonString);
        }
    }
}