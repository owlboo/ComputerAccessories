using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ComputerAccessoriesV2.DI;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ComputerAccessoriesV2Context _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRedis _cache;

        [BindProperty]
        public ProductViewModel ProductVM { get; set; }
        public ProductController(ComputerAccessoriesV2Context db, IWebHostEnvironment hostEnvironment, IRedis cache)
        {
            _db = db;
            _cache = cache;
            ProductVM = new ProductViewModel
            {
                Product = new Products(),
                Brands = _db.Brand.ToList(),
                Categories = _db.Category.ToList(),
                ProductImages = new List<ProductImages>()
            };
            _hostEnvironment = hostEnvironment;
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult ProductManagement()
        {
            var products = _db.Products.Include(x => x.Brand).Include(x => x.Category).ToList();
            ViewBag.controller = "Product";
            return View(products);
        }


        [Authorize(Policy = Policy.AdminAccess)]
        [Route("/[controller]/GetProduct")]
        public JsonResult GetProduct(int?CategoryId,int?BrandId,string FromTime,string ToTime)
        {
            var from = new DateTime();
           
            var to = new DateTime();

            var predicate = PredicateBuilder.True<Products>();

            var query = _db.Products.Include("Brand").Include("Category").AsNoTracking().AsQueryable();
            if (CategoryId.HasValue)
            {
                predicate = predicate.And(x => x.CategoryId == CategoryId.Value);
            }
            if (BrandId.HasValue)
            {
                //query.Where(x => x.BrandId == BrandId.Value);
                predicate = predicate.And(x => x.BrandId == BrandId.Value);
            }

            if (!String.IsNullOrEmpty(FromTime))
            {
                from = DateTime.Parse(FromTime);
                if (from.Year < 2018)
                    from = new DateTime(2019, 11, 1);
                
            }


            if (!String.IsNullOrEmpty(ToTime))
            {
                to = DateTime.Parse(ToTime);
                if (to < from)
                {
                    //query.Where(x => x.CreatedDate >= to && x.CreatedDate <= from);
                    predicate.And(x => x.CreatedDate >= to && x.CreatedDate <= from);
                }
                else
                {
                    //query.Where(x => x.CreatedDate >= from && x.CreatedDate <= to);
                    predicate.And(x => x.CreatedDate >= from && x.CreatedDate <= to);
                }
            }
            return Json(query.Where(predicate).Select(x => new ProductGridModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                PromotionPrice = x.PromotionPrice != null ? x.PromotionPrice.Value.ToString("###.###") : "",
                BrandId = x.BrandId ?? x.BrandId.Value,
                BrandName = x.Brand.BrandName,
                CategoryId = x.CategoryId ?? x.CategoryId.Value,
                CategoryName = x.Category.CategoryName,
                OriginalPrice = x.OriginalPrice != null ? x.OriginalPrice.Value.ToString("###,###") : "",
                Origin = x.Origin,
                Color = x.Color,
                Code = x.Code,
                Status = x.Status,
                Quantity = x.Quantity ?? x.Quantity.Value,
                CreatedDate = x.CreatedDate ?? x.CreatedDate.Value,
                Thumnail = x.Thumnail
            }).ToList());
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult CreateNewProduct()
        {
            return View(ProductVM);
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            Products product = new Products
            {
                BrandId = model.Product.BrandId,
                CategoryId = model.Product.CategoryId,
                ProductName = model.Product.ProductName,
                OriginalPrice = model.Product.OriginalPrice,
                Origin = model.Product.Origin,
                Color = model.Product.Color,
                PromotionPrice = model.Product.PromotionPrice,
                CreatedDate = DateTime.Now,
                ShorDescription = model.Product.ShorDescription,
                FullDescription = model.Product.FullDescription,
                Quantity = model.Product.Quantity,
                Status = 0,
                Code = model.Product.Code,
                IsNew = true,
                ViewCounts = 0
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            var filesUpload = Request.Form.Files;
            var productFromDb = _db.Products.Where(x => x.Id == product.Id).SingleOrDefault();
            if (filesUpload.Count > 0)
            {
                var webrootPath = _hostEnvironment.WebRootPath;
                var pathImage = Path.Combine(webrootPath, SD.ProductImages);
                int i = 1;
                foreach (var item in filesUpload)
                {
                    if (item.Name == "thumnail1")
                    {
                        var fileName = product.Code + "-thumnail1";
                        var extension = Path.GetExtension(item.FileName);
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail = @"/" + SD.ProductImages + @"/" + fileName + extension;
                        await _db.SaveChangesAsync();
                    }
                    if (item.Name == "thumnail2")
                    {
                        var fileName = product.Code + "-thumnail2";
                        var extension = Path.GetExtension(item.FileName);
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail2 = @"/" + SD.ProductImages + @"/" + fileName + extension;
                        await _db.SaveChangesAsync();
                    }
                    if (item.Name == "moreimages")
                    {
                        var fileName = product.Code + "-image" + i;
                        var extension = Path.GetExtension(item.FileName);
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        ProductImages image = new ProductImages()
                        {
                            ImageUrl = @"/" + SD.ProductImages + @"/" + fileName + extension,
                            ProductId = productFromDb.Id,
                        };
                        _db.ProductImages.Add(image);
                        await _db.SaveChangesAsync();
                        i++;
                    }

                }
            }
            return RedirectToAction(nameof(ProductManagement));
        }

        [Authorize(Policy = Policy.AdminAccess)]
        public IActionResult ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductVM.Product = _db.Products.Where(x => x.Id == id).Include(x => x.Brand).Include(x => x.Category).FirstOrDefault();
            ProductVM.ProductImages = _db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(ProductVM);
        }

        [Authorize(Policy = Policy.AdminModify)]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductVM.Product = _db.Products.Where(x => x.Id == id).Include(x => x.Brand).Include(x => x.Category).FirstOrDefault();
            ProductVM.ProductImages = _db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(ProductVM);
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var productFromDb = await _db.Products.Where(x => x.Id == model.Product.Id).FirstOrDefaultAsync();
            if (productFromDb == null)
            {
                return NotFound();
            }
            productFromDb.ProductName = model.Product.ProductName;
            productFromDb.CategoryId = model.Product.CategoryId;
            productFromDb.BrandId = model.Product.BrandId;
            productFromDb.ShorDescription = model.Product.ShorDescription;
            productFromDb.FullDescription = model.Product.FullDescription;
            productFromDb.ModifiedDate = DateTime.Now;
            productFromDb.Origin = model.Product.Origin;
            productFromDb.Color = model.Product.Color;
            productFromDb.Code = model.Product.Code;
            //productFromDb.IsAvailable = model.Product.IsAvailable;
            productFromDb.OriginalPrice = model.Product.OriginalPrice;
            productFromDb.PromotionPrice = model.Product.PromotionPrice;
            await _db.SaveChangesAsync();

            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var webrootPath = _hostEnvironment.WebRootPath;
                var pathImage = Path.Combine(webrootPath, SD.ProductImages);
                foreach (var item in files)
                {
                    var fileName = Path.GetFileName(item.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    if (item.Name == "thumnail1")
                    {
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + fileExtension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail = @"/" + SD.ProductImages + @"/" + fileName + fileExtension;
                        await _db.SaveChangesAsync();
                    }
                    if (item.Name == "thumnail2")
                    {
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + fileExtension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail2 = @"/" + SD.ProductImages + @"/" + fileName + fileExtension;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction(nameof(ProductManagement));

        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        [Route("Product/DeleteImage")]
        public async Task<IActionResult> DeleteImage([FromForm]int _imgId)
        {
            var imageFromDb = _db.ProductImages.Where(x => x.Id.Equals(_imgId)).FirstOrDefault();
            if (imageFromDb == null)
                return NotFound();
            _db.ProductImages.Remove(imageFromDb);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                return Json(new { code = 1, id = _imgId });
            }
            else
            {
                return Json(new { code = 0 });
            }
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        //[Route("Product/UpdateMoreImages")]
        public async Task<IActionResult> UploadMoreImages(int prodId)
        {
            var productFromDb = _db.Products.Where(x => x.Id == prodId).FirstOrDefault();
            var images = _db.ProductImages.Where(x => x.ProductId == prodId).Select(x => x.ImageUrl).ToList();
            var files = Request.Form.Files;

            if (files.Count > 0)
            {
                int i = 1;
                int count = 0;
                var webRootPath = _hostEnvironment.WebRootPath;
                var pathImage = Path.Combine(webRootPath, SD.ProductImages);
                
                    foreach (var item in files)
                {
                    
                    var fileName = Path.GetFileName(item.FileName);
                    var extension = Path.GetExtension(fileName);
                    
                        if (images.Count != 0)
                        {
                            while (checkContain(images, @"/"+SD.ProductImages +@"/"+ productFromDb.Code + "-image" + i + extension) == true)
                            {
                                i++;
                            }
                        }
                    var fullPath = Path.Combine(pathImage, productFromDb.Code + "-image" + i + extension);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            
                            await item.CopyToAsync(fileStream);
                        }
                        ProductImages img = new ProductImages
                        {
                            ImageUrl = @"/"+SD.ProductImages+@"/"+ productFromDb.Code + "-image"+i+extension,
                            ProductId = prodId,
                        };
                            i++;
                            _db.ProductImages.Add(img);
                                if (await _db.SaveChangesAsync() > 0)
                                {
                                    count++;
                                }
                    
                    
                }
                if (count == files.Count)
                {
                    return RedirectToAction(nameof(EditProduct), new {id = prodId });
                }
            }
            return RedirectToAction(nameof(EditProduct), new { id = prodId }); ;
        }

        [Authorize(Policy = Policy.AdminModify)]
        public ActionResult UpdateAttribute(int id, int categoryId)
        {
            var listAttributes = _db.Attributes.Where(x => x.CategoryId == categoryId).ToList();
            var listProductAttributes = _db.ProductAttribute.Where(x => x.ProductId == id).ToList();
            UpdateAttributeViewModel model = new UpdateAttributeViewModel()
            {
                CategoryId = categoryId,
                CategoryName = _db.Category.Where(z => z.Id == categoryId).Select(z => z.CategoryName).FirstOrDefault(),
                ProductId = id,
                ProductName = _db.Products.Where(z => z.Id == id).Select(z => z.ProductName).FirstOrDefault(),
                Attributes = listAttributes,
                ProductAttributes = listProductAttributes
            };

            return View(model);
        }

        [Authorize(Policy = Policy.AdminModify)]
        [HttpPost]
        [Route("/[controller]/SaveAttributesProduct")]
        public async Task<JsonResult> SaveAttributesProduct(AttrsStoredProductViewModel model)
        {
            string returnUrl = "/";
            try
            {
                bool isExistProductAttributeUpdate = _db.ProductAttribute.Where(x => x.ProductId == model.ProductId).ToList().Count > 0 ? true : false;

                if (!isExistProductAttributeUpdate)
                {
                    int i = 0;
                    foreach (var item in model.ListAttrs)
                    {
                        _db.ProductAttribute.Add(new ProductAttribute
                        {
                            ProductId = model.ProductId,
                            AttributeId = item.Id,
                            Value = item.Value
                        });
                        await _db.SaveChangesAsync();
                        i++;
                    }

                    if (i <= model.ListAttrs.Count && i >= 1)
                    {
                        //var listProductUpdate = _db.Products.Where(x => x.Id == model.ProductId).ToList();
                        //foreach (var item in listProductUpdate)
                        //{
                        //    item.Status = 1;
                        //}
                        //_db.Products.UpdateRange(listProductUpdate);

                        var productFromdb = _db.Products.Where(x => x.Id == model.ProductId).FirstOrDefault();
                        productFromdb.Status = 1;
                        await _db.SaveChangesAsync();
                        returnUrl = "/Admin/Product/ProductManagement";
                        return Json(new { code = 1, count = i, url = returnUrl });
                    }
                    if (i == 0)
                    {
                        return Json(new { code = 0, err = "Có lỗi xảy ra khi thêm giá trị", url = returnUrl });
                    }
                }
                else
                {
                    //List<ProductAttribute> newValueAtribute = new List<ProductAttribute>();
                    foreach (var item in model.ListAttrs)
                    {
                        var productAttribute = _db.ProductAttribute.Where(x => x.AttributeId == item.Id).FirstOrDefault();
                        productAttribute.Value = item.Value;
                        await _db.SaveChangesAsync();
                    }
                    var productFromdb = _db.Products.Where(x => x.Id == model.ProductId).FirstOrDefault();
                    if (productFromdb.Status < 1)
                    {
                        productFromdb.Status = 1;
                    }
                    await _db.SaveChangesAsync();
                    returnUrl = "/Admin/Product/ProductManagement";
                    return Json(new { code = 1, count = model.ListAttrs.Count, url = returnUrl });
                }

            }
            catch (Exception e)
            {

                throw;
            }
            returnUrl = "/Admin/Product/ProductManagement";
            return Json(new { code = 1, url = returnUrl });
        }

        [Authorize(Policy = Policy.AdminAccess)]
        [HttpGet]
        public JsonResult GetProductOriginPrice(int id)
        {
            var product = _db.Products.Where(x => x.Id == id).FirstOrDefault();
            if(product != null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { originPrice = product.OriginalPrice });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Error when load product id ="+ id });
            }
        }

        public bool checkContain(List<string> strs, string key)
        {
            string result = "";
            foreach (var item in strs)
            {
                if (item.Contains(key))
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("/[controller]/GetProductReviews")]
        public JsonResult GetProductReviews(int id)
        {
            var listReview = (
                from r in _db.Reviews
                let user = _db.AspNetUsers.Where(u => u.Id == r.UserId).FirstOrDefault()
                where r.ProductId == id
                select new
                {
                    r.GuestName,
                    r.CreatedDate,
                    r.Description,
                    r.LikedNumber,
                    r.Star,
                    r.ReviewId,
                    r.UserId,
                    user.DisplayName
                }
                ).ToList();
            return Json(listReview);
        }

        [HttpPost]
        [Route("/[controller]/LikeReview")]
        public async Task<JsonResult> LikeReview(int reviewId)
        {
            var review = _db.Reviews.Where(x => x.ReviewId == reviewId).FirstOrDefault();
            if(review == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { notify = "Không tìm thấy Review!" });
            }
            else
            {
                review.LikedNumber += 1;
                await _db.SaveChangesAsync();

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { notify = "Like thành công ^-^ !!" });
            }
        }

        [HttpPost]
        [Route("/[controller]/UploadPreview")]
        public async Task<JsonResult> UploadPreview([FromBody]ReviewViewModel _params)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    var newReview = new Reviews
                    {
                        CreatedDate = DateTime.Now,
                        Description = _params.Description,
                        UserId = _params.UserId,
                        GuestName = _params.GuestName,
                        LikedNumber = 0,
                        ProductId = _params.ProductId,
                        Star = _params.Star
                    };
                    _db.Reviews.Add(newReview);

                    await _db.SaveChangesAsync();
                    await scope.CommitAsync();



                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { notify = "Gửi đánh giá thành công, cảm ơn bạn đã nhận xét về sản phẩm!" });
                } catch(Exception e)
                {
                    await scope.RollbackAsync();

                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { notify = "Gửi đánh giá thất bại, vui lòng thử lại sau!", message = e.Message });
                }
            }
        }

        [HttpGet]
        [Route("/[controller]/SearchProduct")]
        public JsonResult SearchProduct(string productName)
        {
            return Json(_db.Products
                .Where(x => x.ProductName.Contains(productName))
                .Select(x => new {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Price = x.PromotionPrice == null ? x.OriginalPrice : x.PromotionPrice,
                    PromotionPrice = x.PromotionPrice,
                    Thumbnail = x.Thumnail,
                }));
        }
    }
}