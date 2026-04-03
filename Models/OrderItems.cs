using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // OrderItems (Товары в заказе)
    public class OrderItems
    {
        [Key]
        public int OrderItem_Id { get; set; }

        [Required]
        public int Order_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Range(0, 10000000)]
        public decimal PriceAtPurchase { get; set; }

        // Внешние ключи
        [ForeignKey("Order_Id")]
        public Orders? Order { get; set; }

        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }
    }
}
