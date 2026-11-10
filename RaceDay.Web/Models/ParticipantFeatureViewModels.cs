using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class EnrolmentViewModels
    {
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public int EventID { get; set; }

        [Required]
        public int EventCategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "Confirmed";
        public EventSummaryViewModels? Event { get; set; }
        public CategorySummaryViewModels? EventCategory { get; set; }
        public ParticipantSummaryViewModels? Participant { get; set; }
    }

    public class EventSummaryViewModels
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
    }

    public class CategorySummaryViewModels
    {
        public int EventCategoryID { get; set; }
        public decimal Distance { get; set; }
        public decimal EntryFee { get; set; }
        public string Gender { get; set; } = string.Empty;
    }

    public class ParticipantSummaryViewModels
    {
        public int ParticipantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ResultViewModels
    {
        public int ResultsID { get; set; }
        public int EventID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? FinishTime { get; set; }

        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public string? Pace { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? CompetitionTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? ChipTime { get; set; }
        public EnrolmentViewModels? Enrolment { get; set; }
    }

    public class ProfileViewModels
    {
        public int ParticipantID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string IDNumber { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string Role { get; set; } = "Participant";
    }
}
