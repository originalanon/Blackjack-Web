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
}

