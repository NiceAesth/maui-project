using System.Collections.Generic;
using System.Threading.Tasks;
using PROIECT.Models;

namespace PROIECT.Data
{
    public class AsociatieDatabase
    {
        IRestService restService;

        public AsociatieDatabase(IRestService service)
        {
            restService = service;
        }

       
        public Task<List<FacturaIndividuala>> GetFacturiAsync(int id)
        {
            return restService.RefreshFacturiAsync(id);
        }

        
        public Task SaveSesizareAsync(Sesizare item, bool isNew = true)
        {
            return restService.SaveSesizareAsync(item, isNew);
        }

      
        public Task<List<Sesizare>> GetSesizariAsync()
        {
            
            return restService.RefreshSesizariAsync();
        }
    }
}