using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // CarComment (Комментарии к авто)
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
        public string? CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        // Внешние ключи
        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }

        [ForeignKey("Author_Id")]
        public Users? Author { get; set; }
    }
}
