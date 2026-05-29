namespace Atelier.Api._Entities
{
    public class PlayerLastResult
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public bool Result { get; set; }

        public int PlayerDataId { get; set; }
        public PlayerData PlayerData { get; set; } = null!;
    }
}
