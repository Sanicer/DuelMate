using DuelMate.Core.Enums;
using DuelMate.Core.Models;
using System;

namespace DuelMate.Core.Services;

public class GameManager
{
    public Player Player1;
    public Player Player2;
    public bool IsGameOver { get; private set; }
    public Player Winner { get; private set; }
    private const int HandSize = 3;

    public void StartGame()
    {
        Player1 = new Player();
        Player2 = new Player();
        IsGameOver = false;
        Winner = null;

        DrawHand(Player1);
        DrawHand(Player2);
    }

    public void DrawHand(Player player)
    {
        if (player.Hand.Count > 0)
        {
            player.Deck.AddRange(player.Hand);
            player.Hand.Clear();
        }

        for (int i = 0; i < HandSize; i++)
        {
            if (player.Deck.Count == 0)
            {
                ReshuffleDiscardIntoDeck(player);
            }

            int lastIndex = player.Deck.Count - 1;
            Card drawn = player.Deck[lastIndex];
            player.Deck.RemoveAt(lastIndex);
            player.Hand.Add(drawn);
        }
    }

    private void ReshuffleDiscardIntoDeck(Player player)
    {
        player.Deck.AddRange(player.DiscardPile);
        player.DiscardPile.Clear();
        Player.Shuffle(player.Deck);
    }

    // Resolves a full round: both players commit a card index from their
    // *current* hand simultaneously. Returns what happened.
    public RoundResult PlayRound(int player1CardIndex, int player2CardIndex)
    {
        if (IsGameOver)
        {
            throw new InvalidOperationException("The game has already ended.");
        }

        int player1HealthBefore = Player1.Health;
        int player2HealthBefore = Player2.Health;

        Card p1Card = TakeCardFromHand(Player1, player1CardIndex);
        Card p2Card = TakeCardFromHand(Player2, player2CardIndex);

        RoundResult result = new RoundResult
        {
            Player1Card = p1Card,
            Player2Card = p2Card
        };

        Resolve(Player1, p1Card, Player2, p2Card, result);

        // Only played cards become visible in the discard pile.
        Player1.DiscardPile.Add(p1Card);
        Player2.DiscardPile.Add(p2Card);

        if (!Player1.IsAlive && !Player2.IsAlive)
        {
            // Draws should not happened
            throw new InvalidOperationException(
                $"Invalid game state: both players reached 0 health in the same round. " +
                $"Player1 played {p1Card.Type} (health {player1HealthBefore} -> {Player1.Health}, buffs {Player1.BuffCount}). " +
                $"Player2 played {p2Card.Type} (health {player2HealthBefore} -> {Player2.Health}, buffs {Player2.BuffCount}).");
        }

        if (!Player1.IsAlive || !Player2.IsAlive)
        {
            IsGameOver = true;
            result.GameOver = true;
            Winner = Player1.IsAlive ? Player1 : Player2;
            result.Winner = Winner;
        }

        return result;
    }

    private static Card TakeCardFromHand(Player player, int index)
    {
        if (index < 0 || index >= player.Hand.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Card index is not in the player's current hand.");
        }

        Card card = player.Hand[index];
        player.Hand.RemoveAt(index);
        return card;
    }

    private static void Resolve(Player p1, Card p1Card, Player p2, Card p2Card, RoundResult result)
    {
        CardType t1 = p1Card.Type;
        CardType t2 = p2Card.Type;

        if (t1 == CardType.Attack && t2 == CardType.Attack)
        {
            if (p1.AttackPower > p2.AttackPower)
            {
                p2.Health -= 1;
                result.Player2DamageTaken = 1;
            }
            else if (p2.AttackPower > p1.AttackPower)
            {
                p1.Health -= 1;
                result.Player1DamageTaken = 1;
            }
            // Equal attack power: no damage to either side.
        }
        else if (t1 == CardType.Attack && t2 == CardType.Parry)
        {
            ResolveAttackVsParry(attacker: p1, result, attackerIsPlayer1: true);
        }
        else if (t1 == CardType.Parry && t2 == CardType.Attack)
        {
            ResolveAttackVsParry(attacker: p2, result, attackerIsPlayer1: false);
        }
        else if (t1 == CardType.Parry && t2 == CardType.Parry)
        {
            // Nothing happens.
        }
        else if (t1 == CardType.Attack && t2 == CardType.Setup)
        {
            p2.Health -= 1;
            result.Player2DamageTaken = 1;
        }
        else if (t1 == CardType.Setup && t2 == CardType.Attack)
        {
            p1.Health -= 1;
            result.Player1DamageTaken = 1;
        }
        else if (t1 == CardType.Parry && t2 == CardType.Setup)
        {
            p2.BuffCount += 2;
        }
        else if (t1 == CardType.Setup && t2 == CardType.Parry)
        {
            p1.BuffCount += 2;
        }
        else if (t1 == CardType.Setup && t2 == CardType.Setup)
        {
            p1.BuffCount += 1;
            p2.BuffCount += 1;
        }
    }

    // Parry blocks and strips 1 buff from the attacker. If the attacker has no buffs to strip,
    // the attacker takes 1 damage for attacking into a Parry with nothing to back it up.
    private static void ResolveAttackVsParry(Player attacker, RoundResult result, bool attackerIsPlayer1)
    {
        if (attacker.BuffCount > 0)
        {
            attacker.BuffCount -= 1;
        }
        else
        {
            attacker.Health -= 1;
            if (attackerIsPlayer1)
            {
                result.Player1DamageTaken = 1;
            }
            else
            {
                result.Player2DamageTaken = 1;
            }
        }
    }
}