using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FPT_Book_Khôi_Phi.Data;
using FPT_Book_Khôi_Phi.Models;
using FPT_Book_Khôi_Phi.Utility;
using FPT_Book_Khôi_Phi.ViewModel;
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
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }

        public OrderOverViewsController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public IActionResult Index(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim =claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;
            
            orderHeaderList = _db.OrderHeaders.Include(u => u.ApplicationUser).ToList();

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus==SD.StatusApproved ||
                                                                 o.OrderStatus==SD.StatusInProcess||
                                                                 o.OrderStatus==SD.StatusPending);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusCancelled ||
                                                                 o.OrderStatus == SD.StatusRefunded ||
                                                                 o.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return View(orderHeaderList);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM()
            {
                OrderHeader = _db.OrderHeaders.Where(u => u.Id == id).Include(u => u.ApplicationUser).FirstOrDefault(),
                OrderDetails = _db.OrderDetailss.Where(o => o.OrderHeaderId == id).Include(u => u.Product)

            };
            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {
            OrderHeader orderHeader = _db.OrderHeaders.Where(u => u.Id == OrderVM.OrderHeader.Id).Include(u=>u.ApplicationUser).FirstOrDefault();
            // if(stripeToken!=null)
            // {
            //     //process the payment
            //     var options = new ChargeCreateOptions
            //     {
            //         Amount = Convert.ToInt32(orderHeader.OrderTotal*100),
            //         Currency = "usd",
            //         Description = "Order ID : " + orderHeader.Id,
            //         Source = stripeToken
            //     };
            //
            //     var service = new ChargeService();
            //     Charge charge = service.Create(options);
            //
            //     if (charge.Id == null)
            //     {
            //         orderHeader.PaymentStatus = SD.PaymentStatusRejected;
            //     }
            //     else
            //     {
            //         orderHeader.TransactionId = charge.Id;
            //     }
            //     if (charge.Status.ToLower() == "succeeded")
            //     {
            //         orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            //        
            //         orderHeader.PaymentDate = DateTime.Now;
            //     }
            //
            //     _db.SaveChanges();
            //
            // }
            return RedirectToAction("Details", "OrderOverViews", new { id = orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_StoreOwner)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_StoreOwner)]
        public IActionResult ProcessOrder()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = SD.StatusInProcess;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_StoreOwner)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            // if (orderHeader.PaymentStatus == SD.StatusApproved)
            // {
            //     var options = new RefundCreateOptions
            //     {
            //         Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
            //         Reason = RefundReasons.RequestedByCustomer,
            //         Charge = orderHeader.TransactionId
            //
            //     };
            //     var service = new RefundService();
            //     Refund refund = service.Create(options);
            //
            //     orderHeader.OrderStatus = SD.StatusRefunded;
            //     orderHeader.PaymentStatus = SD.StatusRefunded;
            // }
            // else
            // {
            //     orderHeader.OrderStatus = SD.StatusCancelled;
            //     orderHeader.PaymentStatus = SD.StatusCancelled;
            // }
            //
            // _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult UpdateOrderDetails()
        {
            var orderHEaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHEaderFromDb.Address = OrderVM.OrderHeader.Address;
            
            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _db.SaveChanges();
            TempData["Error"] = "Order Details Updated Successfully.";
            return RedirectToAction("Details", "OrderOverViews", new { id = orderHEaderFromDb.Id });
        }
    }
}