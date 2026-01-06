using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ApartmentManager.Shared.Models;

public class FacturaIndividuala
{
    [JsonPropertyName("id")]
    [PrimaryKey] [AutoIncrement] public int ID { get; set; }

    [JsonPropertyName("idApartament")]
    [ForeignKey(typeof(Apartament))]
    [Display(Name = "Apartament")]
    public int IdApartament { get; set; }

    [JsonPropertyName("tip")]
    [Display(Name = "Tip")] public string Tip { get; set; } = string.Empty;

    [JsonPropertyName("numeApartament")]
    [Display(Name = "Nume Apartament")] public string NumeApartament { get; set; } = string.Empty;

    [JsonPropertyName("luna")]
    [Display(Name = "Lună")] public string Luna { get; set; } = string.Empty;

    [JsonPropertyName("sumaDePlata")]
    [Display(Name = "Sumă de Plată")] public decimal SumaDePlata { get; set; }

    [JsonPropertyName("status")]
    [Display(Name = "Status")] public string Status { get; set; } = "Neplătit";

    [Ignore] [JsonIgnore] public string DetaliiAfisare { get; set; } = string.Empty;

    [Ignore] [JsonIgnore] public bool EsteNeplatita => Status == "Neplătit";

    [Ignore] [JsonIgnore] public bool EsteAdmin { get; set; }
}