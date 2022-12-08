using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using FPT_Book_Khôi_Phi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FPT_Book_Khôi_Phi.Areas.UnAuthenticated.Controllers
{
    [Area("UnAuthenticated")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var productList = _db.Products.Include(p => p.Category).ToList();
            return View(productList);
        }
        
        
        public IActionResult Details(int id)
        {
            var productFromDb = _db.Products.Where(p => p.Id == id).Include(c => c.Category).First();
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(shoppingCart);
        }
        
        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity) User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.UserId = claim.Value;
                ShoppingCart cartFromDb = _db.ShoppingCarts.Where(
                        u => u.UserId == CartObject.UserId && u.ProductId == CartObject.ProductId).Include(u => u.Product).FirstOrDefault();
                if(cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _db.ShoppingCarts.Add(CartObject);
                }
                else
                {
                    cartFromDb.Count += CartObject.Count;
                    _db.ShoppingCarts.Update(cartFromDb);
                }

                _db.SaveChanges();
                // store to sesion
                var count = _db.ShoppingCarts.Where(c => c.UserId == CartObject.UserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _db.Products.FirstOrDefault(u => u.Id == CartObject.ProductId);
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(shoppingCart);
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}