using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // Order (Заказы)
    public class Orders
    {
        [Key]
        public int Order_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? OrderNumber { get; set; }

        [Required]
        public int OrderStatus_Id { get; set; }

        [Required]
        public int OrderMethod_Id { get; set; }

        [Required]
        public int DeliveryMethod_Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        [Range(0, 10000000)]
        public decimal TotalAmount { get; set; }

        [Required]
        public int User_Id { get; set; }

        // Внешние ключи
        [ForeignKey("OrderStatus_Id")]
        public OrderStatuses? OrderStatus { get; set; }

        [ForeignKey("DeliveryMethod_Id")]
        public DeliveryMethods? DeliveryMethod { get; set; }

        [ForeignKey("User_Id")]
        public Users? User { get; set; }

        // Навигационное свойство
        public ICollection<OrderItems>? OrderItems { get; set; }
    }
}
