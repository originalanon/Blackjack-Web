using System;

namespace Blackjack.Core;

public sealed class Hand
{
    private readonly List<Card> _cards = new();
    public IReadOnlyList<Card> Cards => _cards;

    public void Add(Card c) => _cards.Add(c);

    //Treat aces as 11, but then 1 as needed
    public int BestValue()
    {
        int total = 0; 
        int aces = 0;

        foreach (var c in _cards)
        {
            if (c.Rank == Rank.Ace) { total += 11; aces++; }
            else total += (int)c.Rank; // Face cards already 10 via enum
        }

        while (total > 21 && aces > 0) { total -= 10; aces--; }
        return total;
    }


    public bool IsBlackjack => _cards.Count == 2 && BestValue() == 21;
    public bool IsBust => BestValue() > 21;
}
