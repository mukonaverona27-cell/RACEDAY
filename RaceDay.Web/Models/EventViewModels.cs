using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class EventViewModels
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }

        [Required]
        [Display(Name = "Event name")]
        public string EventName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event type")]
        public string EventType { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required]
        public string Venue { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Province { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public string? BannerImagePath { get; set; }
    }

    public class EventCategoryViewModels
    {
        public int EventCategoryID { get; set; }
        public int EventID { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal EntryFee { get; set; }

        [Required]
        [Range(0, 120)]
        public int AgeFrom { get; set; }

        [Required]
        [Range(0, 120)]
        public int AgeTo { get; set; }

        [Required]
        public string Gender { get; set; } = "All";

        [Required]
        [Range(0.1, 500)]
        public decimal Distance { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
    }
}
