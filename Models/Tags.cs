using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Tags (Теги для авто)
    public class Tags
    {
        [Key]
        public int Tag_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<CarTags>? CarTags { get; set; }
    }
}
