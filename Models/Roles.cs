using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // Roles (Роли пользователей)
    public class Roles
    {
        [Key]
        public int Role_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        // Навигационное свойство
        public ICollection<Users>? Users { get; set; }
    }
}
