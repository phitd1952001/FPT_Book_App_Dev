using System;
using System.Linq;
using System.Security.Claims;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;

namespace FPT_Book_Khôi_Phi.Areas.Authenticated
{
    [Area("Authenticated")]
    [Authorize]
    
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public CartController(ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // GET
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCarts = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).Include(x => x.Product)
            };
            ShoppingCartVM.OrderHeader.Total = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCarts)
            {
                ShoppingCartVM.OrderHeader.Total += (list.Price+ list.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int CartId)
        {
            var cart = _db.ShoppingCarts.Include(x=> x.Product).FirstOrDefault(x => x.Id == CartId);
            cart.Count += 1;
            cart.Price = cart.Product.Price * cart.Count;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int CartId)
        {
            var cart = _db.ShoppingCarts.Include(x=> x.Product).FirstOrDefault(x => x.Id == CartId);
            if (cart.Count == 1)
            {
                var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = cart.Product.Price * cart.Count;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int CartId)
        {
            var cart = _db.ShoppingCarts.Include(x=> x.Product).FirstOrDefault(x => x.Id == CartId);
            var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt -1);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCarts = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).Include(u => u.Product)
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCarts)
            {
                ShoppingCartVM.OrderHeader.Total += (list.Price + list.Count);
            }

            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVM.ListCarts = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value)
                .Include(u => u.Product);
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
            _db.SaveChanges();

            foreach (var item in ShoppingCartVM.ListCarts)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Quantity = item.Count
                };
                ShoppingCartVM.OrderHeader.Total += orderDetails.Quantity * orderDetails.Price;
                _db.OrderDetailss.Add(orderDetails);
            }
            
            _db.ShoppingCarts.RemoveRange(ShoppingCartVM.ListCarts);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);
            
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}