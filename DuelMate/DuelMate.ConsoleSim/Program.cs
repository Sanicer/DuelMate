using DuelMate.Core.Models;
using DuelMate.Core.Services;

var game = new GameManager();
game.StartGame();

Console.WriteLine("DuelMate console sim. Each round, pick a card index (0-2) for each player.");
Console.WriteLine();

while (!game.IsGameOver)
{
    PrintHand("Player 1", game.Player1);
    int p1Index = PromptForCardIndex("Player 1");

    PrintHand("Player 2", game.Player2);
    int p2Index = PromptForCardIndex("Player 2");

    RoundResult result = game.PlayRound(p1Index, p2Index);

    Console.WriteLine();
    Console.WriteLine($"Player 1 played {result.Player1Card.Type}, Player 2 played {result.Player2Card.Type}");
    Console.WriteLine($"Player 1 took {result.Player1DamageTaken} damage, Player 2 took {result.Player2DamageTaken} damage");
    Console.WriteLine($"HP -> Player 1: {game.Player1.Health}, Player 2: {game.Player2.Health}");
    Console.WriteLine($"Buffs -> Player 1: {game.Player1.BuffCount} (power {game.Player1.AttackPower}), Player 2: {game.Player2.BuffCount} (power {game.Player2.AttackPower})");
    Console.WriteLine();

    if (result.GameOver)
    {
        string outcome = result.Winner == null
            ? "Draw! Both players ran out of health at once."
            : (result.Winner == game.Player1 ? "Player 1 wins!" : "Player 2 wins!");
        Console.WriteLine(outcome);
        break;
    }

    game.DrawHand(game.Player1);
    game.DrawHand(game.Player2);
}

static void PrintHand(string label, Player player)
{
    string cards = string.Join(", ", player.Hand.Select((c, i) => $"[{i}] {c.Type}"));
    Console.WriteLine($"{label} hand: {cards}");
}

static int PromptForCardIndex(string label)
{
    while (true)
    {
        Console.Write($"{label} card index: ");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int index) && index >= 0 && index <= 2)
        {
            return index;
        }
        Console.WriteLine("Enter 0, 1, or 2.");
    }
}