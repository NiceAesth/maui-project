using SQLite;

namespace PROIECT.Models;

public class CheltuialaAsociatie
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Tip { get; set; } 
    public decimal SumaTotala { get; set; }
    public DateTime DataFactura { get; set; }
}