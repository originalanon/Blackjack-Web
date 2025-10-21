using System;

namespace Blackjack.Core;

public class BlackjackGame
{
    private readonly Deck _deck;

    public BlackjackGame(int decks = 1) => _deck = new Deck(decks);

    public void DealInitial()
    {
        
    }
}
