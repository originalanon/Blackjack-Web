//GameState class to store game data between HTTP requests and sessions
//Stores Player Cards, Dealer's Cards, the rest of the deck, Player's current score
//TODO: Store player chips and items once that's impelemented
using System;
using Blackjack.Core;

namespace Blackjack.Web.Models;

public class GameState
{
    public List<Card> PlayerCards { get; set; } = new();
    public List<Card> DealersCards { get; set; } = new();
    public List<Card> RemainingDeck { get; set; } = new();

    public int PlayerScore { get; set; } = new();
}
