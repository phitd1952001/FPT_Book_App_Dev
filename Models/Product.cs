using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT_Book_Khôi_Phi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        public String Author { get; set; }
        [Required]
        public String NoPage { get; set; }
        public String ImageUrl { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}