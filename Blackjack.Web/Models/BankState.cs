using System;

namespace Blackjack.Web.Models;


//Class for player's bank and bet
public sealed class BankState
{
    public decimal Bank { get; set; } = 10m;
    public decimal CurrentBet { get; set; } = 0m;
}
