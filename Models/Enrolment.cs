namespace RaceDay.Api.Models;

public class Enrolment
{
    public int EnrolmentID { get; set; }
    public int ParticipantID { get; set; }
    public int EventID { get; set; }
    public int EventCategoryID { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "Confirmed";
    public Participant? Participant { get; set; }
    public Event? Event { get; set; }
    public EventCategory? EventCategory { get; set; }
    public Results? Results { get; set; }
}
