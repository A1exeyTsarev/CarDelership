using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // Manufacturer (Производители авто)
    public class Manufacturers
    {
        [Key]
        public int Manufacturer_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        public int Country_Id { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Внешние ключи
        [ForeignKey("Country_Id")]
        public Country? Country { get; set; }

        // Навигационное свойство
        public ICollection<CarModels>? Models { get; set; }
    }
}
