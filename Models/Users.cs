using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace CarDelership.Models
{
    // User (Пользователи)
    public class Users
    {
        [Key]
        public int User_Id { get; set; }

        [Required]
        public int Role_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Login { get; set; }

        [Required]
        [StringLength(255)]
        public string? Password { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime RegistrationDate { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [StringLength(50)]
        public string? PassportData { get; set; }

        // Внешние ключи
        [ForeignKey("Role_Id")]
        public Roles? Role { get; set; }

        // Навигационные свойства
        public ICollection<Orders>? Orders { get; set; }
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
        public ICollection<CarComments>? CarComments { get; set; }
    }
}
