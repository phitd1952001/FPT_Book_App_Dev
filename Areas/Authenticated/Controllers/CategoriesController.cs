using System;
using System.Linq;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT_Book_Khôi_Phi.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = SD.Role_StoreOwner)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index(string searchString)
        {
            var listCategories = _db.Categories.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                listCategories = listCategories.Where(c => c.Name.Contains(searchString)).ToList();
            }

            return View(listCategories);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            var findCategory = _db.Categories.Find(id);

            return View(findCategory);
        }

        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _db.Categories.Add(category);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                _db.Categories.Update(category);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var deleteId = _db.Categories.Find(id);
            _db.Categories.Remove(deleteId);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}