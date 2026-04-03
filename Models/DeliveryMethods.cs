using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // DeliveryMethods (Способы доставки)
    public class DeliveryMethods
    {
        [Key]
        public int DeliveryMethods_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        // Навигационное свойство
        public ICollection<Orders>? Orders { get; set; }
    }
}
