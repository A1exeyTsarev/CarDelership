// Models/SupplyItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    public class SupplyItem
    {
        [Key]
        public int SupplyItem_Id { get; set; }

        [Required]
        public int Supply_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [ForeignKey("Supply_Id")]
        public virtual Supply? Supply { get; set; }

        [ForeignKey("Car_Id")]
        public virtual Cars? Car { get; set; }
    }
}