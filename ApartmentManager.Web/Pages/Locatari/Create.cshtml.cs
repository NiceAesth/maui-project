using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApartmentManager.Web.Pages.Locatari;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SelectList? ApartamentList { get; set; }

    [BindProperty] public Locatar Locatar { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("apartaments");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var apartamente = JsonSerializer.Deserialize<List<Apartament>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ApartamentList = new SelectList(apartamente, "ID", "NumarApartament");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");


        var response = await client.PostAsJsonAsync("locatars", Locatar);

        if (response.IsSuccessStatusCode) return RedirectToPage("./Index");

        return Page();
    }
}