using System.Net.Http.Headers;
using System.Security.Claims;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Sesizari;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public Sesizare Sesizare { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("API");

        var token = User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Sesizare.IdLocatar = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Sesizare.Status = "Nou";

        var response = await client.PostAsJsonAsync("sesizari", Sesizare);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Sesizarea a fost trimisă!";
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, "Eroare la trimiterea sesizării.");
        return Page();
    }
}