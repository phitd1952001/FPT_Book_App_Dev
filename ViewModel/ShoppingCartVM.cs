using System.Collections;
using System.Collections.Generic;

namespace FPT_Book_Khôi_Phi.Models
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}