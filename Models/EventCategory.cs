namespace RaceDay.Api.Models;

public class EventCategory
{
    public int EventCategoryID { get; set; }
    public int EventID { get; set; }
    public decimal EntryFee { get; set; }
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
    public string Gender { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public TimeSpan StartTime { get; set; }
    public Event? Event { get; set; }
    public List<Enrolment> Enrolments { get; set; } = new();
}
