using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class WeatherController : ControllerBase
{
    private readonly RaceDayContext _db;
    public WeatherController(RaceDayContext db) => _db = db;

    [HttpGet("events/{eventId:int}/weather")]
    public async Task<IActionResult> GetForEvent(int eventId) => Ok(await _db.WeatherRecords.Where(w => w.EventID == eventId).ToListAsync());

    [HttpPost("weather")]
    public async Task<IActionResult> Create(Weather weather)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var organiserId = HttpContext.Session.GetInt32("UserID");
        if (!await _db.Events.AnyAsync(e => e.EventID == weather.EventID && e.OrganiserID == organiserId)) return NotFound("Event not found for this organiser.");
        _db.WeatherRecords.Add(weather); await _db.SaveChangesAsync();
        return Created($"/api/weather/{weather.WeatherID}", weather);
    }
}
