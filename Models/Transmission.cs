using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Transmission (Коробки передач)
    public class Transmission
    {
        [Key]
        public int Transmission_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<Model>? Models { get; set; }
    }
}
