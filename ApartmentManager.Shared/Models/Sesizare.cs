using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SQLite;

namespace ApartmentManager.Shared.Models;

public class Sesizare
{
    [JsonPropertyName("id")]
    [PrimaryKey] [AutoIncrement] public int ID { get; set; }

    [JsonPropertyName("idLocatar")]
    [Display(Name = "Locatar")] public string? IdLocatar { get; set; }

    [JsonPropertyName("subiect")]
    [Display(Name = "Subiect")] public string Subiect { get; set; } = string.Empty;

    [JsonPropertyName("descriere")]
    [Display(Name = "Descriere")] public string Descriere { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    [Display(Name = "Status")] public string Status { get; set; } = "Nou";


    [Ignore] [JsonIgnore] public string NumeAutor { get; set; } = string.Empty;

    [Ignore] [JsonIgnore] public bool EsteAdmin { get; set; }
}