using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _db;
    public CategoriesController(RaceDayContext db) => _db = db;

    [HttpGet("events/{eventId:int}/categories")]
    public async Task<IActionResult> GetForEvent(int eventId) => Ok(await _db.EventCategories.Where(c => c.EventID == eventId).ToListAsync());

    [HttpPost("events/{eventId:int}/categories")]
    public async Task<IActionResult> Create(int eventId, EventCategory category)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId && e.OrganiserID == HttpContext.Session.GetInt32("UserID"))) return NotFound("Event not found for this organiser.");
        category.EventID = eventId; _db.EventCategories.Add(category); await _db.SaveChangesAsync();
        return Created($"/api/events/{eventId}/categories", category);
    }

    [HttpPut("events/{eventId:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> Update(int eventId, int categoryId, EventCategory input)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return StatusCode(403, "Organiser role required.");

        if (!await _db.Events.AnyAsync(e => e.EventID == eventId &&
            e.OrganiserID == HttpContext.Session.GetInt32("UserID")))
            return NotFound("Event not found for this organiser.");

        var category = await _db.EventCategories.FirstOrDefaultAsync(c =>
            c.EventCategoryID == categoryId && c.EventID == eventId);
        if (category is null) return NotFound();

        category.EntryFee = input.EntryFee;
        category.AgeFrom = input.AgeFrom;
        category.AgeTo = input.AgeTo;
        category.Gender = input.Gender;
        category.Distance = input.Distance;
        category.StartTime = input.StartTime;

        await _db.SaveChangesAsync();
        return Ok(category);
    }

    [HttpDelete("events/{eventId:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> Delete(int eventId, int categoryId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return StatusCode(403, "Organiser role required.");

        if (!await _db.Events.AnyAsync(e => e.EventID == eventId &&
            e.OrganiserID == HttpContext.Session.GetInt32("UserID")))
            return NotFound("Event not found for this organiser.");

        var category = await _db.EventCategories.FirstOrDefaultAsync(c =>
            c.EventCategoryID == categoryId && c.EventID == eventId);
        if (category is null) return NotFound();

        if (await _db.Enrolments.AnyAsync(e => e.EventCategoryID == categoryId))
            return Conflict("This category cannot be deleted because participants are enrolled in it.");

        _db.EventCategories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }

}
