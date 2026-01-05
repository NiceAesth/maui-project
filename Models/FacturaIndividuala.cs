using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PROIECT.Models;

public class FacturaIndividuala
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    [ForeignKey(typeof(Apartament))]
    public int IdApartament { get; set; }
    public string Tip { get; set; }
    public string NumeApartament { get; set; }

    public string Luna { get; set; }
    public decimal SumaDePlata { get; set; }
    public string Status { get; set; }
    [Ignore]
    public string DetaliiAfisare { get; set; }

    [Ignore]
    public bool EsteNeplatita => Status == "Neplătit";
    public bool EsteAdmin { get; set; }
}
