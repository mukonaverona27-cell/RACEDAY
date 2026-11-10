namespace RaceDay.Api.Models;

public class Participant
{
    public int ParticipantID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IDNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow.Date;
    public string PasswordHash { get; set; } = string.Empty;
    public List<Enrolment> Enrolments { get; set; } = new();
}
