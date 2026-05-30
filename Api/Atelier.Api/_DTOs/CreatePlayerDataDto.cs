using System.ComponentModel.DataAnnotations;

namespace Atelier.Api._DTOs
{
    public class CreatePlayerDataDto
    {
        [Range(1, 1000, ErrorMessage = "Rank must be between 1 and 1000")]
        public int Rank { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Points must be positive")]
        public int Points { get; set; }

        [Range(40000, 120000, ErrorMessage = "Weight must be between 40kg and 120kg (in grams)")]
        public int Weight { get; set; }

        [Range(100, 250, ErrorMessage = "Height must be between 100cm and 250cm")]
        public int Height { get; set; }

        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100 years")]
        public int Age { get; set; }

        [MinLength(5, ErrorMessage = "Last must contain 5 results")]
        [MaxLength(5, ErrorMessage = "Last must contain exactly 5 results")]
        public List<int> Last { get; set; } = new List<int>();
    }
}
