﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
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
        [BindProperty]
        public ProductViewModel ProductVM { get; set; }
        public ProductController(ComputerAccessoriesV2Context db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            ProductVM = new ProductViewModel
            {
                Product = new Products(),
                Brands = _db.Brand.ToList(),
                Categories = _db.Category.ToList(),
                ProductImages = new List<ProductImages>()
            };
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult ProductManagement()
        {
            var products = _db.Products.Include(x => x.Brand).Include(x => x.Category).ToList();
            ViewBag.controller = "Product";
            return View(products);
        }

        [Route("/[controller]/GetProduct")]
        public JsonResult GetProduct()
        {
            return Json(_db.Products.Select(x => new ProductGridModel
            {
                Id =x.Id,
                ProductName = x.ProductName,
                PromotionPrice=x.PromotionPrice ?? x.PromotionPrice.Value,
                BrandId =x.BrandId ?? x.BrandId.Value,
                BrandName = x.Brand.BrandName,
                CategoryId = x.CategoryId ?? x.CategoryId.Value,
                CategoryName= x.Category.CategoryName,
                OriginalPrice = x.OriginalPrice ?? x.OriginalPrice.Value,
                Origin = x.Origin,
                Color = x.Color,
                Code =x.Code,
                IsAvailable = x.IsAvailable,
                Quantity = x.Quantity ?? x.Quantity.Value,
                CreatedDate= x.CreatedDate ?? x.CreatedDate.Value,
                Thumnail =x.Thumnail
            }).ToList());
        }
        public IActionResult CreateNewProduct()
        {
            return View(ProductVM);
        }
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
                IsAvailable = model.Product.IsAvailable,
                Code = model.Product.Code
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
                    if (item.Name == "thumnail")
                    {
                        var fileName = product.Code + "-thumnail";
                        var extension = Path.GetExtension(item.FileName);
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail = @"/" + SD.ProductImages + @"/" + fileName + extension;
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
            productFromDb.IsAvailable = model.Product.IsAvailable;
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
                    if (item.Name == "thumnail")
                    {
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + fileExtension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail = @"/" + SD.ProductImages + @"/" + fileName + fileExtension;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction(nameof(ProductManagement));

        }
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

        //[HttpPost]
        //[Route("Product/UpdateMoreImages")]      
        //public async Task<IActionResult> UpdateMoreImages(int prodId)
        //{
        //    var productFromDb = _db.Products.Where(x => x.Id == prodId).FirstOrDefault();
        //    var images = _db.ProductImages.Where(x => x.ProductId == prodId).Select(x=>x.ImageUrl).ToList();
        //    //var files = Request.Form.Files;
            
        //    if (files.Count > 0)
        //    {
        //        int i = 1;
        //        int count = 0;
        //        var webRootPath = _hostEnvironment.WebRootPath;
        //        var pathImage = Path.Combine(webRootPath, SD.ProductImages);
                
        //        foreach (var item in files)
        //        {
        //            var fullPath = Path.Combine(pathImage, productFromDb.Code + "-thumnail" + i);
        //            while (checkContain(images, fullPath) == "")
        //            {
        //                i++;
        //            }
        //            using (var fileStream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                await item.CopyToAsync(fileStream);
        //            }
        //            ProductImages img = new ProductImages
        //            {
        //                ImageUrl = fullPath,
        //                ProductId = prodId,
        //            };
        //            _db.ProductImages.Add(img);
        //            if(await _db.SaveChangesAsync() > 0)
        //            {
        //                count++;
        //            }
        //        }
        //        if(count == files.Count)
        //        {
        //            return Json(new { code = 1, notice = $"Đã thêm {count} hình ảnh" });
        //        }
        //    }
        //    return Json(new { code = 0, notice="Cập nhật thất bại" });
        //}

        public string checkContain(List<string> strs , string key)
        {
            string result = "";
            foreach (var item in strs)
            {
                if (item.Contains(key))
                {
                    continue;
                }
                else
                {
                    result = key;
                }
            }
            return result;
        }
    }
}