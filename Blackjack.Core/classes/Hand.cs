/**
 * @ Author: Lindsay Barton
 * @ Description: The Hand class. Hand is a class consisting of two or more Cards. Each Participant (so, Player and Dealer) 
 * have a hand. Hands can be splittable, double-down-able, can be a blackjack, or can be a bust.
 */

using System;
using System.Reflection.Metadata.Ecma335;

namespace Blackjack.Core;

public sealed class Hand
{
    private readonly List<Card> _cards = new();
    //Public for UI
    public IReadOnlyList<Card> Cards => _cards;

    public void Add(Card c) => _cards.Add(c);

    //Remove card at index (used to remove second card after split so it can be put into the new hand)
    public void RemoveAt(int index) => _cards.RemoveAt(index);

    //Clear the hand
    public void Clear() => _cards.Clear();

    //Treat aces as 11, but then 1 as needed
    public int BestValue()
    {
        int total = 0;
        int aces = 0;

        foreach (var card in _cards)
        {
            int value = card.Rank.GetValue();
            total += value;
            if (card.Rank == Rank.Ace) aces++;
        }

        // Try to count some aces as 11 (1 + 10)
        while (aces > 0 && total + 10 <= 21)
        {
            total += 10;
            aces--;
        }

        return total;
    }


    //Is the hand splittable? Splittable when the "first two cards are of the same denomination"
    //Also there needs to be only two cards in the hand, so you can't split after you hit without splitting first
    public bool IsSplittable => _cards.Count == 2 && _cards[0].Rank == _cards[1].Rank;

    //Is the hand double-down-able? The first two cards need to sum to 9, 10, or 11
    public bool IsDouble => _cards.Count == 2 && (_cards[0].Rank.GetValue() + _cards[1].Rank.GetValue()) is 9 or 10 or 11;
    public bool IsBlackjack => _cards.Count == 2 && BestValue() == 21;
    public bool IsBust => BestValue() > 21;
}
