using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/participants")]
public class ParticipantsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public ParticipantsController(RaceDayContext db) => _db = db;

    // GET: api/participants
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return StatusCode(403, "Organiser role required.");

        return Ok(await _db.Participants
            .Select(p => new
            {
                p.ParticipantID,
                p.Name,
                p.Surname,
                p.Email,
                p.City,
                p.Province
            })
            .ToListAsync());
    }

    // GET: api/participants/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Participants
            .Where(p => p.ParticipantID == id)
            .Select(p => new
            {
                p.ParticipantID,
                p.Name,
                p.Surname,
                p.Email,
                p.IDNumber,
                p.DateOfBirth,
                p.Gender,
                p.Phone,
                p.City,
                p.Province,
                p.ProfileImagePath,
                p.DateJoined
            })
            .FirstOrDefaultAsync();

        return item is null ? NotFound() : Ok(item);
    }

    // PUT: api/participants/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Participant input)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return StatusCode(403, "Participant role required.");

        var item = await _db.Participants.FindAsync(id);

        if (item is null) return NotFound();

        if (item.ParticipantID != HttpContext.Session.GetInt32("UserID"))
            return Forbid();

        item.Name = input.Name;
        item.Surname = input.Surname;
        item.Email = input.Email;
        item.DateOfBirth = input.DateOfBirth;
        item.Gender = input.Gender;
        item.Phone = input.Phone;
        item.City = input.City;
        item.Province = input.Province;
        item.ProfileImagePath = input.ProfileImagePath;

        await _db.SaveChangesAsync();
        return Ok(new
        {
            item.ParticipantID,
            item.Name,
            item.Surname,
            item.Email,
            item.DateOfBirth,
            item.Gender,
            item.Phone,
            item.City,
            item.Province,
            item.ProfileImagePath
        });
    }

    // DELETE: api/participants/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return StatusCode(403, "Participant role required.");

        var item = await _db.Participants.FindAsync(id);

        if (item is null) return NotFound();

        if (item.ParticipantID != HttpContext.Session.GetInt32("UserID"))
            return Forbid();

        var hasEnrolments = await _db.Enrolments
            .AnyAsync(e => e.ParticipantID == id);

        if (hasEnrolments)
            return Conflict("Cannot delete a participant who has enrolments.");

        _db.Participants.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}