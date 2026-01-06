using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui.Data;

public interface IRestService
{
    Task<List<Apartament>> RefreshDataAsync();
    Task SaveApartamentAsync(Apartament item, bool isNewItem);
    Task DeleteApartamentAsync(int id);

    Task<List<FacturaIndividuala>> RefreshFacturiAsync(int idApartament);


    Task<List<Sesizare>> RefreshSesizariAsync();

    Task SaveSesizareAsync(Sesizare sesizare, bool isNewItem);
    Task SaveLocatarAsync(Locatar item, bool isNewItem);
    Task DeleteLocatarAsync(string id);
    Task<string> LoginAsync(LoginModel model);
    Task<bool> RegisterAsync(RegisterModel model);

    Task<List<FacturaIndividuala>> GetAllFacturiAsync();
    Task<List<Locatar>> GetLocatariByApartamentAsync(int apartamentId);
    Task<List<Locatar>> GetAllLocatariAsync();
    Task<Locatar?> GetLocatarAsync(string id);
    Task DeleteSesizareAsync(int id);
    Task<List<Notification>> GetNotificationsAsync();
    Task SaveFacturaAsync(FacturaIndividuala item, bool isNewItem);
    Task DeleteFacturaAsync(int id);
}