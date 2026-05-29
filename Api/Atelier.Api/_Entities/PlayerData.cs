namespace Atelier.Api._Entities
{
    public class PlayerData
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public int Points { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int Age { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public ICollection<PlayerLastResult> LastResults { get; set; } = new List<PlayerLastResult>();
    }
}
