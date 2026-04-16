// Models/Supply.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDelership.Models
{
    public class Supply
    {
        [Key]
        public int Supply_Id { get; set; }

        [Required]
        public int Supplier_Id { get; set; }

        [Required]
        public int Status_Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

        [ForeignKey("Supplier_Id")]
        public virtual Suppliers? Supplier { get; set; }

        [ForeignKey("Status_Id")]
        public virtual StatusSupply? StatusSupply { get; set; }

        // Навигационное свойство для позиций поставки
        public virtual ICollection<SupplyItem>? SupplyItems { get; set; }
    }
}