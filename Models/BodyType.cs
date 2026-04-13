using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // BodyType (Типы кузова)
    public class BodyType
    {
        [Key]
        public int BodyType_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<CarModels>? Models { get; set; }
    }
}
