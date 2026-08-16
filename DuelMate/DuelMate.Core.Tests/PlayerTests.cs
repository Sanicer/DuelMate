using DuelMate.Core.Enums;
using DuelMate.Core.Models;

namespace DuelMate.Core.Tests;

public class PlayerTests
{
    [Fact]
    public void Constructor_BuildsNineCardDeck_ThreeOfEachType()
    {
        var player = new Player();

        Assert.Equal(9, player.Deck.Count);
        Assert.Equal(3, player.Deck.Count(c => c.Type == CardType.Attack));
        Assert.Equal(3, player.Deck.Count(c => c.Type == CardType.Parry));
        Assert.Equal(3, player.Deck.Count(c => c.Type == CardType.Setup));
    }

    [Fact]
    public void Constructor_SetsStartingHealthAndBuffs()
    {
        var player = new Player();

        Assert.Equal(3, player.Health);
        Assert.Equal(0, player.BuffCount);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(5, 6)]
    public void AttackPower_IsOnePlusBuffCount(int buffCount, int expectedPower)
    {
        var player = new Player { BuffCount = buffCount };

        Assert.Equal(expectedPower, player.AttackPower);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsAlive_ReflectsHealth(int health, bool expectedAlive)
    {
        var player = new Player { Health = health };

        Assert.Equal(expectedAlive, player.IsAlive);
    }

    [Fact]
    public void Shuffle_ReordersWithoutLosingOrDuplicatingCards()
    {
        var player = new Player();
        var beforeTypes = player.Deck.Select(c => c.Type).OrderBy(t => t).ToList();

        Player.Shuffle(player.Deck);

        var afterTypes = player.Deck.Select(c => c.Type).OrderBy(t => t).ToList();
        Assert.Equal(9, player.Deck.Count);
        Assert.Equal(beforeTypes, afterTypes);
    }
}