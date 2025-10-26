using System;
using Blackjack.Core;

namespace Blackjack.Web.Services;

public class BettingService
{

    public static decimal NetPayout(BlackjackGame.Outcome outcome, decimal bet)
        {
            return outcome switch
            {
                //Win
                BlackjackGame.Outcome.PlayerBlackjack => bet * 1.5m,
                //break even on both win or bust
                BlackjackGame.Outcome.PlayerWin => bet,
                BlackjackGame.Outcome.DealerBust => bet,
                //Tie
                BlackjackGame.Outcome.Push => 0m,
                //Player lose
                _ => -bet
            };
        }

}
