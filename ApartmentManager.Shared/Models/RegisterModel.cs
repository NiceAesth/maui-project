using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Shared.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Parolă")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirmă Parola")]
    public string ConfirmPassword { get; set; }

    [Required] [Display(Name = "Nume")] public string Nume { get; set; }

    [Required] [Display(Name = "Prenume")] public string Prenume { get; set; }

    [Required]
    [Display(Name = "Apartament")]
    public int ApartamentID { get; set; }
}