using System.Collections.Generic;
using FPT_Book_Khôi_Phi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FPT_Book_Khôi_Phi.Models;

namespace FPT_Book_Khôi_Phi.ViewModel
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}