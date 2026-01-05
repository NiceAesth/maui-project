using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace PROIECT.Models
{
    public class Apartament
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

     
        public string? NumarApartament { get; set; }

        public int Etaj { get; set; }
        public double Suprafata { get; set; }

        public string DetaliiApartament
        {
            get { return "Ap. " + NumarApartament + ", Etaj " + Etaj; }
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
   
        public List<Locatar> Locatari { get; set; } = new List<Locatar>();
    }
}