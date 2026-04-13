using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    public class News
    {
        [Key]
        public int News_Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }


        public bool IsActive { get; set; } = true; // Активна/в архиве

        public bool IsPromotion { get; set; } = false; // Акция или новость

        [StringLength(100)]
        public string PromotionCode { get; set; } // Промокод для акций

        public int? DiscountPercent { get; set; } // Скидка в процентах
    }
}
