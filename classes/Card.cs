using System;

//TODO: Document this file
namespace blackjack;

public enum Suit { Clubs, Diamonds, Hearts, Spades }
public enum Rank { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

//Class extension for Ranks, because Ten, Jack, Queen, and King all have a value of 10 and C# thinks thinks all the face-cards
//should be "Ten" because of that
public static class RankExtension
{
    //GetValue can be used later to get the value of the named ranks
    public static int GetValue(this Rank rank) =>
        rank switch
        {
            Rank.Ace => 11,
            Rank.King or Rank.Queen or Rank.Jack => 10,
            _ => (int)rank + 2
        };
}

//TODO: Add variables for different card types (holographic, foil, prismatic?)
public class Card
{
    public Rank Rank { get; set; }
    public Suit Suit { get; set; }

    public Card(Rank rank, Suit suit)
    {
        Rank = rank; Suit = suit;
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }

}
