//TODO: Document
using System;

namespace Blackjack.Core;

public sealed class BlackjackGame
{
    private readonly Deck _deck;
    private readonly Player _player = new();
    private readonly Dealer _dealer = new();

    //public for UI
    public IReadOnlyList<Card> PlayerCards => _player.Hand.Cards;
    public IReadOnlyList<Card> DealerCards => _dealer.Hand.Cards;
    public Deck RemainingDeck => _deck;

    //More UI helpers -- these two for wheteher or not the "Split" and "Double Down" buttons should display
    public bool PlayerHandSplittable() => _player.Hand.IsSplittable;
    public bool PlayerHandDouble() => _player.Hand.IsDouble;

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

    //After player stands, resolve which win (or lose) condition from Outcome enum
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

    //Helper functions for post calls in the web app
    //Get the player's hand
    public string PlayerHandText() =>
    string.Join(", ", _player.Hand.Cards);

    //Get player hand total to display on UI
    public int PlayerHandTotal()
    {
        int total = 0;

        for (int i = 0; i < _player.Hand.Cards.Count; i++)
        {
            total += _player.Hand.Cards[i].Rank.GetValue();
        }

        return total;
    }

    //The hole card is still hidden, so only display the value of the first card
    public int DealerHandTotalUnknown() => _dealer.Hand.Cards[0].Rank.GetValue();

    //Only shows once the player has stood
    public int DealerHandTotalKnown()
    {
        int total = 0;

        for (int i = 0; i < _dealer.Hand.Cards.Count; i++)
        {
            total += _dealer.Hand.Cards[i].Rank.GetValue();
        }

        return total;
    }

    //Dealer's hand, which only reveals the dealer's face-down card (hole card, I've learned) later
    public string DealerHandText(bool revealHole = false)
    {
        if (revealHole)
        {
            return string.Join(", ", _dealer.Hand.Cards);
        }

        if (_dealer.Hand.Cards.Count == 0)
        {
            return "";
        }

        var first = _dealer.Hand.Cards[0];
        var restHidden = _dealer.Hand.Cards.Count > 1 ? ", [hole card]" : "";
        return $"{first}{restHidden}";
    }

    //Replace existing, pre-loaded cards and deck with saved ones
    //This is called in GameStateMapper

    public void LoadFrom(IEnumerable<Card> playerCards, IEnumerable<Card> dealerCards, IEnumerable<Card> remainingDeck, int PlayerScore)
    {
        //Clear exisiting hands so they can be replaced
        _player.Hand.Clear();
        _dealer.Hand.Clear();

        //Set player's current score to their saved one
        _player.Score = PlayerScore;

        //Replace
        foreach (var card in playerCards) _player.Receive(card);
        foreach (var card in dealerCards) _dealer.Receive(card);

        _deck.ReplaceDeck(remainingDeck);
    }


}

