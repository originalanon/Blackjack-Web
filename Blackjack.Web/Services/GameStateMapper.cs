using System;
using Blackjack.Core;
using Blackjack.Web.Models;

namespace Blackjack.Web.Services;

public class GameStateMapper
{
    private const string SessionKey = "BLACKJACK_STATE";

    //Save the current gamestate
    public static GameState ToState(BlackjackGame game) => new()
    {
        PlayerCards = game.CurrentPlayerCards.ToList(),
        DealersCards = game.DealerCards.ToList(),
        PlayerB = game.CurrentPlayerCardsB.Any() ? game.CurrentPlayerCardsB.ToList() : null,
        
        RemainingDeck = game.RemainingDeck.TopToBottom().ToList(),
        PlayerScore= game.PlayerHandTotal(),     
        ActiveHandIndex = game.ActiveHandIndex
    };

    //Load from an existing state
    public static BlackjackGame FromState(GameState state)
    {
        var game = new BlackjackGame(1);
            game.LoadFrom(
                state.PlayerCards,
                state.DealersCards,
                state.RemainingDeck,
                state.PlayerScore,
                state.PlayerB,              
                state.ActiveHandIndex
            );
            return game;

    }

}
