namespace RaceDay.Api.Models
{
    public class Weather
    {
        public int WeatherID { get; set; }

        public int EventID { get; set; }

        public string Conditions { get; set; } = string.Empty;

        public decimal? Temperature { get; set; }

        public decimal? WindSpeed { get; set; }

        public decimal? Rainfall { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public Event? Event { get; set; }
    }
}