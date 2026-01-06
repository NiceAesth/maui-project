using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Cheltuieli;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public CheltuialaAsociatie Cheltuiala { get; set; } = new();

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("cheltuieli", Cheltuiala);

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");

        return Page();
    }
}