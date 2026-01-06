using System.ComponentModel.DataAnnotations;
using SQLite;

namespace ApartmentManager.Shared.Models;

public class CheltuialaAsociatie
{
    [PrimaryKey] [AutoIncrement] public int ID { get; set; }

    [Display(Name = "Tip")] public string Tip { get; set; }

    [Display(Name = "Sumă Totală")] public decimal SumaTotala { get; set; }

    [Display(Name = "Data Facturii")] public DateTime DataFactura { get; set; }
}