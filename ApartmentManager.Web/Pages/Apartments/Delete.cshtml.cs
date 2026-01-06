using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Apartments;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DeleteModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public Apartament Apartament { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"apartamente/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var content = await response.Content.ReadAsStringAsync();
        Apartament =
            JsonSerializer.Deserialize<Apartament>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Apartament();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.DeleteAsync($"apartamente/{id}");

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");
        return Page();
    }
}