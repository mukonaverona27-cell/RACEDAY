namespace RaceDay.Api.Models;

public class Route
{
    public int RouteID { get; set; }
    public int EventID { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public decimal? ElevationGain { get; set; }
    public string? RouteDescription { get; set; }
    public Event? Event { get; set; }
}
