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

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Subject { get; set; }

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