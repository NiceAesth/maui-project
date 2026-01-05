using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PROIECT.Models;

public class Sesizare
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    [ForeignKey(typeof(Locatar))]
    public int IdLocatar { get; set; }

    public string Subiect { get; set; }
    public string Descriere { get; set; }
    public string Status { get; set; }

   

    [Ignore]
    public string NumeAutor { get; set; } 

    [Ignore]
    public bool EsteAdmin { get; set; }   
}