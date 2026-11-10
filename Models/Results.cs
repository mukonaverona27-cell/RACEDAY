namespace RaceDay.Api.Models;

public class Results
{
    public int ResultsID { get; set; }
    public int EnrolmentID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? OverallPosition { get; set; }
    public int? CategoryPosition { get; set; }
    public string? Pace { get; set; }
    public TimeSpan? CompetitionTime { get; set; }
    public TimeSpan? ChipTime { get; set; }
    public Enrolment? Enrolment { get; set; }
}
