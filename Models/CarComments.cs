using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    public class CarComments
    {
        [Key]
        public int CarComment_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Required]
        public int Author_Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        // Новое поле для модерации
        public bool IsApproved { get; set; } = false; // false - ожидает проверки, true - одобрен

        public DateTime? ApprovedAt { get; set; }

        [StringLength(100)]
        public string? ApprovedBy { get; set; }

        // Навигационные свойства
        [ForeignKey("Car_Id")]
        public virtual Cars? Car { get; set; }

        [ForeignKey("Author_Id")]
        public virtual Users? Author { get; set; }
    }
}