using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public EventsController(RaceDayContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Events.Include(e => e.Categories).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventID == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Event item)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        item.OrganiserID = HttpContext.Session.GetInt32("UserID")!.Value;
        _db.Events.Add(item); await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id=item.EventID }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Event input)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var item = await _db.Events.FindAsync(id);
        if (item is null) return NotFound();
        if (item.OrganiserID != HttpContext.Session.GetInt32("UserID")) return Forbid();
        item.EventName=input.EventName; item.Description=input.Description; item.BannerImagePath=input.BannerImagePath; item.EventType=input.EventType; item.EventDate=input.EventDate; item.Venue=input.Venue; item.City=input.City; item.Province=input.Province; item.StartTime=input.StartTime; item.EndTime=input.EndTime;
        await _db.SaveChangesAsync(); return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var item = await _db.Events.FindAsync(id);
        if (item is null) return NotFound();
        if (item.OrganiserID != HttpContext.Session.GetInt32("UserID")) return Forbid();
        _db.Events.Remove(item); await _db.SaveChangesAsync(); return NoContent();
    }
}
