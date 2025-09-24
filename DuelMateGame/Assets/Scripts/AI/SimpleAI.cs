using DuelMate.Core.Models;
using UnityEngine;

public class SimpleAI
{
    public int ChooseCardIndex(Player aiPlayer)
    {
        if (aiPlayer.Hand == null || aiPlayer.Hand.Count == 0)
        {
            Debug.LogWarning("AI has no cards to choose from!");
            return -1;
        }

        // For now: randomly pick one of the 3 cards
        return Random.Range(0, aiPlayer.Hand.Count);
    }
}