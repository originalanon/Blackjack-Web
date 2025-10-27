/**
 * @ Author: Lindsay Barton
 * @ Description: This is the class for Cards, the most granular class in the program. Cards don't inherit from anything.
 *   Cards have a suit, a rank, a material, and a coating.
 */

using System;

namespace Blackjack.Core;

public enum Suit { Clubs, Diamonds, Hearts, Spades }
public enum Rank { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

//Card multipliers based on coating
public enum CardCoat { Standard = 0, Foil = 2, Holographic = 3, Prismatic = 4, Specular = 5}

//Card modifiers based on material (+0 to original card value score, +3, +5, etc.)
public enum CardMaterial { Standard = 0, Stone = 3, Silver = 5, Gold = 7, Platinum = 10, Ethereum = 15 }

//Class extension for Ranks, because Ten, Jack, Queen, and King all have a value of 10 and C# thinks thinks all the face-cards
//should be "Ten" because of that
public static class RankExtension
{
    //GetValue can be used later to get the value of the named ranks
    /// <summary>
    /// Get Value - Gets the numeric value of a card's rank (Two = 2, Jack = 10, etc.)
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>

    public static int GetValue(this Rank rank) =>
        rank switch
        {
            Rank.Ace => 11,
            Rank.King or Rank.Queen or Rank.Jack => 10,
            _ => (int)rank + 2
        };
}

public class Card
{
    private static readonly Random _rng = new();

    public Rank Rank { get; set; }
    public Suit Suit { get; set; }

    //Card coating
    public CardCoat Coat { get; set; }

    //Card material
    public CardMaterial CardMaterial { get; set; }

    //Card score has to be +1 to coat mult or else it'd be 0 for standard
    public int CardScore => Rank.GetValue() + ((int)CardMaterial) * (((int)Coat) + 1); 

    //TODO: Card score based off of ((rank * 10) + material bonus) * coat mult)
    public Card(Rank rank, Suit suit)
    {
        Rank = rank;
        Suit = suit;
        this.Coat = GenerateRandomCoat();
        this.CardMaterial = GenerateRandomMaterial();

    }

    /// <summary>
    /// "{rank} of {suit}"
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Rank} of {Suit}, [{Coat}, {CardMaterial}]";
    }

    private static CardCoat GenerateRandomCoat()
    {
        double roll = _rng.NextDouble();

        return roll switch
        {
            <= 0.70 => CardCoat.Standard,
            <= 0.88 => CardCoat.Foil,
            <= 0.97 => CardCoat.Holographic,
            <= 0.993 => CardCoat.Prismatic,
            _ => CardCoat.Specular
        };
    }

    private static CardMaterial GenerateRandomMaterial()
    {
        double roll = _rng.NextDouble();

        return roll switch
        {
            <= 0.70 => CardMaterial.Standard,
            <= 0.88 => CardMaterial.Stone,
            <= 0.97 => CardMaterial.Silver,
            <= 0.993 => CardMaterial.Gold,
            <= 0.998 => CardMaterial.Platinum,
            _ => CardMaterial.Ethereum
        };
    }

}