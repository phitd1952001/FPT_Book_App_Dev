using System.Collections.Generic;
using FPT_Book_Khôi_Phi.Models;

namespace FPT_Book_Khôi_Phi.ViewModel
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}