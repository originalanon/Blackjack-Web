using System;

namespace Blackjack.Core;

//TODO: Document this file
/**************************************************************
*
*/
    
public class Deck
{
    //A deck is just a stack of cards IRL, so it's a stack here, too
    private readonly Stack<Card> _cards = new();
    private static readonly Random _rng = new();

    public IEnumerable<Card> TopToBottom() => _cards;

    public Deck(int decks = 1)
    {
        //List of cards
        var list = new List<Card>();

        //Generate the cards. One of each rank in each suit
        for (int d = 0; d < decks; d++)
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                    list.Add(new Card(r, s));
        //Shuffle
        Shuffle(list);
        //push the shuffled list to _cards
        foreach (var c in list) _cards.Push(c);
    }


    //Shuffle the cards
    public static void Shuffle(List<Card> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        { int j = _rng.Next(i + 1); (list[i], list[j]) = (list[j], list[i]); }
    }

    //String override to print cards each on one line so it's readable
    public override string ToString()
    {
        return $"Deck: [{string.Join("\n ", _cards)}]";
    }

    //A deck is just a stack, so just pop from the top ;) -- I love when OOP works out like this lol
    public Card Draw() => _cards.Pop();

    //Replace pre-loaded deck with either a deck from a previously saved game, OR with a Funny(tm) deck in the future ;)
    public void ReplaceDeck(IEnumerable<Card> topToBottom)
    {
        _cards.Clear();

        //Debug
        var list = topToBottom is IList<Card> il ? il : topToBottom.ToList();

        //Push backwards bc it's a stack
        for (int i = list.Count - 1; i >= 0; i--)
        {
            _cards.Push(list[i]);
        }
    }
}
