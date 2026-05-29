namespace Atelier.Api._Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
