using ApartmentManager.API.Data;
using ApartmentManager.API.Hubs;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ApartamenteController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ApartamenteController(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Apartament>>> GetApartamente()
    {
        return await _context.Apartamente.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Apartament>> GetApartament(int id)
    {
        var apartament = await _context.Apartamente.FindAsync(id);

        if (apartament == null) return NotFound();

        return apartament;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutApartament(int id, Apartament apartament)
    {
        if (id != apartament.ID) return BadRequest();

        _context.Entry(apartament).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ApartamentExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Apartament>> PostApartament(Apartament apartament)
    {
        _context.Apartamente.Add(apartament);
        await _context.SaveChangesAsync();

        var dbNotif = new Notification
        {
            Title = "Apartament adăugat",
            Message = $"A fost adăugat apartamentul {apartament.NumarApartament}.",
            UserId = "",
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(dbNotif);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", dbNotif.Title, dbNotif.Message);

        return CreatedAtAction("GetApartament", new { id = apartament.ID }, apartament);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApartament(int id)
    {
        var apartament = await _context.Apartamente.FindAsync(id);
        if (apartament == null) return NotFound();

        _context.Apartamente.Remove(apartament);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ApartamentExists(int id)
    {
        return _context.Apartamente.Any(e => e.ID == id);
    }
}