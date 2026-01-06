using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Shared.Models;

public class Notification
{
    public int Id { get; set; }

    [Required] public string Title { get; set; } = string.Empty;

    [Required] public string Message { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsRead { get; set; } = false;
}