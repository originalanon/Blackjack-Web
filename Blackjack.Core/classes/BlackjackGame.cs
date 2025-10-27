/**
 * @ Author: Lindsay Barton
 * @ Description: BlackjackGame class. This one has a bunch of members and functions for different things throughout
 * the game. It's basically a big orchestrating class that helps Participants, Hand, Deck, and Cards interact properly.
 */

using System;

namespace Blackjack.Core;

public sealed class BlackjackGame
{
    #region Private Class Members
    private readonly Deck _deck;
    private readonly Player _player = new();
    private readonly Dealer _dealer = new();

    //Splitting
    private Hand? _playerHandB;

    #endregion


    /***************************************************************/
    #region Public Properties

    // 0 = first (original), 1 = second hand
    public int ActiveHandIndex { get; private set; } = 0;
    public bool HasSecondHand => _playerHandB is not null;

    //public for UI
    public IReadOnlyList<Card> CurrentPlayerCards => ActiveHandIndex == 0 ? _player.Hand.Cards : (_playerHandB?.Cards ?? Array.Empty<Card>());
    public int CurrentPlayerValue => (ActiveHandIndex == 0 ? _player.Hand : (_playerHandB ?? _player.Hand)).BestValue();

    //Player's second hand can be null if there are no cards in the second hand
    public IReadOnlyList<Card> CurrentPlayerCardsB => _playerHandB?.Cards ?? [];

    public IReadOnlyList<Card> DealerCards => _dealer.Hand.Cards;
    public Deck RemainingDeck => _deck;


    public string CurrentPlayerHandText() => string.Join(", ", CurrentPlayerCards);

    //Value of the player's hand 
    public int PlayerHandValue(int handIndex) => (handIndex == 0 ? _player.Hand : (_playerHandB ?? _player.Hand)).BestValue();

    //Player's current hand 
    private Hand CurrentHand() => (ActiveHandIndex == 0) ? _player.Hand : (_playerHandB ?? _player.Hand);

    #endregion
    /***************************************************************/

    #region Public Properties for UI
    //More UI helpers -- these two for wheteher or not the "Split" and "Double Down" buttons should display
    public bool PlayerHandSplittable() => CurrentHand().IsSplittable && !HasSecondHand;
    public bool PlayerHandDouble() => CurrentHand().IsDouble;

    //Reworked playerHandText for new hand indexs
    public string PlayerHandText(int handIndex)
    {
        var h = handIndex == 0 ? _player.Hand : (_playerHandB ?? _player.Hand);
        return string.Join("\n", h.Cards);
    }

    //Total of the player's hand -- different from Value
    public int PlayerHandTotal() => PlayerHandValue(ActiveHandIndex);

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
        if (revealHole) return string.Join(", ", _dealer.Hand.Cards);
        if (_dealer.Hand.Cards.Count == 0) return "";
        var first = _dealer.Hand.Cards[0];
        var restHidden = _dealer.Hand.Cards.Count > 1 ? ", [hole card]" : "";
        return $"{first}{restHidden}";
    }

    #endregion

    /******************************************************/

    public BlackjackGame(int decks = 1) => _deck = new Deck(decks);

    #region Player Actions
    public void DealInitial()
    {
        // Standard dealing
        _player.Receive(_deck.Draw());
        _dealer.Receive(_deck.Draw());
        _player.Receive(_deck.Draw());
        _dealer.Receive(_deck.Draw());

        //Only one hand on initial deal
        ActiveHandIndex = 0;
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

        int p = BestValueAllHands();
        int d = _dealer.Hand.BestValue();
        if (p > d) return Outcome.PlayerWin;
        if (p < d) return Outcome.DealerWin;
        return Outcome.Push;
    }

    public bool PlayerHit()
    {
        var hand = CurrentHand();
        hand.Add(_deck.Draw());
        return hand.IsBust;
    }

    public void PlayerSplit()
    {
        if (!PlayerHandSplittable())
            throw new InvalidOperationException("Hand is not splittable or already split.");

        //Move the second card to new hand
        _playerHandB = new Hand();
        var a = _player.Hand;
        if (a.Cards.Count != 2) throw new InvalidOperationException("Split only allowed with 2 cards.");

        var moved = a.Cards[1];
        //Remove original second card from OG hand
        a.RemoveAt(1);
        _playerHandB.Add(moved);

        //Draw one card to each hand
        a.Add(_deck.Draw());
        _playerHandB.Add(_deck.Draw());

        ActiveHandIndex = 0;
    }

    public bool PlayerDoubleDown()
    {
        if (!PlayerHandDouble())
            throw new InvalidOperationException("Current hand cannot be doubled.");

        //Add card to current hand ONLY
        var hand = CurrentHand();
        hand.Add(_deck.Draw());

        //Play other hand
        if (HasSecondHand && ActiveHandIndex == 0)
        {
            ActiveHandIndex = 1;
            return false;
        }

        return true;
    }

    #endregion

    /**********************************/

    #region Score Functions
    //Best value of all hands -- for after a split
    private int BestValueAllHands()
    {
        if (!HasSecondHand) return _player.Hand.BestValue();

        var a = _player.Hand.BestValue();
        var b = _playerHandB!.BestValue();

        var candidates = new[] { a <= 21 ? a : 0, b <= 21 ? b : 0 };
        return candidates.Max();
    }
    //TODO: Update score
    //TODO: Post-hand "store" for rare cards

    #endregion

    #region Load From State
    //Replace existing, pre-loaded cards and deck with saved ones
    //This is called in GameStateMapper

    public void LoadFrom(IEnumerable<Card> playerCards, IEnumerable<Card> dealerCards, IEnumerable<Card> remainingDeck, int PlayerScore, IEnumerable<Card>? playerCardsB = null, int activeHandIndex = 0)
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

        //If the player has a second hand from splitting, save it and add it to their second hand
        if (playerCardsB is not null)
        {
            _playerHandB = new Hand();
            foreach (var c in playerCardsB) _playerHandB.Add(c);
        }
        else
        {
            _playerHandB = null;
        }

        ActiveHandIndex = activeHandIndex;
    }

    #endregion
}

