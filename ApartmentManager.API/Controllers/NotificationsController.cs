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
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationsController(IHubContext<NotificationHub> hubContext, AppDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMyNotifications()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await _context.Notifications
            .Where(n => n.UserId == userId || string.IsNullOrEmpty(n.UserId))
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .ToListAsync();
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationDto notification)
    {
        var dbNotif = new Notification
        {
            Title = notification.Title,
            Message = notification.Message,
            UserId = notification.UserId ?? "",
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        _context.Notifications.Add(dbNotif);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification.Title, notification.Message);
        return Ok();
    }
}