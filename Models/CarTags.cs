using Azure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // CarTags (Связь авто с тегами)
    public class CarTags
    {
        [Key]
        public int CarTag_Id { get; set; }

        [Required]
        public int Tag_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        // Внешние ключи
        [ForeignKey("Tag_Id")]
        public Tags? Tag { get; set; }

        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }
    }
}
