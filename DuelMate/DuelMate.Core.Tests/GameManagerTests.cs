using DuelMate.Core.Enums;
using DuelMate.Core.Models;
using DuelMate.Core.Services;

namespace DuelMate.Core.Tests;

public class GameManagerTests
{
    private static GameManager CreateGameWithHands(CardType player1Card, CardType player2Card)
    {
        var game = new GameManager();
        game.StartGame();

        game.Player1.Hand.Clear();
        game.Player1.Hand.Add(new Card(player1Card));

        game.Player2.Hand.Clear();
        game.Player2.Hand.Add(new Card(player2Card));

        return game;
    }

    [Fact]
    public void AttackVsAttack_HigherPowerWins()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Attack);
        game.Player1.BuffCount = 1; // Player1 attack power 2, Player2 attack power 1

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(0, result.Player1DamageTaken);
        Assert.Equal(1, result.Player2DamageTaken);
        Assert.Equal(2, game.Player2.Health);
    }

    [Fact]
    public void AttackVsAttack_EqualPower_NoDamage()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Attack);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(0, result.Player1DamageTaken);
        Assert.Equal(0, result.Player2DamageTaken);
        Assert.Equal(3, game.Player1.Health);
        Assert.Equal(3, game.Player2.Health);
    }

    [Fact]
    public void AttackVsParry_AttackerHasBuffs_StripsBuffNoDamage()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Parry);
        game.Player1.BuffCount = 2;

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(-1, result.Player1BuffChange);
        Assert.Equal(1, game.Player1.BuffCount);
        Assert.Equal(0, result.Player1DamageTaken);
        Assert.Equal(3, game.Player1.Health);
    }

    [Fact]
    public void AttackVsParry_AttackerHasNoBuffs_AttackerTakesDamage()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Parry);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(0, result.Player1BuffChange);
        Assert.Equal(1, result.Player1DamageTaken);
        Assert.Equal(2, game.Player1.Health);
        Assert.Equal(3, game.Player2.Health); // parrier untouched either way
    }

    [Fact]
    public void ParryVsParry_NothingHappens()
    {
        var game = CreateGameWithHands(CardType.Parry, CardType.Parry);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(0, result.Player1DamageTaken);
        Assert.Equal(0, result.Player2DamageTaken);
        Assert.Equal(0, game.Player1.BuffCount);
        Assert.Equal(0, game.Player2.BuffCount);
    }

    [Fact]
    public void AttackVsSetup_AttackerDealsDamage_SetupGetsNothing()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Setup);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(1, result.Player2DamageTaken);
        Assert.Equal(2, game.Player2.Health);
        Assert.Equal(0, game.Player2.BuffCount);
    }

    [Fact]
    public void ParryVsSetup_SetupPlayerGainsTwoBuffs()
    {
        var game = CreateGameWithHands(CardType.Parry, CardType.Setup);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(2, result.Player2BuffChange);
        Assert.Equal(2, game.Player2.BuffCount);
        Assert.Equal(0, game.Player1.BuffCount);
    }

    [Fact]
    public void SetupVsSetup_BothGainOneBuff()
    {
        var game = CreateGameWithHands(CardType.Setup, CardType.Setup);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Equal(1, result.Player1BuffChange);
        Assert.Equal(1, result.Player2BuffChange);
        Assert.Equal(1, game.Player1.BuffCount);
        Assert.Equal(1, game.Player2.BuffCount);
    }

    [Fact]
    public void PlayRound_HealthReachesZero_EndsGameWithCorrectWinner()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Setup);
        game.Player2.Health = 1;

        RoundResult result = game.PlayRound(0, 0);

        Assert.True(result.GameOver);
        Assert.True(game.IsGameOver);
        Assert.Equal(game.Player1, result.Winner);
        Assert.False(game.Player2.IsAlive);
    }

    [Fact]
    public void PlayRound_MovesOnlyPlayedCardsToDiscardPile()
    {
        var game = CreateGameWithHands(CardType.Setup, CardType.Setup);

        RoundResult result = game.PlayRound(0, 0);

        Assert.Single(game.Player1.DiscardPile);
        Assert.Equal(CardType.Setup, game.Player1.DiscardPile[0].Type);
    }

    [Fact]
    public void PlayRound_ThrowsWhenGameAlreadyOver()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Setup);
        game.Player2.Health = 1;
        RoundResult result = game.PlayRound(0, 0);

        Assert.Throws<InvalidOperationException>(() => game.PlayRound(0, 0));
    }

    [Fact]
    public void PlayRound_ThrowsOnInvalidCardIndex()
    {
        var game = CreateGameWithHands(CardType.Attack, CardType.Setup);

        Assert.Throws<ArgumentOutOfRangeException>(() => game.PlayRound(5, 0));
    }
}