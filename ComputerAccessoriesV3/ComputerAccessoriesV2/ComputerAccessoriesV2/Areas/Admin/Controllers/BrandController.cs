using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerAccessoriesV2.Models;
using ComputerAccessoriesV2.Ultilities;
using ComputerAccessoriesV2.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ComputerAccessoriesV2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        public BrandController(ComputerAccessoriesV2Context db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEvironment = hostingEnvironment;
        }
        private readonly IHostingEnvironment _hostingEvironment;
        private readonly ComputerAccessoriesV2Context _db;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Brand()
        {
            ViewBag.controller = "Brand";
            var listBrand = _db.Brand.ToList();
            return View(listBrand);
        }

        [Route("/[controller]/GetBrand")]
        [HttpGet]
        public JsonResult GetBrand(int? status, string fromTime, string toTime,string text)
        {
            var from = new DateTime();
            var to = new DateTime();
            if (String.IsNullOrEmpty(fromTime) && string.IsNullOrEmpty(toTime)&&!status.HasValue)
            {

                if (!String.IsNullOrEmpty(text))
                {
                    return Json(_db.Brand.Where(x => x.BrandName.Contains(text)).Select(x => new Brand
                    {
                        Id = x.Id,
                        BrandName = x.BrandName,
                        Logo = x.Logo,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        Status = x.Status
                    }).ToList());
                }
                return Json(_db.Brand.Select(x => new Brand
                {
                    Id = x.Id,
                    BrandName = x.BrandName,
                    Logo = x.Logo,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Status = x.Status
                }).ToList());
            }
            if (status.HasValue)
            {
                bool stt = status.Value == 0 ? false : true;
                if (String.IsNullOrEmpty(toTime)&&String.IsNullOrEmpty(fromTime))
                {
                    
                    return Json(_db.Brand.Where(x=>x.Status == stt).Select(x => new Brand
                    {
                        Id = x.Id,
                        BrandName = x.BrandName,
                        Logo = x.Logo,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        Status = x.Status
                    }).ToList());
                }
                else
                {
                    if (String.IsNullOrEmpty(fromTime))
                    {
                        from = DateTime.Now.AddMonths(-1);
                    }
                    else
                    {
                        from = DateTime.Parse(fromTime);
                    }
                    if (String.IsNullOrEmpty(toTime))
                    {
                        to = DateTime.Now;
                    }
                    else
                    {
                        toTime += " 23:59:59";
                        to = DateTime.Parse(toTime);
                    }
                    return Json(_db.Brand.Where(x => x.CreatedDate >= from && x.CreatedDate <= to && x.Status == stt).Select(x => new Brand
                    {
                        Id = x.Id,
                        BrandName = x.BrandName,
                        Logo = x.Logo,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        Status = x.Status
                    }).ToList());
                }
                
            }
            else
            {
                if (String.IsNullOrEmpty(toTime))
                {
                    to = DateTime.Now;
                }
                else
                {
                    from = DateTime.Parse(fromTime);
                    toTime += " 23:59:59";
                    to = DateTime.Parse(toTime);
                }
                return Json(_db.Brand.Where(x => x.CreatedDate >= from && x.CreatedDate <= to).Select(x => new Brand
                {
                    Id = x.Id,
                    BrandName = x.BrandName,
                    Logo = x.Logo,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Status = x.Status
                }).ToList());
            }
            

        }
        public IActionResult EditBrand(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandFromDb = _db.Brand.Where(x => x.Id == id).FirstOrDefault();
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
                //String currentDir = Directory.GetCurrentDirectory();
                String webRootPath = _hostingEvironment.WebRootPath;
                
                var file = HttpContext.Request.Form.Files;

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, SD.BrandImages);
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName+extension);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    _db.Brand.Add(new Brand
                    {
                        BrandName = brand.BrandName,
                        CreatedDate = DateTime.Now,
                        Logo = @"/" + SD.BrandImages + @"/" + fileName,
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
                String webRootPath = _hostingEvironment.WebRootPath;

                var file = HttpContext.Request.Form.Files;
                var currentBrandFromBd = _db.Brand.Where(x => x.Id == brand.Id).FirstOrDefault();

                if (file.Count != 0)
                {
                    var upload = Path.Combine(webRootPath, SD.BrandImages);
                    var extension = Path.GetExtension(file[0].FileName);
                    var fileName = file[0].FileName;
                    var newFileDir = Path.Combine(upload, fileName);
                    using (var fileStream = new FileStream(newFileDir, FileMode.Create))
                    {
                        await file[0].CopyToAsync(fileStream);
                    }

                    currentBrandFromBd.Logo = @"/" + SD.BrandImages + @"/" + fileName;

                }

                currentBrandFromBd.BrandName = brand.BrandName;
                currentBrandFromBd.ModifiedDate = DateTime.Now;
                currentBrandFromBd.Status = brand.Status;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Brand));
            }
            else
            {
                return NotFound();
            }
        }
    }
}