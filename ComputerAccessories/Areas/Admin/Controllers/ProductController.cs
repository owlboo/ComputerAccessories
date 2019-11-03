using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessories.Models;
using ComputerAccessories.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;

namespace ComputerAccessories.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ComputerAccessoriesContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public ProductViewModel ProductVM { get; set; }
        public ProductController(ComputerAccessoriesContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            ProductVM = new ProductViewModel
            {
                Product = new TblProduct(),
                Brands = _db.TblBrand.ToList(),
                Categories = _db.TblCategory.ToList(),
                ProductImages = new List<TblProductImages>()
            };
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult ProductManagement()
        {
            var products = _db.TblProduct.Include(x => x.Brand).Include(x => x.Category).ToList();
            ViewBag.controller = "Product";
            return View(products);
        }
        public IActionResult CreateNewProduct()
        {
            return View(ProductVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> CreateNewProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            TblProduct product = new TblProduct
            {
                BrandId = model.Product.BrandId,
                CategoryId = model.Product.CategoryId,
                ProductName = model.Product.ProductName,
                Price = model.Product.Price,
                Origin = model.Product.Origin,
                Color = model.Product.Color,
                PromotionPrice = model.Product.PromotionPrice,
                CreatedDate = DateTime.Now,
                ShortDescription = model.Product.ShortDescription,
                FullDescription = model.Product.FullDescription,
                Quantity = model.Product.Quantity,
                IsAvailable = model.Product.IsAvailable,
                Code = model.Product.Code
            };
            _db.TblProduct.Add(product);
            await _db.SaveChangesAsync();
            var filesUpload = Request.Form.Files;
            var productFromDb = _db.TblProduct.Where(x => x.Id == product.Id).SingleOrDefault();
            if (filesUpload.Count > 0)
            {
                var webrootPath = _hostEnvironment.WebRootPath;
                var pathImage = Path.Combine(webrootPath, SD.ConstantsHolder.ProductImages);
                int i = 1;
                foreach (var item in filesUpload)
                {
                    if(item.Name == "thumnail")
                    {
                        var fileName = product.Code+"-thumnail";
                        var extension = Path.GetExtension(item.FileName);
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension),FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail =  @"/"+SD.ConstantsHolder.ProductImages +@"/"+ fileName + extension;
                        await _db.SaveChangesAsync();
                    }
                    if(item.Name == "moreimages")
                    {
                        var fileName = product.Code + "-image" + i;
                        var extension = Path.GetExtension(item.FileName);
                        using(var fileStream = new FileStream(Path.Combine(pathImage, fileName + extension), FileMode.Create)){
                            await item.CopyToAsync(fileStream);
                        }
                        TblProductImages image = new TblProductImages()
                        {
                            ImageUrl = @"/"+SD.ConstantsHolder.ProductImages + @"/" + fileName + extension,
                            ProductId = productFromDb.Id,
                        };
                        _db.TblProductImages.Add(image);
                        await _db.SaveChangesAsync();
                        i++;
                    }
                                      
                }
            }
            return RedirectToAction(nameof(ProductManagement));
        }

        public IActionResult ProductDetails(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            ProductVM.Product = _db.TblProduct.Where(x => x.Id == id).Include(x => x.Brand).Include(x => x.Category).FirstOrDefault();
            ProductVM.ProductImages = _db.TblProductImages.Where(x => x.ProductId == id).ToList();
            return View(ProductVM);
        }
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductVM.Product = _db.TblProduct.Where(x => x.Id == id).Include(x => x.Brand).Include(x => x.Category).FirstOrDefault();
            ProductVM.ProductImages = _db.TblProductImages.Where(x => x.ProductId == id).ToList();
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
            var productFromDb = await _db.TblProduct.Where(x => x.Id == model.Product.Id).FirstOrDefaultAsync();
            if(productFromDb == null)
            {
                return NotFound();
            }
            productFromDb.ProductName = model.Product.ProductName;
            productFromDb.CategoryId = model.Product.CategoryId;
            productFromDb.BrandId = model.Product.BrandId;
            productFromDb.ShortDescription = model.Product.ShortDescription;
            productFromDb.FullDescription = model.Product.FullDescription;
            productFromDb.ModifiedDate = DateTime.Now;
            productFromDb.Origin = model.Product.Origin;
            productFromDb.Color = model.Product.Color;
            productFromDb.Code = model.Product.Code;
            productFromDb.IsAvailable = model.Product.IsAvailable;
            productFromDb.Price = model.Product.Price;
            productFromDb.PromotionPrice = model.Product.PromotionPrice;
            await _db.SaveChangesAsync();

            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var webrootPath = _hostEnvironment.WebRootPath;
                var pathImage = Path.Combine(webrootPath, SD.ConstantsHolder.ProductImages);
                foreach (var item in files)
                {
                    var fileName = Path.GetFileName(item.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    if(item.Name == "thumnail")
                    {
                        using (var fileStream = new FileStream(Path.Combine(pathImage, fileName + fileExtension), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        productFromDb.Thumnail = @"/" + SD.ConstantsHolder.ProductImages + @"/" + fileName + fileExtension;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction(nameof(ProductManagement));

        }
    }
}