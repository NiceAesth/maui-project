using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Apartments;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<Apartament> Apartamente { get; set; } = new();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("apartamente");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Apartamente =
                JsonSerializer.Deserialize<List<Apartament>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Apartament>();
        }
    }
}