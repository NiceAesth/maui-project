using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Locatari;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<Locatar> Locatari { get; set; } = new List<Locatar>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        try
        {
            var response = await client.GetAsync("locatari");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Locatari = JsonSerializer.Deserialize<List<Locatar>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Locatar>();
            }
        }
        catch (Exception)
        {
        }
    }
}