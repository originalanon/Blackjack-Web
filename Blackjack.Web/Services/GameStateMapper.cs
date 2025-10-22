using System;
using Blackjack.Core;
using Blackjack.Web.Models;

namespace Blackjack.Web.Services;

public class GameStateMapper
{

    private static GameState ToState(BlackjackGame currentGame) => new()
    {
        PlayerCards = currentGame.PlayerCards.ToList(),
        DealersCards = currentGame.DealerCards.ToList(),
        RemainingDeck = new Stack<Card>()
    };

}
