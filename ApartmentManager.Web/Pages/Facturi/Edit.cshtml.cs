using System.Net.Http.Headers;
using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApartmentManager.Web.Pages.Facturi;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SelectList ApartamentList { get; set; }

    [BindProperty] public FacturaIndividuala Factura { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var token = User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"facturi/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var content = await response.Content.ReadAsStringAsync();
        Factura = JsonSerializer.Deserialize<FacturaIndividuala>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (Factura != null)
        {
            Factura.ID = id.Value;
        }

        var aptResponse = await client.GetAsync("apartamente");
        if (aptResponse.IsSuccessStatusCode)
        {
            var aptContent = await aptResponse.Content.ReadAsStringAsync();
            var apartamente = JsonSerializer.Deserialize<List<Apartament>>(aptContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ApartamentList = new SelectList(apartamente, "ID", "NumarApartament");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(Factura.ID);
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");
        var token = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            TempData["Error"] = "Sesiune expirată sau token invalid. Vă rugăm să vă reautentificați.";
            return RedirectToPage("/Auth/Login");
        }
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync($"facturi/{Factura.ID}", Factura);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Factură actualizată cu succes.";
            return RedirectToPage("./Index");
        }

        TempData["Error"] = "Eroare la actualizarea facturii în API.";
        return Page();
    }
}