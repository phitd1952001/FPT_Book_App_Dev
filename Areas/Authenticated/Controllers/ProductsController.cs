using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FPT_Book_Khôi_Phi.ViewModel;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPT_Book_Khôi_Phi.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = SD.Role_StoreOwner)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var listAllData = _db.Products.ToList();
            ViewData["Message"] = TempData["Message"];
            return View(listAllData);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVm = new ProductVM()
            {
                Product = new Product(),
                CategoryList = categoriesSelectListItems()
            };

            if (id == null)
            {
                return View(productVm);
            }

            productVm.Product = _db.Products.Find(id);

            return View(productVm);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images/products");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (productVm.Product.ImageUrl != null)
                    {
                        // to edit path so we need to delete the old path and update new one
                        var imagePath = Path.Combine(webRootPath, productVm.Product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var filesStreams =
                        new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }

                    productVm.Product.ImageUrl = @"/images/products/" + fileName + extension;
                }
                else
                {
                    //update without change the images
                    if (productVm.Product.Id != 0)
                    {
                        Product objFromDb = _db.Products.Find(productVm.Product.Id);
                        productVm.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (productVm.Product.Id == 0)
                {
                    _db.Products.Add(productVm.Product);
                    _db.SaveChanges();
                    TempData["Message"] = "Success: Add Successfully";
                    return RedirectToAction(nameof(Index));
                }

                var productDb = _db.Products.Find(productVm.Product.Id);
                productDb.Author = productVm.Product.Author;
                productDb.Title = productVm.Product.Title;
                productDb.Category = productVm.Product.Category;
                productDb.Description = productVm.Product.Description;
                productDb.Price = productVm.Product.Price;
                productDb.NoPage = productVm.Product.NoPage;
                productDb.ImageUrl = productVm.Product.ImageUrl;
                productDb.Quantity = productVm.Product.Quantity;
                
                _db.Products.Update(productDb);
                _db.SaveChanges();
                TempData["Message"] = "Success: Update Successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Message"] = "Error: Invalid Input, Please Recheck Again";
            productVm.CategoryList = categoriesSelectListItems();

            return View(productVm);
        }

        [NonAction]
        private IEnumerable<SelectListItem> categoriesSelectListItems()
        {
            var categories = _db.Categories.ToList();
            var result = categories.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return result;
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                ViewData["Message"] = "Error: Id input null";
            }

            var productNeedToDelete = _db.Products.Find(id);
            _db.Products.Remove(productNeedToDelete);
            _db.SaveChanges();
            TempData["Message"] = "Success: Delete Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}