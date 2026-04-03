using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // EngineType (Типы двигателей)
    public class EngineType
    {
        [Key]
        public int EngineType_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<Model>? Models { get; set; }
    }
}
