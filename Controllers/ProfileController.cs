using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly RaceDayContext _db;
    public ProfileController(RaceDayContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var role = HttpContext.Session.GetString("Role"); var id = HttpContext.Session.GetInt32("UserID");
        if (role is null || id is null) return Unauthorized();
        if (role == "Participant") { var p = await _db.Participants.FindAsync(id); return p is null ? NotFound() : Ok(new { p.ParticipantID,p.Name,p.Surname,p.Email,p.IDNumber,p.DateOfBirth,p.Gender,p.Phone,p.City,p.Province,p.ProfileImagePath,Role=role }); }
        var o = await _db.Organisers.FindAsync(id); return o is null ? NotFound() : Ok(new { o.OrganiserID,o.OrganizationName,o.Email,o.Phone,o.City,Role=role });
    }
}
