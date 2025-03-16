using System.ComponentModel.DataAnnotations;

namespace QuestRoomMVC.WebMVC.ViewModel;

public class RegisterViewModel
{
    [Required]
    [MaxLength(50)]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Рік народження")]
    public DateTime BirthYear { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    [Display(Name = "Підтвердження паролю")]
    public string PasswordConfirm { get; set; }
}