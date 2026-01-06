using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApartmentManager.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SelectList? ApartamentList { get; set; }

    [BindProperty] public ApartmentManager.Shared.Models.RegisterModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
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
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");
        try
        {
            var response = await client.PostAsJsonAsync("auth/register", Input);

            if (response.IsSuccessStatusCode) return RedirectToPage("/Auth/Login");

            var content = await response.Content.ReadAsStringAsync();
            ErrorMessage = "Registration failed. " + content;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error connecting to server: " + ex.Message;
        }

        await OnGetAsync();
        return Page();
    }
}