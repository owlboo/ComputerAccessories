using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessories.Models;
using Microsoft.AspNetCore.Mvc;
using ComputerAccessories.ViewModels;

namespace ComputerAccessories.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {

        public BrandController(ComputerAccessoriesContext db)
        {
            _db = db;
        }

        private readonly ComputerAccessoriesContext _db;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Brand()
        {
            ViewBag.controller = "Brand";
            var listBrand = _db.TblBrand.ToList();
            return View(listBrand);
        }

        public IActionResult EditBrand(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandFromDb = _db.TblBrand.Where(x => x.Id == id).FirstOrDefault();
            var brandViewModel = new BrandViewModel()
            {
                BrandName = brandFromDb.BrandName,
                Id = brandFromDb.Id,
                Logo = brandFromDb.Logo,
                Status = brandFromDb.Status == null ? false : brandFromDb.Status.Value
            };
            return View(brandViewModel);
        }

        public IActionResult CreateNewBrand()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBrand(BrandViewModel brand)
        {
            if (ModelState.IsValid)
            {
                String currentDir = Directory.GetCurrentDirectory();
                String webRootPath = Path.Combine(currentDir, "wwwroot");
                var file = HttpContext.Request.Form.Files;

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, @"image/BrandImage");
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    _db.TblBrand.Add(new TblBrand
                    {
                        BrandName = brand.BrandName,
                        CreatedDate = DateTime.Now,
                        Logo = Path.GetFileName(fileName),
                        Status = brand.Status
                    });
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Brand));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditBrand(BrandViewModel brand)
        {
            if (ModelState.IsValid)
            {
                String currentDir = Directory.GetCurrentDirectory();
                String webRootPath = Path.Combine(currentDir, "wwwroot");
                var file = HttpContext.Request.Form.Files;
                var currentBrandFromBd = _db.TblBrand.Where(x => x.Id == brand.Id).FirstOrDefault();

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, @"image/BrandImage");
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    currentBrandFromBd.Logo = Path.GetFileName(fileName);
                   
                }

                currentBrandFromBd.BrandName = brand.BrandName;
                currentBrandFromBd.ModifiedDate = DateTime.Now;
                currentBrandFromBd.Status = brand.Status;

                var result = await _db.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction(nameof(Brand));
                }
                return null;
            }
            else
            {
                return NotFound();
            }
        }

    }

}