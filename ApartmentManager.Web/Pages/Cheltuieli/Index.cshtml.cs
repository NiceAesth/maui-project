using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Cheltuieli;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<CheltuialaAsociatie> Cheltuieli { get; set; } = new List<CheltuialaAsociatie>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        try
        {
            var response = await client.GetAsync("cheltuieli");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Cheltuieli =
                    JsonSerializer.Deserialize<List<CheltuialaAsociatie>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                    new List<CheltuialaAsociatie>();
            }
        }
        catch (Exception ex)
        {
        }
    }
}