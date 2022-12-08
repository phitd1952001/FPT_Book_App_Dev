using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT_Book_Khôi_Phi.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        
        public int Count { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser{ get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [NotMapped] 
        public int Price { get; set; }
    }
}