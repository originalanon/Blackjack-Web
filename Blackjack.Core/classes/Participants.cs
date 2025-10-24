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

    //TODO: Implement player score increase/decrease
    //Add (or subtract, if passed a negative) to the player's score
    private void AddSubtractPlayerScore(int amount)
    {
        Score += amount;
    }

    //I want to do multiplayer later, so here's an ID and a username
    //TODO: Implement PlayerID and Username
    public Guid PlayerId { get; set; }
    public string? Username { get; set; }

    public Hand Hand { get; } = new();

    public void Receive(Card card) => Hand.Add(card);

    public Player()
    {
        Score = 0;
        PlayerId = Guid.NewGuid();
        //Change
        Username = "Jack Black";
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
            bool soft17 = value == 17 && Hand.Cards.Any(c => c.Rank == Rank.Ace) &&
                Hand.BestValue() != Hand.Cards.Sum(c => (int)c.Rank); // soft if an Ace counted as 11
            if (value < 17 || (hitSoft17 && soft17))
                Receive(deck.Draw());
            else break;
        }
    }
}

