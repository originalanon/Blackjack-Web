/**
 * @ Author: Lindsay Barton
 * @ Description: The Participant class, which contains two inherit classes: Player and Dealer. Player and Dealer
 * each have a Hand.
 */

using System;

namespace Blackjack.Core;

//Participant interface that Dealer and Player will inherit from
public interface IParticipant
{
    //Create a hand
    Hand Hand { get; }
    void Receive(Card card);
}

//Player class
public class Player : IParticipant
{
    public int Score { get; set; }


    public Hand Hand { get; } = new();

    public void Receive(Card card) => Hand.Add(card);

    public Player()
    {
        Score = 0;
    }

}

public sealed class Dealer : IParticipant
{
    public Hand Hand { get; } = new();
    public void Receive(Card card) => Hand.Add(card);


    //Function to get the dealer to play
    //According to this: https://www.blackjackapprenticeship.com/blackjack-strategy-charts/ , Dealers usually stay at 17
    public void Play(Deck deck, bool hitSoft17 = false)
    {
        while (true)
        {
            int value = Hand.BestValue();

            bool soft17 = value == 17 && Hand.Cards.Any(c => c.Rank == Rank.Ace) && Hand.BestValue() != Hand.Cards.Sum(c => (int)c.Rank);

            if (value < 17 || (hitSoft17 && soft17))
            {

                Receive(deck.Draw());
            }

            else break;
        }
    }
}

