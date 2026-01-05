using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PROIECT.Models
{
    public class Locatar
    {
        [PrimaryKey, AutoIncrement]
        public int ID  { get; set; }
        public string ? Nume { get; set; }
        public string ? Prenume { get; set; }
        public string ? Email { get; set; }

        [ForeignKey(typeof(Apartament))]
        public int ApartamentID { get; set; }
        public string Parola { get; set; }
        public bool IsAdmin { get; set; }
    }
}
