using DuelMate.Core.Enums;
using DuelMate.Core.Models;
using System;
using System.Diagnostics;

namespace DuelMate.Core.Services;

public class GameManager
{
    public Player Player1;
    public Player Player2;

    public void StartGame()
    {
        Player1 = new Player();
        Player2 = new Player();

        // Draw opening hands
        DrawCards(Player1);
        DrawCards(Player2);
    }

    public Card PlayCard(Player player, int cardIndex)
    {
        Debug.Write($"[GameManager] PlayCard called with index {cardIndex}");

        if (player.Hand.Count > cardIndex)
        {
            Card playedCard = player.Hand[cardIndex];
            player.DiscardPile.Add(playedCard);
            player.Hand.RemoveAt(cardIndex);

            // TEMP: Apply dummy damage if Attack
            if (playedCard.Type == CardType.Attack)
            {
                if (player == Player1)
                    Player2.Health -= 1;
                else
                    Player1.Health -= 1;
            }

            return playedCard;
        }

        return null;
    }

    public void DrawCards(Player player)
    {
        player.Hand.Clear();

        for (int i = 0; i < 3; i++)
        {
            CardType randomType = GetRandomCardType();
            var card = new Card { Type = randomType };
            player.Hand.Add(card);
        }
    }

    private CardType GetRandomCardType()
    {
        Array types = Enum.GetValues(typeof(CardType));
        Random random = new Random();
        return (CardType)types.GetValue(random.Next(0, types.Length));
    }
}