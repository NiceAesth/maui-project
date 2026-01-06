using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApartmentManager.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public ApartmentManager.Shared.Models.LoginModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();

        returnUrl ??= Url.Content("~/");

        var client = _httpClientFactory.CreateClient("API");
        try
        {
            var response = await client.PostAsJsonAsync("auth/login", Input);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonNode.Parse(content);

                var token = json?["token"]?.ToString();
                var username = json?["username"]?.ToString();
                var locatarId = json?["locatarId"]?.ToString();
                var apartamentId = json?["apartamentId"]?.ToString();
                var roles = json?["roles"]?.AsArray().Select(r => r?.ToString() ?? "").ToList();

                if (!string.IsNullOrEmpty(token))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, username ?? Input.Email),
                        new(ClaimTypes.NameIdentifier, locatarId ?? ""),
                        new("access_token", token),
                        new("ApartamentID", apartamentId ?? "0")
                    };

                    if (roles != null)
                        foreach (var role in roles)
                            claims.Add(new Claim(ClaimTypes.Role, role));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(3)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return LocalRedirect(returnUrl);
                }
            }

            ErrorMessage = "Invalid login attempt.";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error connecting to server: " + ex.Message;
        }

        return Page();
    }
}