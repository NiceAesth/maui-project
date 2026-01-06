using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApartmentManager.Web.Pages.Locatari;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SelectList? ApartamentList { get; set; }

    [BindProperty] public Locatar Locatar { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var client = _httpClientFactory.CreateClient("API");

        var response = await client.GetAsync($"locatari/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var content = await response.Content.ReadAsStringAsync();
        Locatar = JsonSerializer.Deserialize<Locatar>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Locatar();

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
            await OnGetAsync(Locatar.ID);
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PutAsJsonAsync($"locatari/{Locatar.ID}", Locatar);

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");

        return Page();
    }
}