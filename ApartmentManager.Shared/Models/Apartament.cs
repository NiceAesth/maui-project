using System.ComponentModel.DataAnnotations;
using SQLite;

namespace ApartmentManager.Shared.Models;

public class Apartament
{
    [PrimaryKey] [AutoIncrement] public int ID { get; set; }


    [Display(Name = "Nr. Apartament")] public string? NumarApartament { get; set; }

    [Display(Name = "Etaj")] public int Etaj { get; set; }

    [Display(Name = "Suprafață (mp)")] public double Suprafata { get; set; }

    [Display(Name = "Nr. Persoane")] public int NumarPersoane { get; set; }

    public string DetaliiApartament => "Ap. " + NumarApartament + ", Etaj " + Etaj;
}