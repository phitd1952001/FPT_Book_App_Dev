using System.Linq;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPT_Book_Khôi_Phi.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = SD.Role_StoreOwner)]
    public class OrderOverViewsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderOverViewsController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        
        [HttpGet]
        public IActionResult Index()
        {
            var orderHeaderList = _db.OrderHeaders.Include(u => u.ApplicationUser).ToList();
            return View(orderHeaderList);
        }

        [HttpGet]
        public IActionResult OrderDetails(int orderHeaderId)
        {
            var orderDetails = _db.OrderDetailss.Where(o => o.OrderHeaderId == orderHeaderId).Include(p => p.Product)
                .ToList();
            return View(orderDetails);
        }
    }
}