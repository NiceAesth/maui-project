using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Facturi;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DeleteModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public FacturaIndividuala Factura { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"facturi/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var content = await response.Content.ReadAsStringAsync();
        Factura = JsonSerializer.Deserialize<FacturaIndividuala>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new FacturaIndividuala();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.DeleteAsync($"facturi/{id}");

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");
        return Page();
    }
}