using System.ComponentModel.DataAnnotations;

namespace FPT_Book_Khôi_Phi.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}