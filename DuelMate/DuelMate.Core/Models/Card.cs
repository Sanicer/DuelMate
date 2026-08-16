using DuelMate.Core.Enums;

namespace DuelMate.Core.Models;

public class Card
{
    public CardType Type { get; set; }

    public Card(CardType type)
    {
        Type = type;
    }
}