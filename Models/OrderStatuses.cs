using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // OrderStatuses (Статусы заказов)
    public class OrderStatuses
    {
        [Key]
        public int OrderStatus_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<Orders>? Orders { get; set; }
    }
}
