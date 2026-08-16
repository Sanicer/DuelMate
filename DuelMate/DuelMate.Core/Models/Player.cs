using DuelMate.Core.Enums;
using System;
using System.Collections.Generic;

namespace DuelMate.Core.Models;

public class Player
{
    private const int BaseAttackPower = 1;

    public int Health { get; set; } = 3;
    public int BuffCount { get; set; } = 0;

    public int AttackPower => BaseAttackPower + BuffCount;
    public bool IsAlive => Health > 0;

    public List<Card> Deck { get; set; } = new();
    public List<Card> Hand { get; set; } = new();
    public List<Card> DiscardPile { get; set; } = new();

    public Player()
    {
        BuildFreshDeck();
        Shuffle(Deck);
    }

    public void BuildFreshDeck()
    {
        Deck.Clear();
        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            // create 3 of each
            for (int i = 0; i < 3; i++)
            {
                Deck.Add(new Card(type));
            }
        }
    }

    public static void Shuffle(List<Card> cards)
    {
        var rng = new Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
}