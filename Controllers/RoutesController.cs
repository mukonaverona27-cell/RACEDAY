using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceRoute = RaceDay.Api.Models.Route;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class RoutesController : ControllerBase
{
    private readonly RaceDayContext _db;
    public RoutesController(RaceDayContext db) => _db = db;

    [HttpGet("events/{eventId:int}/routes")]
    public async Task<IActionResult> GetForEvent(int eventId) => Ok(await _db.Routes.Where(r => r.EventID == eventId).ToListAsync());

    [HttpPost("routes")]
    public async Task<IActionResult> Create(RaceRoute route)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var organiserId = HttpContext.Session.GetInt32("UserID");
        if (!await _db.Events.AnyAsync(e => e.EventID == route.EventID && e.OrganiserID == organiserId)) return NotFound("Event not found for this organiser.");
        _db.Routes.Add(route); await _db.SaveChangesAsync();
        return Created($"/api/routes/{route.RouteID}", route);
    } 
}
