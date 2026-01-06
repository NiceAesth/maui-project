using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public int TotalApartments { get; set; }
    public int TotalTenants { get; set; }
    public int TotalInvoices { get; set; }
    public int TotalRequests { get; set; }
    public int TotalExpenses { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");

        try
        {
            var aptResponse = await client.GetAsync("apartamente");
            if (aptResponse.IsSuccessStatusCode)
            {
                var content = await aptResponse.Content.ReadAsStringAsync();
                var apts = JsonSerializer.Deserialize<List<object>>(content);
                TotalApartments = apts?.Count ?? 0;
            }

            var locResponse = await client.GetAsync("locatari");
            if (locResponse.IsSuccessStatusCode)
            {
                var content = await locResponse.Content.ReadAsStringAsync();
                var locs = JsonSerializer.Deserialize<List<object>>(content);
                TotalTenants = locs?.Count ?? 0;
            }

            var facResponse = await client.GetAsync("facturi");
            if (facResponse.IsSuccessStatusCode)
            {
                var content = await facResponse.Content.ReadAsStringAsync();
                var facts = JsonSerializer.Deserialize<List<object>>(content);
                TotalInvoices = facts?.Count ?? 0;
            }

            var sesResponse = await client.GetAsync("sesizari");
            if (sesResponse.IsSuccessStatusCode)
            {
                var content = await sesResponse.Content.ReadAsStringAsync();
                var sess = JsonSerializer.Deserialize<List<object>>(content);
                TotalRequests = sess?.Count ?? 0;
            }

            var chelResponse = await client.GetAsync("cheltuieli");
            if (chelResponse.IsSuccessStatusCode)
            {
                var content = await chelResponse.Content.ReadAsStringAsync();
                var chels = JsonSerializer.Deserialize<List<object>>(content);
                TotalExpenses = chels?.Count ?? 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard data");
        }
    }
}