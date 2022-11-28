using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FPT_Book_Khôi_Phi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPT_Book_Khôi_Phi.ViewModels
{
    public class UserVM
    {
        
        [Required]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> Rolelist { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}