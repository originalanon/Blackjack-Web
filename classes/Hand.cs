using System;

namespace blackjack;

public class Hand
{
    private readonly List<Card> _cards = new();
    public IReadOnlyList<Card> Cards => _cards;

    //Treat aces as 11, but then 1 as needed
    public int BestValue()
    {
        //TODO: Handle aces and add values together
        return 0;
    }


    public bool IsBlackjack => _cards.Count == 2 && BestValue() == 21;
    public bool IsBust => BestValue() > 21;
}

