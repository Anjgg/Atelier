namespace Atelier.Api._DTOs
{
    public class GetAllPlayersDto
    {
        public List<PlayerNameDto> Male { get; set; } = new List<PlayerNameDto>();
        public List<PlayerNameDto> Female { get; set; } = new List<PlayerNameDto>();
    }
}
