using RaceRoute = RaceDay.Api.Models.Route;

namespace RaceDay.Api.Models;

public class Event
{
    public int EventID { get; set; }
    public int OrganiserID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BannerImagePath { get; set; }
    public DateTime EventDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Organiser? Organiser { get; set; }
    public List<EventCategory> Categories { get; set; } = new();
    public List<Enrolment> Enrolments { get; set; } = new();
    public List<RaceRoute> Routes { get; set; } = new();
    public ICollection<Weather> WeatherRecords { get; set; } = new List<Weather>();
}
