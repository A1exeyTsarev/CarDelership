using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CarDelership.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Логин может содержать только буквы, цифры и знак подчеркивания")]
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
        [StringLength(100, MinimumLength = 5, ErrorMessage = "ФИО должно быть от 5 до 100 символов")]
        [RegularExpression(@"^[а-яА-Яa-zA-Z\s\-]+$", ErrorMessage = "ФИО может содержать только буквы, пробелы и дефисы")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Телефон должен быть от 10 до 20 символов")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]+$", ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email должен быть от 5 до 100 символов")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(50, MinimumLength = 10, ErrorMessage = "Паспортные данные должны быть от 10 до 50 символов")]
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Паспортные данные должны содержать только цифры и пробелы")]
        [Display(Name = "Паспортные данные")]
        public string? PassportData { get; set; }
    }
}