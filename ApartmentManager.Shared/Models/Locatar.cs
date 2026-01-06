using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Shared.Models;

public class Locatar
{
    [Display(Name = "ID")] public string? ID { get; set; }

    [Display(Name = "Nume")] public string? Nume { get; set; }

    [Display(Name = "Prenume")] public string? Prenume { get; set; }

    [Display(Name = "Email")] public string? Email { get; set; }

    [Display(Name = "ID Apartament")] public int ApartamentID { get; set; }
}