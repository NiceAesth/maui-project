using System.Security.Claims;
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
public class FacturiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public FacturiController(AppDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FacturaIndividuala>>> GetFacturi()
    {
        return await _context.FacturiIndividuale.ToListAsync();
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<FacturaIndividuala>>> GetMyFacturi()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        return await _context.FacturiIndividuale
            .Where(f => f.IdApartament == user.ApartamentID)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FacturaIndividuala>> GetFactura(int id)
    {
        var factura = await _context.FacturiIndividuale.FindAsync(id);

        if (factura == null) return NotFound();

        return factura;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutFactura(int id, FacturaIndividuala factura)
    {
        if (id != factura.ID) return BadRequest();

        _context.Entry(factura).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FacturaExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<FacturaIndividuala>> PostFactura(FacturaIndividuala factura)
    {
        _context.FacturiIndividuale.Add(factura);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.ApartamentID == factura.IdApartament);
        if (user != null)
        {
            var dbNotif = new Notification
            {
                Title = "Factură Nouă",
                Message = $"Ai o factură nouă de {factura.SumaDePlata} RON.",
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(dbNotif);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", dbNotif.Title, dbNotif.Message);
        }

        return CreatedAtAction("GetFactura", new { id = factura.ID }, factura);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFactura(int id)
    {
        var factura = await _context.FacturiIndividuale.FindAsync(id);
        if (factura == null) return NotFound();

        _context.FacturiIndividuale.Remove(factura);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FacturaExists(int id)
    {
        return _context.FacturiIndividuale.Any(e => e.ID == id);
    }
}