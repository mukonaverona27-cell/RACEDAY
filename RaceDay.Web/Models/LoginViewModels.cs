using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class LoginViewModels
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an account type")]
        public string Role { get; set; } = string.Empty;
    }
}