using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RaceDayContext _db;
    private readonly PasswordHasher<object> _hasher = new();
    public AuthController(RaceDayContext db) => _db = db;

    /// <summary>Registers an Organiser or Participant account and hashes its password.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || request.Password.Length < 8)
            return BadRequest("Email is required and password must contain at least 8 characters.");
        var role = request.Role.Trim();
        if (role.Equals("Participant", StringComparison.OrdinalIgnoreCase))
        {
            if (await _db.Participants.AnyAsync(x => x.Email == request.Email)) return Conflict("Email already registered.");
            var user = new Participant { Name=request.Name, Surname=request.Surname, Email=request.Email, IDNumber=request.IDNumber, DateOfBirth=request.DateOfBirth, Gender=request.Gender, Phone=request.Phone, City=request.City, Province=request.Province };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _db.Participants.Add(user);
            await _db.SaveChangesAsync();
            return Created("/api/profile", new { user.ParticipantID, user.Name, user.Surname, user.Email, Role="Participant" });
        }
        if (role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
        {
            if (await _db.Organisers.AnyAsync(x => x.Email == request.Email)) return Conflict("Email already registered.");
            var user = new Organiser { OrganizationName=string.IsNullOrWhiteSpace(request.OrganizationName) ? request.Name : request.OrganizationName, Email=request.Email, Phone=request.Phone, City=request.City };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _db.Organisers.Add(user);
            await _db.SaveChangesAsync();
            return Created("/api/profile", new { user.OrganiserID, user.OrganizationName, user.Email, Role="Organiser" });
        }
        return BadRequest("Role must be Organiser or Participant.");
    }

    /// <summary>Checks credentials and stores the user's ID and role in a session cookie.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (request.Role.Equals("Participant", StringComparison.OrdinalIgnoreCase))
        {
            var user = await _db.Participants.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user is null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed) return Unauthorized("Invalid email or password.");
            HttpContext.Session.SetInt32("UserID", user.ParticipantID);
            HttpContext.Session.SetString("Role", "Participant");
            return Ok(new { message="Login successful", userID=user.ParticipantID, user.Email, Role="Participant" });
        }
        if (request.Role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
        {
            var user = await _db.Organisers.SingleOrDefaultAsync(x => x.Email == request.Email);
            if (user is null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed) return Unauthorized("Invalid email or password.");
            HttpContext.Session.SetInt32("UserID", user.OrganiserID);
            HttpContext.Session.SetString("Role", "Organiser");
            return Ok(new { message="Login successful", userID=user.OrganiserID, user.Email, Role="Organiser" });
        }
        return BadRequest("Role must be Organiser or Participant.");
    }

    /// <summary>Clears the current session.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message="Logged out successfully" });
    }
}
