using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT_Book_Khôi_Phi.Models
{
    public class OrderHeader
    {
        [Key] public int Id { get; set; }

        public int Total { get; set; }
        [Required] public string Address { get; set; }
        public DateTime OrderDate { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")] 
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}