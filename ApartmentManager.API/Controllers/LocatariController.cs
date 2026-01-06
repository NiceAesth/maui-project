using ApartmentManager.API.Data;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocatariController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LocatariController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Locatar>>> GetLocatari()
    {
        var users = await _userManager.GetUsersInRoleAsync("User");
        var locatari = users.Select(u => new Locatar
        {
            ID = u.Id,
            Nume = u.Nume,
            Prenume = u.Prenume,
            Email = u.Email,
            ApartamentID = u.ApartamentID
        }).ToList();

        return Ok(locatari);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Locatar>> GetLocatar(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null) return NotFound();

        return new Locatar
        {
            ID = user.Id,
            Nume = user.Nume,
            Prenume = user.Prenume,
            Email = user.Email,
            ApartamentID = user.ApartamentID
        };
    }

    [HttpPost]
    public async Task<ActionResult<Locatar>> PostLocatar(Locatar locatar)
    {
        var user = new ApplicationUser
        {
            UserName = locatar.Email,
            Email = locatar.Email,
            Nume = locatar.Nume,
            Prenume = locatar.Prenume,
            ApartamentID = locatar.ApartamentID
        };

        var result = await _userManager.CreateAsync(user, "Default123!");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return CreatedAtAction("GetLocatar", new { email = user.Email }, locatar);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLocatar(string id, Locatar locatar)
    {
        if (id != locatar.ID) return BadRequest();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Nume = locatar.Nume;
        user.Prenume = locatar.Prenume;
        user.ApartamentID = locatar.ApartamentID;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return NoContent();

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocatar(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return NoContent();
    }
}