using System.Net.Http.Headers;
using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Sesizari;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<Sesizare> Sesizari { get; set; } = new List<Sesizare>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var endpoint = User.IsInRole("Admin") ? "sesizari" : "sesizari/my";
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Sesizari = JsonSerializer.Deserialize<List<Sesizare>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Sesizare>();

                foreach (var sesizare in Sesizari)
                {
                    if (!string.IsNullOrEmpty(sesizare.IdLocatar))
                    {
                        try
                        {
                            var userResponse = await client.GetAsync($"locatari/{sesizare.IdLocatar}");
                            if (userResponse.IsSuccessStatusCode)
                            {
                                var userContent = await userResponse.Content.ReadAsStringAsync();
                                var user = JsonSerializer.Deserialize<Locatar>(userContent,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                                if (user != null)
                                {
                                    sesizare.NumeAutor = $"{user.Prenume} {user.Nume} ({user.Email})";
                                }
                            }
                        }
                        catch
                        {
                            sesizare.NumeAutor = "Utilizator necunoscut";
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public async Task<IActionResult> OnPostPreiaAsync(int id)
    {
        return await UpdateStatusAsync(id, "In Procesare");
    }

    public async Task<IActionResult> OnPostRezolvaAsync(int id)
    {
        return await UpdateStatusAsync(id, "Rezolvat");
    }

    private async Task<IActionResult> UpdateStatusAsync(int id, string newStatus)
    {
        var client = _httpClientFactory.CreateClient("API");
        var token = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            TempData["Error"] = "Sesiune expirată sau token invalid. Vă rugăm să vă reautentificați.";
            return RedirectToPage("/Auth/Login");
        }
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await client.GetAsync($"sesizari/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sesizare = JsonSerializer.Deserialize<Sesizare>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (sesizare != null)
                {
                    sesizare.ID = id;
                    sesizare.Status = newStatus;
                    var putResponse = await client.PutAsJsonAsync($"sesizari/{id}", sesizare);
                    if (putResponse.IsSuccessStatusCode)
                    {
                        TempData["Message"] = $"Status actualizat: {newStatus}";
                    }
                    else
                    {
                        TempData["Error"] = "Eroare la actualizarea statusului în API.";
                    }
                }
                else
                {
                    TempData["Error"] = "Sesizarea nu a putut fi procesată.";
                }
            }
            else
            {
                TempData["Error"] = "Sesizarea nu a fost găsită.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Eroare neașteptată: {ex.Message}";
        }

        return RedirectToPage();
    }
}