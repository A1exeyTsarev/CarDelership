using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    // Supply (Поставки)
    public class Supply
    {
        [Key]
        public int Supply_Id { get; set; }

        [Required]
        public int Supplier_Id { get; set; }

        [Required]
        public int Car_Id { get; set; }

        [Required]
        public int Status_Id { get; set; }

        // Внешние ключи (FK)
        [ForeignKey("Supplier_Id")]
        public Suppliers? Supplier { get; set; }

        [ForeignKey("Car_Id")]
        public Cars? Car { get; set; }

        [ForeignKey("Status_Id")]
        public StatusSupply? StatusSupply { get; set; }
    }
}
