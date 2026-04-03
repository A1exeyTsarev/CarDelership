using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // Car (Автомобили)
    public class Cars
    {
        [Key]
        public int Car_Id { get; set; }

        [Required]
        public int model_Id { get; set; }

        [Range(0, 1000)]
        public int Quantity { get; set; }

        [Range(0, 10000000)]
        public decimal Price { get; set; }

        [Range(0, 10000000)]
        public decimal DiscountPrice { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? AvilabilityStatus { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        [Range(1900, 2026)]
        public int Year { get; set; }

        public int Color_Id { get; set; }

        [Range(0, 500000)]
        public int Mileage { get; set; }

        [StringLength(17)]
        public string? VIN { get; set; }

        // Внешние ключи
        [ForeignKey("model_Id")]
        public Model? Model { get; set; }

        // Навигационные свойства
        public ICollection<CarTags>? CarTags { get; set; }
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
        public ICollection<CarComments>? CarComments { get; set; }
        public ICollection<OrderItems>? OrderItems { get; set; }
    }
}
