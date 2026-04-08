using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    public class Wishlist
    {
        [Key]
        public int Wishlist_Id { get; set; }

        [Required]
        public int User_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        public DateTime AddedAt { get; set; }

        // Navigation properties
        [ForeignKey("User_Id")]
        public virtual Users? User { get; set; }

        [ForeignKey("Car_Id")]
        public virtual Cars? Car { get; set; }
    }
}
