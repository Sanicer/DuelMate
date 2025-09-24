using System.Collections.Generic;

namespace DuelMate.Core.Models;

public class Player
{
    public int Health { get; set; }
    public int BuffCount { get; set; }

    public List<Card> Deck { get; set; } = new();
    public List<Card> Hand { get; set; } = new();
    public List<Card> DiscardPile { get; set; } = new();

    public Player()
    {
        Hand = new List<Card>();
        DiscardPile = new List<Card>();
        Health = 3;
        BuffCount = 0;
    }
}