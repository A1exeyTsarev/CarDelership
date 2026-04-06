using System.ComponentModel.DataAnnotations;

namespace CarDelership.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        [StringLength(100, ErrorMessage = "ФИО не может быть длиннее 100 символов")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Паспортные данные")]
        public string? PassportData { get; set; }
    }
}