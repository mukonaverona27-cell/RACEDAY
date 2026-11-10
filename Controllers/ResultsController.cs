using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceResult = RaceDay.Api.Models.Results;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public ResultsController(RaceDayContext db) => _db = db;

    [HttpGet("my")]
    public async Task<IActionResult> MyResults()
    {
        if (HttpContext.Session.GetString("Role") != "Participant") return StatusCode(403, "Participant role required.");
        var id = HttpContext.Session.GetInt32("UserID");
        return Ok(await _db.Results
            .Include(r => r.Enrolment).ThenInclude(e => e!.Event)
            .Include(r => r.Enrolment).ThenInclude(e => e!.EventCategory)
            .Where(r => r.Enrolment!.ParticipantID == id)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(RaceResult result)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var enrolment = await _db.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentID == result.EnrolmentID);
        if (enrolment is null) return NotFound("Enrolment not found.");
        if (enrolment.Event!.OrganiserID != HttpContext.Session.GetInt32("UserID")) return Forbid();
        _db.Results.Add(result); await _db.SaveChangesAsync(); return Created($"/api/results/{result.ResultsID}", result);
    }

    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId && e.OrganiserID == HttpContext.Session.GetInt32("UserID"))) return NotFound();
        return Ok(await _db.Results
            .Include(r => r.Enrolment).ThenInclude(e => e!.Participant)
            .Include(r => r.Enrolment).ThenInclude(e => e!.EventCategory)
            .Where(r => r.Enrolment!.EventID == eventId)
            .ToListAsync());
    } 
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return StatusCode(403, "Organiser role required.");

        var result = await _db.Results
            .Include(r => r.Enrolment)
            .ThenInclude(e => e!.Event)
            .FirstOrDefaultAsync(r => r.ResultsID == id);

        if (result is null)
            return NotFound();

        if (result.Enrolment!.Event!.OrganiserID !=
            HttpContext.Session.GetInt32("UserID"))
            return Forbid();

        _db.Results.Remove(result);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}
