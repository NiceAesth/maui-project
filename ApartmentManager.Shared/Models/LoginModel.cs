using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Shared.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Parolă")]
    public string Password { get; set; } = string.Empty;
}