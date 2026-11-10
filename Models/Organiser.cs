namespace RaceDay.Api.Models;

public class Organiser
{
    public int OrganiserID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<Event> Events { get; set; } = new();
}
