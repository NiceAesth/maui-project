using System.Net.Http.Headers;
using System.Text.Json;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Facturi;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<FacturaIndividuala> Facturi { get; set; } = new List<FacturaIndividuala>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var endpoint = User.IsInRole("Admin") ? "facturi" : "facturi/my";
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Facturi = JsonSerializer.Deserialize<List<FacturaIndividuala>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<FacturaIndividuala>();
            }
        }
        catch (Exception)
        {
        }
    }
}