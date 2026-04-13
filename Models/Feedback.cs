using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    public class Feedback
    {
        [Key]
        public int Feedback_Id { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Введите ваше имя")]
        [StringLength(100, ErrorMessage = "Имя не может быть длиннее 100 символов")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Выберите тему обращения")]
        [StringLength(200)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Введите сообщение")]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsProcessed { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [StringLength(100)]
        public string? ProcessedBy { get; set; }

        public string? AdminComment { get; set; }

        public bool IsClosed { get; set; }

        public bool IsClosedByUser { get; set; }

        [StringLength(50)]
        public string? IsReadByUser { get; set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }
    }
}