namespace Atelier.Api._Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public string Picture { get; set; } = string.Empty;

        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public PlayerData Data { get; set; } = null!;
    }
}