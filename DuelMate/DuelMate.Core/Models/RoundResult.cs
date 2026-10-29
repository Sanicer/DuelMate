namespace DuelMate.Core.Models
{
    public class RoundResult
    {
        public Card Player1Card { get; set; }
        public Card Player2Card { get; set; }
        public int Player1DamageTaken { get; set; }
        public int Player2DamageTaken { get; set; }
        public bool GameOver { get; set; }
        public Player Winner { get; set; }
        public int Player1BuffChange { get; set; }
        public int Player2BuffChange { get; set; }
    }
}
