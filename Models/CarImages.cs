using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // CarImages (Изображения автомобилей)
    public class CarImages
    {
        [Key]
        public int Image_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? ImageName { get; set; }

        // Внешний ключ
        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }
    }
}
