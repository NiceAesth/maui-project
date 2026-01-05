using System.Threading.Tasks;
using System.Collections.Generic;
using PROIECT.Models;

namespace PROIECT.Data
{
    public interface IRestService
    {
        Task<List<Apartament>> RefreshDataAsync();
        Task SaveApartamentAsync(Apartament item, bool isNewItem);
        Task DeleteApartamentAsync(int id);

        Task<List<FacturaIndividuala>> RefreshFacturiAsync(int idApartament);

        
        Task<List<Sesizare>> RefreshSesizariAsync();

        Task SaveSesizareAsync(Sesizare sesizare, bool isNewItem);
        Task SaveLocatarAsync(Locatar item, bool isNewItem);
        Task DeleteLocatarAsync(int id);
    }
}