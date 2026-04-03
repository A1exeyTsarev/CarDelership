using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Country (Страны)
    public class Country
    {
        [Key]
        public int Country_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<Manufacturers>? Manufacturers { get; set; }
    }
}
