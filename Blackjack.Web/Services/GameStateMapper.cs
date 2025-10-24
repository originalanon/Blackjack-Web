using System;
using Blackjack.Core;
using Blackjack.Web.Models;

namespace Blackjack.Web.Services;

public class GameStateMapper
{
    private const string SessionKey = "BLACKJACK_STATE";

    //Save the current gamestate
    public static GameState ToState(BlackjackGame currentGame) => new()
    {
        PlayerCards = currentGame.PlayerCards.ToList(),
        DealersCards = currentGame.DealerCards.ToList(),
        PlayerB = currentGame.HasSecondHand ? currentGame.PlayerCardsB.ToList() : null,
        RemainingDeck = currentGame.RemainingDeck.TopToBottom().ToList(),
        ActiveHandIndex = currentGame.ActiveHandIndex,
    };

    //Load from an existing state
    public static BlackjackGame FromState(GameState state)
    {
        var game = new BlackjackGame(1);
        game.LoadFrom(state.PlayerCards, state.DealersCards, state.RemainingDeck, state.PlayerScore, state.PlayerB, state.ActiveHandIndex);
        return game;

    }

}
