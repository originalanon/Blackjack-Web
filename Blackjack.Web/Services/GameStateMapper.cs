using System;
using Blackjack.Core;
using Blackjack.Web.Models;

namespace Blackjack.Web.Services;

public class GameStateMapper
{

    //Save the current gamestate
    private static GameState ToState(BlackjackGame currentGame) => new()
    {
        PlayerCards = currentGame.PlayerCards.ToList(),
        DealersCards = currentGame.DealerCards.ToList(),
        RemainingDeck = new Stack<Card>()
    };

    //Load from an existing state
    private static BlackjackGame FromState(GameState state)
    {
        var game = new BlackjackGame(1);
        game.LoadFrom(state.PlayerCards, state.DealersCards, state.RemainingDeck, state.PlayerScore);
        return game;

    }

}
