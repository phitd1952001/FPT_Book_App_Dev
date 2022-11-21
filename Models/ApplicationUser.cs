using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FPT_Book_Khôi_Phi.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}