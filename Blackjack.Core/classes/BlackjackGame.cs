//TODO: Document
using System;

namespace Blackjack.Core;

public sealed class BlackjackGame
{
    private readonly Deck _deck;
    private readonly Player _player = new();
    private readonly Dealer _dealer = new();

    public BlackjackGame(int decks = 1) => _deck = new Deck(decks);

    public void DealInitial()
    {
        // Standard dealing
        _player.Receive(_deck.Draw());
        _dealer.Receive(_deck.Draw());
        _player.Receive(_deck.Draw());
        _dealer.Receive(_deck.Draw());
    }

    public enum Outcome { PlayerBlackjack, DealerBlackjack, PlayerBust, DealerBust, PlayerWin, DealerWin, Push }

    public Outcome ResolveAfterPlayerStand()
    {
        if (_player.Hand.IsBlackjack && _dealer.Hand.IsBlackjack) return Outcome.Push;
        if (_player.Hand.IsBlackjack) return Outcome.PlayerBlackjack;
        if (_dealer.Hand.IsBlackjack) return Outcome.DealerBlackjack;

        _dealer.Play(_deck);

        if (_dealer.Hand.IsBust) return Outcome.DealerBust;

        int p = _player.Hand.BestValue();
        int d = _dealer.Hand.BestValue();
        if (p > d) return Outcome.PlayerWin;
        if (p < d) return Outcome.DealerWin;
        return Outcome.Push;
    }

    public bool PlayerHit()
    {
        _player.Receive(_deck.Draw());
        return _player.Hand.IsBust;
    }

    //TODO: Double-down, split
    //TODO: Update score
    //TODO: Post-hand "store" for rare cards
}
