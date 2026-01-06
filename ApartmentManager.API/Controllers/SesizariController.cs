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
public class SesizariController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public SesizariController(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sesizare>>> GetSesizari()
    {
        return await _context.Sesizari.ToListAsync();
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<Sesizare>>> GetMySesizari()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await _context.Sesizari
            .Where(s => s.IdLocatar == userId)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sesizare>> GetSesizare(int id)
    {
        var sesizare = await _context.Sesizari.FindAsync(id);

        if (sesizare == null) return NotFound();

        return sesizare;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSesizare(int id, Sesizare sesizare)
    {
        if (id != sesizare.ID) return BadRequest();

        var existingSesizare = await _context.Sesizari.AsNoTracking().FirstOrDefaultAsync(s => s.ID == id);
        if (existingSesizare == null) return NotFound();

        _context.Entry(sesizare).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();

            if (existingSesizare.Status != sesizare.Status)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == sesizare.IdLocatar);
                if (user != null)
                {
                    var dbNotif = new Notification
                    {
                        Title = "Update Sesizare",
                        Message = $"Statutul sesizării tale '{sesizare.Subiect}' a fost actualizat la: {sesizare.Status}",
                        UserId = user.Id,
                        CreatedAt = DateTime.Now
                    };
                    _context.Notifications.Add(dbNotif);
                    await _context.SaveChangesAsync();

                    await _hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", dbNotif.Title, dbNotif.Message);
                }
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SesizareExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Sesizare>> PostSesizare(Sesizare sesizare)
    {
        _context.Sesizari.Add(sesizare);
        await _context.SaveChangesAsync();

        var dbNotif = new Notification
        {
            Title = "Sesizare Nouă",
            Message = $"O nouă sesizare: {sesizare.Subiect}",
            UserId = "",
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(dbNotif);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", dbNotif.Title, dbNotif.Message);
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", dbNotif.Title,
            dbNotif.Message);

        return CreatedAtAction("GetSesizare", new { id = sesizare.ID }, sesizare);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSesizare(int id)
    {
        var sesizare = await _context.Sesizari.FindAsync(id);
        if (sesizare == null) return NotFound();

        _context.Sesizari.Remove(sesizare);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SesizareExists(int id)
    {
        return _context.Sesizari.Any(e => e.ID == id);
    }
}