using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Suppliers (Поставщики)
    public class Suppliers
    {
        [Key]
        public int Supplier_Id { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        // Навигационное свойство: один поставщик - много поставок
        public ICollection<Supply>? Supplies { get; set; }
    }
}
