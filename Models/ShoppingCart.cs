using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // ShoppingCart (Корзина покупок)
    public class ShoppingCart
    {
        [Key]
        public int ShoppingCart_Id { get; set; }

        [Required]
        public int User_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; }

        // Внешние ключи
        [ForeignKey("User_Id")]
        public Users? User { get; set; }

        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }
    }
}
