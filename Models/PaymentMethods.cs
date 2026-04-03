using System.ComponentModel.DataAnnotations;

namespace CarDelership.Models
{
    // PaymentMethods (Способы оплаты)
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethods_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
