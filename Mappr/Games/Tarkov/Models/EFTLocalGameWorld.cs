namespace Mappr.Games.Tarkov.Models
{
    public class EFTLocalGameWorld
    {
        public EFTPlayer? MainPlayer { get; set; }
        public List<EFTPlayer> RegisteredPlayers { get; set; } = new List<EFTPlayer>();
    }
}


