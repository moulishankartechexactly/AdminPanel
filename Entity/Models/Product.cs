using System.ComponentModel.DataAnnotations;

namespace Entity.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Url]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
