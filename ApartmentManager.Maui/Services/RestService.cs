using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApartmentManager.Maui.Data;
using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui.Services;

public class RestService : IRestService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly string BaseUrl;

    public RestService()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
        _client = new HttpClient(handler);
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        BaseUrl = DeviceInfo.Platform == DevicePlatform.Android ?
            "http://10.0.2.2:5191/api/" :
            "http://localhost:5191/api/";
    }

    public async Task<string> LoginAsync(LoginModel model)
    {
        var uri = new Uri(BaseUrl + "auth/login");
        try
        {
            var json = JsonSerializer.Serialize(model, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var contentStr = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(contentStr);
                var token = result.RootElement.GetProperty("token").GetString() ?? "";
                if (!string.IsNullOrEmpty(token))
                {
                    await SecureStorage.SetAsync("auth_token", token);
                    await SetTokenAsync();

                    var locatarId = "";
                    if (result.RootElement.TryGetProperty("locatarId", out var locIdProp))
                        locatarId = locIdProp.GetString() ?? "";

                    var apartamentId = 0;
                    if (result.RootElement.TryGetProperty("apartamentId", out var apIdProp))
                        apartamentId = apIdProp.GetInt32();

                    var username = "";
                    if (result.RootElement.TryGetProperty("username", out var userProp))
                        username = userProp.GetString() ?? "";

                    var isAdmin = false;
                    if (result.RootElement.TryGetProperty("roles", out var rolesProp) &&
                        rolesProp.ValueKind == JsonValueKind.Array)
                        foreach (var role in rolesProp.EnumerateArray())
                            if (role.GetString() == "Admin")
                                isAdmin = true;

                    var email = "";
                    if (result.RootElement.TryGetProperty("userEmail", out var emailProp))
                        email = emailProp.GetString() ?? "";

                    Preferences.Set("LoggedLocatarId", locatarId);
                    Preferences.Set("LoggedApartamentId", apartamentId);
                    Preferences.Set("LoggedLocatarName", username);
                    Preferences.Set("LoggedUserEmail", email);
                    Preferences.Set("IsAdmin", isAdmin);
                }

                return token;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return string.Empty;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var uri = new Uri(BaseUrl + "auth/register");
        try
        {
            var json = JsonSerializer.Serialize(model, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return false;
    }

    public async Task<List<Apartament>> RefreshDataAsync()
    {
        await SetTokenAsync();
        var items = new List<Apartament>();
        var uri = new Uri(BaseUrl + "apartamente");
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                items = JsonSerializer.Deserialize<List<Apartament>>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return items ?? new List<Apartament>();
    }

    public async Task SaveApartamentAsync(Apartament item, bool isNewItem)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "apartamente");
        try
        {
            var json = JsonSerializer.Serialize(item, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (isNewItem)
                await _client.PostAsync(uri, content);
            else
                await _client.PutAsync(new Uri(BaseUrl + "apartamente/" + item.ID), content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteApartamentAsync(int id)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "apartamente/" + id);
        try
        {
            await _client.DeleteAsync(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task<List<FacturaIndividuala>> GetAllFacturiAsync()
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "facturi");
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<FacturaIndividuala>>(content, _serializerOptions) ??
                       new List<FacturaIndividuala>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return new List<FacturaIndividuala>();
    }

    public async Task<List<FacturaIndividuala>> RefreshFacturiAsync(int idApartament)
    {
        var all = await GetAllFacturiAsync();
        return all.Where(f => f.IdApartament == idApartament).ToList();
    }

    public async Task<List<Sesizare>> RefreshSesizariAsync()
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "sesizari");
        var items = new List<Sesizare>();
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                items = JsonSerializer.Deserialize<List<Sesizare>>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return items ?? new List<Sesizare>();
    }

    public async Task SaveSesizareAsync(Sesizare item, bool isNewItem)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "sesizari");
        try
        {
            var json = JsonSerializer.Serialize(item, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (isNewItem)
                await _client.PostAsync(uri, content);
            else
                await _client.PutAsync(new Uri(BaseUrl + "sesizari/" + item.ID), content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task SaveLocatarAsync(Locatar item, bool isNewItem)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "locatari");
        try
        {
            var json = JsonSerializer.Serialize(item, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (isNewItem)
                await _client.PostAsync(uri, content);
            else
                await _client.PutAsync(new Uri(BaseUrl + "locatari/" + item.ID), content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteLocatarAsync(string id)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "locatari/" + id);
        try
        {
            await _client.DeleteAsync(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task<List<Locatar>> GetAllLocatariAsync()
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "locatari");
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Locatar>>(content, _serializerOptions) ?? new List<Locatar>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return new List<Locatar>();
    }

    public async Task<Locatar?> GetLocatarAsync(string id)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "locatari/" + id);
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Locatar>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return null;
    }

    public async Task<List<Locatar>> GetLocatariByApartamentAsync(int apartamentId)
    {
        var all = await GetAllLocatariAsync();
        return all.Where(l => l.ApartamentID == apartamentId).ToList();
    }

    public async Task DeleteSesizareAsync(int id)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "sesizari/" + id);
        try
        {
            await _client.DeleteAsync(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task SaveFacturaAsync(FacturaIndividuala item, bool isNewItem)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "facturi");
        try
        {
            var json = JsonSerializer.Serialize(item, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (isNewItem)
                await _client.PostAsync(uri, content);
            else
                await _client.PutAsync(new Uri(BaseUrl + "facturi/" + item.ID), content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteFacturaAsync(int id)
    {
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "facturi/" + id);
        try
        {
            await _client.DeleteAsync(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        var items = new List<Notification>();
        await SetTokenAsync();
        var uri = new Uri(BaseUrl + "notifications");
        try
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                items = JsonSerializer.Deserialize<List<Notification>>(content, _serializerOptions) ??
                        new List<Notification>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return items;
    }


    private async Task SetTokenAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}