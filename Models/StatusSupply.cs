using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // StatusSupply (Статусы поставщиков)
    public class StatusSupply
    {
        [Key]
        public int StatusSupply_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? StatusName { get; set; }

        // Навигационное свойство: один статус - много поставок
        public ICollection<Supply>? Supplies { get; set; }
    }
}
