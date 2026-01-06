using ApartmentManager.API.Data;
using ApartmentManager.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CheltuieliController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheltuieliController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheltuialaAsociatie>>> GetCheltuieli()
    {
        return await _context.CheltuieliAsociatie.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CheltuialaAsociatie>> GetCheltuiala(int id)
    {
        var cheltuiala = await _context.CheltuieliAsociatie.FindAsync(id);

        if (cheltuiala == null) return NotFound();

        return cheltuiala;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCheltuiala(int id, CheltuialaAsociatie cheltuiala)
    {
        if (id != cheltuiala.ID) return BadRequest();

        _context.Entry(cheltuiala).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CheltuialaExists(id)) return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<CheltuialaAsociatie>> PostCheltuiala(CheltuialaAsociatie cheltuiala)
    {
        _context.CheltuieliAsociatie.Add(cheltuiala);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCheltuiala", new { id = cheltuiala.ID }, cheltuiala);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCheltuiala(int id)
    {
        var cheltuiala = await _context.CheltuieliAsociatie.FindAsync(id);
        if (cheltuiala == null) return NotFound();

        _context.CheltuieliAsociatie.Remove(cheltuiala);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CheltuialaExists(int id)
    {
        return _context.CheltuieliAsociatie.Any(e => e.ID == id);
    }
}