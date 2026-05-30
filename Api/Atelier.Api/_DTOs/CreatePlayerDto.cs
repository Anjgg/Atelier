using System.ComponentModel.DataAnnotations;

namespace Atelier.Api._DTOs
{
    public class CreatePlayerDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sex is required")]
        [RegularExpression("^[MF]$", ErrorMessage = "Sex must be 'M' or 'F'")]
        public string Sex { get; set; } = string.Empty;
        
        public string Picture { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Country code must be 3 characters long")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Player data is required")]
        public CreatePlayerDataDto Data { get; set; } = new CreatePlayerDataDto();
    }
}
