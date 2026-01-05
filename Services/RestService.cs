using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PROIECT.Models;
using PROIECT.Data;

namespace PROIECT.Services
{
    public class RestService : IRestService
    {
        HttpClient client;
      
        string Url = "https://proiect-asociatie-api.azurewebsites.net/api/apartamente/";

        public RestService()
        {
            client = new HttpClient();
        }

        public async Task<List<Apartament>> RefreshDataAsync()
        {
            var items = new List<Apartament>();
            try
            {
                var response = await client.GetAsync(Url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<Apartament>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
            return items;
        }

        public async Task SaveApartamentAsync(Apartament item, bool isNewItem)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (isNewItem)
                    response = await client.PostAsync(Url, content);
                else
                    response = await client.PutAsync(Url + item.ID, content);
            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
        }

        public async Task DeleteApartamentAsync(int id)
        {
            try
            {
                await client.DeleteAsync(Url + id);
            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
        }

        public async Task<List<FacturaIndividuala>> RefreshFacturiAsync(int idApartament)
        {
            try
            {
                var response = await client.GetAsync(Url + "facturi/" + idApartament);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<FacturaIndividuala>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
            return new List<FacturaIndividuala>();
        }

  
        public async Task<List<Sesizare>> RefreshSesizariAsync()
        {
            var items = new List<Sesizare>();
            try
            {
          
                var response = await client.GetAsync(Url + "sesizari");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    items = JsonSerializer.Deserialize<List<Sesizare>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
            return items;
        }

        public async Task SaveSesizareAsync(Sesizare item, bool isNewItem)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (isNewItem)
                    await client.PostAsync(Url + "sesizari", content);


            }
            catch (Exception ex) { Console.WriteLine(@"\tERROR {0}", ex.Message); }
        }

        public async Task SaveLocatarAsync(Locatar item, bool isNewItem)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                if (isNewItem)
                {
                    response = await client.PostAsync(Url + "locatari", content);
                }
                else
                {
                    response = await client.PutAsync(Url + "locatari/" + item.ID, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Locatarul a fost sincronizat cu succes cu serverul.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROARE la sincronizarea locatarului: {0}", ex.Message);
            }
        }

        public async Task DeleteLocatarAsync(int id)
        {
            try
            {
                var response = await client.DeleteAsync(Url + "locatari/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Locatarul a fost șters de pe server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tEROARE la ștergerea locatarului: {0}", ex.Message);
            }
        }
    }
}