using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // Model (Модели авто)
    public class Model
    {
        [Key]
        public int Model_Id { get; set; }

        [Required]
        public int Manufacturer_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        public int BodyType_Id { get; set; }

        [Required]
        public int EngineType_Id { get; set; }

        [Range(1, 2000)]
        public int Power { get; set; }

        [Required]
        public int Transmission_Id { get; set; }

        // Внешние ключи
        [ForeignKey("Manufacturer_Id")]
        public Manufacturers? Manufacturer { get; set; }

        [ForeignKey("BodyType_Id")]
        public BodyType? BodyType { get; set; }

        [ForeignKey("EngineType_Id")]
        public EngineType? EngineType { get; set; }

        [ForeignKey("Transmission_Id")]
        public Transmission? Transmission { get; set; }

        // Навигационное свойство
        public ICollection<Cars>? Cars { get; set; }
    }
}
