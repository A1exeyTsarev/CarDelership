using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Colors (Цвета автомобилей)
    public class Color
    {
        [Key]
        public int Color_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
