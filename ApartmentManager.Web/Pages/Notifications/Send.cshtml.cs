using System.Text;
using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Notifications;

[Authorize(Roles = "Admin")]
public class SendModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SendModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public NotificationDto Notification { get; set; } = new();

    public List<Locatar> Locatari { get; set; } = new();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("locatari");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Locatari = JsonSerializer.Deserialize<List<Locatar>>(content,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new List<Locatar>();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(Notification);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("notifications/send", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Notificarea a fost trimisă!";
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError(string.Empty, "Eroare la trimiterea notificării.");
        return Page();
    }
}