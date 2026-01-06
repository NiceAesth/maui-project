using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Locatari;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DeleteModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

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

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.DeleteAsync($"locatari/{id}");

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");
        return Page();
    }
}