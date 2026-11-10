using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public EnrolmentsController(RaceDayContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Enrol(Enrolment input)
    {
        if (HttpContext.Session.GetString("Role") != "Participant") return StatusCode(403, "Participant role required.");
        var participantId = HttpContext.Session.GetInt32("UserID")!.Value;
        var category = await _db.EventCategories.FirstOrDefaultAsync(c => c.EventCategoryID == input.EventCategoryID && c.EventID == input.EventID);
        if (category is null) return BadRequest("The selected category does not belong to this event.");
        if (await _db.Enrolments.AnyAsync(e => e.ParticipantID == participantId && e.EventID == input.EventID)) return Conflict("Already enrolled in this event.");
        input.ParticipantID = participantId; input.EnrolmentID = 0; input.EnrolmentDate = DateTime.UtcNow.Date;
        _db.Enrolments.Add(input); await _db.SaveChangesAsync();
        return Created($"/api/enrolments/{input.EnrolmentID}", input);
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyEnrolments()
    {
        if (HttpContext.Session.GetString("Role") != "Participant") return StatusCode(403, "Participant role required.");
        var id = HttpContext.Session.GetInt32("UserID");
        return Ok(await _db.Enrolments.Include(e => e.Event).Include(e => e.EventCategory).Where(e => e.ParticipantID == id).ToListAsync());
    }

    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return StatusCode(403, "Organiser role required.");
        var organiserId = HttpContext.Session.GetInt32("UserID");
        if (!await _db.Events.AnyAsync(e => e.EventID == eventId && e.OrganiserID == organiserId)) return NotFound("Event not found for this organiser.");
        return Ok(await _db.Enrolments.Include(e => e.Participant).Include(e => e.EventCategory).Where(e => e.EventID == eventId).ToListAsync());
    }
}
