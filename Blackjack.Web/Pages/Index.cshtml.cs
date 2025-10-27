/**
 * @ Author: Lindsay Barton
 * @ Description: This is the main page model for the game. It contains functions for starting the game,
 *   hitting, standing, splitting, and doubling down. 
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Blackjack.Core;
using Blackjack.Web.Models;
using static Blackjack.Web.SessionExtensions;
using Blackjack.Web.Services;

namespace Blackjack.Web.Pages;

public class IndexModel : PageModel
{

    #region Page Model Values
    // Values to render

    //String value of player's cards
    public string PlayerHandText { get; private set; } = "";

    //String value of player's SECOND hand of cards, if they split
    public string PlayerHandB { get; private set; } = "";

    //String value of Dealer's hand
    public string DealerHandText { get; private set; } = "";
    public string Message { get; private set; } = "";
    //Renamed bc "Score" should mean player score, not cards value
    public int? PlayerCardScore { get; private set; }

    //Mostly for debug to see the dealer's score (not counting the hole card)
    public int? DealerCardScoreKnown { get; private set; } = 0;
    public int? DealerCardScoreUnknown { get; private set; } = 0;

    //Player's total score -- not the score of their hand
    public int PlayerScore { get; private set; } = 0;

    public bool PlayerHandDouble { get; private set; } = false;
    public bool PlayerHandSplittable { get; private set; } = false;

    //Player's active hand index (for splitting)
    public int ActiveHandIndex { get; private set; }

    public IReadOnlyList<Card> CurrentPlayerCards { get; private set; } = Array.Empty<Card>();
    public IReadOnlyList<Card> CurrentPlayerCardsB { get; private set; } = Array.Empty<Card>();

    public IReadOnlyList<Card> DealerCards { get; private set; } = Array.Empty<Card>();

    public bool RevealDealerHole { get; private set; }

    #region State Keys
    private const string SessionKey = "BLACKJACK_STATE";
    private const string BankKey = "BANK_STATE";

private const string ScoreKey = "BLACKJACK_SCORE";

public int PlayerTotalScore { get; private set; }


    #endregion

    #region Ui State
    public enum UiState { NotStarted, PreDeal, InHand, PostHand }

    private const string UiKey = "UI_STATE";
    public UiState Ui { get; private set; } = UiState.NotStarted;

    private UiState GetUi()
    {
        var s = HttpContext.Session.GetString(UiKey);
        return Enum.TryParse<UiState>(s, out var ui) ? ui : UiState.NotStarted;
    }
    private void SetUi(UiState ui)
    {
        Ui = ui;
        HttpContext.Session.SetString(UiKey, ui.ToString());
    }
    #endregion

    public decimal Bank { get; private set; }
    public bool IsBroke => Bank <= 0m;

    [BindProperty(SupportsGet = false)]
    public decimal Bet { get; set; }

#endregion

#region OnGet
    public void OnGet() {
        StartBank();
        Ui = GetUi();
        LoadScore();
    }

    #endregion

#region StartBank
    //Create Bank for this session
    private BankState StartBank()
    {
        var bank = HttpContext.Session.GetJson<BankState>(BankKey);
        if (bank is null)
        {
            bank = new BankState();
            HttpContext.Session.SetJson(BankKey, bank);
        }

        Bank = bank.Bank;
        return bank;
    }

    //On post, start the game
    #endregion
    #region OnPostStart (Start game)

    public IActionResult OnPostStart()
    {

        BlackjackGame game = new BlackjackGame(decks: 1);

        var bank = StartBank();
        LoadScore();

        //Validate bet
        if (Bet <= 0)
        {
            Message = "Please enter a positive bet.";
            return Page();
        }
        if (Bet > bank.Bank)
        {
            Message = $"Insufficient funds. Bank: ${bank.Bank}";
            return Page();
        }

        //Lock bet
        bank.CurrentBet = Bet;
        HttpContext.Session.SetJson(BankKey, bank);

        //Start the game
        game.DealInitial();

        //Check for natural blackjack right after deal
        if (game.PlayerHandTotal() == 21)
        {
            SetUi(UiState.PostHand);
            LoadScore();

            var outcome = BlackjackGame.Outcome.PlayerBlackjack;
            var net = BettingService.NetPayout(outcome, bank.CurrentBet);
            bank.Bank += net;
            bank.CurrentBet = 0m;
            HttpContext.Session.SetJson(BankKey, bank);

            HttpContext.Session.Remove(SessionKey); // round over

            AddToScore(game.ScoreForPlayerHands());

            PlayerHandText = game.CurrentPlayerHandText();
            DealerHandText = game.DealerHandText(true);
            PlayerCardScore = game.PlayerHandTotal();
            DealerCardScoreKnown = game.DealerHandTotalKnown();
            Bank = bank.Bank;

            CurrentPlayerCards = game.CurrentPlayerCards;
            CurrentPlayerCardsB = game.CurrentPlayerCardsB;

            DealerCards = game.DealerCards;

            RevealDealerHole = true; 

            Message = $"Blackjack! You win ${net}. Bank: ${bank.Bank}";
            return Page();
        }
        else
        {
            SetUi(UiState.InHand);
            LoadScore();
        }

        GameState state = GameStateMapper.ToState(game);

        //Set game state on start
        HttpContext.Session.SetJson(SessionKey, state);

        PlayerHandText = game.CurrentPlayerHandText();
        DealerHandText = game.DealerHandText(revealHole: false);

        PlayerCardScore = game.PlayerHandTotal();
        DealerCardScoreUnknown = game.DealerHandTotalUnknown();

        PlayerHandSplittable = game.PlayerHandSplittable();
        PlayerHandDouble = game.PlayerHandDouble();

        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        DealerCards = game.DealerCards;
        RevealDealerHole = false; 

        Bank = bank.Bank;
        Message = $"Bet locked: ${bank.CurrentBet}";

        return Page();
    }

    #endregion

    #region Player Hit
    public IActionResult OnPostPlayerHit()
    {
        var state = HttpContext.Session.GetJson<GameState>("BLACKJACK_STATE");
        if (state == null)
        {
            Message = "No game in progress.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);
        var bank = StartBank();

        var busted = game.PlayerHit();

        if (busted)
        {

            PlayerHandText = game.CurrentPlayerHandText();
            DealerHandText = game.DealerHandText(false);

            PlayerCardScore = game.PlayerHandTotal();
            DealerCardScoreUnknown = game.DealerHandTotalUnknown();

            PlayerHandSplittable = game.PlayerHandSplittable();
            PlayerHandDouble = game.PlayerHandDouble();

            DealerCards = game.DealerCards;
            RevealDealerHole = true;

            CurrentPlayerCards = game.CurrentPlayerCards;
            CurrentPlayerCardsB = game.CurrentPlayerCardsB;

            //Player loses bet
            bank = StartBank();
            var net = -bank.CurrentBet;
            bank.Bank += net;

            bank.CurrentBet = 0m;
            HttpContext.Session.SetJson(BankKey, bank);

            Bank = bank.Bank;

            SetUi(UiState.PostHand);
            LoadScore();

            //Clear the round
            HttpContext.Session.Remove(SessionKey);

            Message = $"Bust! You lost ${Math.Abs(net)}.";
            if (bank.Bank <= 0)
            {
                Message += " You're out of money!";
            }


            return Page();
        }
        else
        {
            SetUi(UiState.InHand);
            LoadScore();
        }
        
        HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));

        PlayerHandText = game.CurrentPlayerHandText();
        DealerHandText = game.DealerHandText(false);

        PlayerCardScore = game.PlayerHandTotal();
        DealerCardScoreUnknown = game.DealerHandTotalUnknown();

        PlayerHandSplittable = game.PlayerHandSplittable();
        PlayerHandDouble = game.PlayerHandDouble();

        DealerCards = game.DealerCards;
        RevealDealerHole = false;

        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        Bank = bank.Bank;
        Message = busted ? "Bust!" : "Hit or stand?";

        return Page();
    }

    #endregion
    #region  Player Stand
    public IActionResult OnPostPlayerStand()
    {
        var state = HttpContext.Session.GetJson<GameState>(SessionKey);
        //Adding this so VS Code stops yelling, but it shouldn't be possible to try to stand if there's no game going -- the button
        //to Stand is locked behind if the card total is less than 21 or not
        if (state == null)
        {
            Message = "Error! Null state.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);
        var bank = StartBank();

        var outcome = game.ResolveAfterPlayerStand();
        SetUi(UiState.PostHand);
        LoadScore();

        if (outcome is BlackjackGame.Outcome.PlayerBlackjack or BlackjackGame.Outcome.DealerBust or BlackjackGame.Outcome.PlayerWin)
        {
            var gained = game.ScoreForPlayerHands();
            AddToScore(gained);
        }

        var net = BettingService.NetPayout(outcome, bank.CurrentBet);
        bank.Bank += net;

        //Clear bank
        bank.CurrentBet = 0m;

        //Set bank key
        HttpContext.Session.SetJson(BankKey, bank);

        //Remove session key
        HttpContext.Session.Remove(SessionKey);

        PlayerHandText = game.CurrentPlayerHandText();
        DealerHandText = game.DealerHandText(revealHole: true);

        PlayerCardScore = game.PlayerHandTotal();
        DealerCardScoreKnown = game.DealerHandTotalKnown();

        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        DealerCards = game.DealerCards;
        RevealDealerHole = true;

        Bank = bank.Bank;
        Message = $"{outcome} | Net: {(net >= 0 ? "+" : "")}${net}";

        return Page();
    }

    #endregion
    #region Player Splitt
    public IActionResult OnPostPlayerSplit()
    {
        var state = HttpContext.Session.GetJson<GameState>(SessionKey);
        if (state == null)
        {
            Message = "Error! Null state.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);

        if (!game.PlayerHandSplittable())
        {
            Message = "You can't split this hand.";
        }
        else
        {
            game.PlayerSplit();
            HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));
            Message = "Split! Playing Hand #1.";

            SetUi(UiState.InHand);
            LoadScore();

            var bank = StartBank();

            var outcome = game.ResolveAfterPlayerStand();
            SetUi(UiState.PostHand);

            if (outcome is BlackjackGame.Outcome.PlayerBlackjack or BlackjackGame.Outcome.DealerBust or BlackjackGame.Outcome.PlayerWin)
            {
                AddToScore(game.ScoreForPlayerHands());
            }

            var net = BettingService.NetPayout(outcome, bank.CurrentBet);
            bank.Bank += net;

            //Clear bank
            bank.CurrentBet = 0m;

            //Set bank key
            HttpContext.Session.SetJson(BankKey, bank);

            PlayerHandText = game.CurrentPlayerHandText();
            DealerHandText = game.DealerHandText(revealHole: true);

            PlayerCardScore = game.PlayerHandTotal();
            DealerCardScoreKnown = game.DealerHandTotalKnown();

            CurrentPlayerCards = game.CurrentPlayerCards;
            CurrentPlayerCardsB = game.CurrentPlayerCardsB;

            DealerCards = game.DealerCards;
            RevealDealerHole = true;

            Bank = bank.Bank;

            //Remove session key
            HttpContext.Session.Remove(SessionKey);
        }

        RefreshView(game);
        return Page();

    }

    #endregion
    #region Player Double Down
  public IActionResult OnPostPlayerDouble()
    {
        var state = HttpContext.Session.GetJson<GameState>(SessionKey);
        if (state == null)
        {
            Message = "Error! Null state.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);
        var bank = StartBank();

        if (!game.PlayerHandDouble())
        {
            Message = "You can't double down on this hand.";
            RefreshView(game);
            return Page();
        }

        //Double da bet
        bank.CurrentBet *= 2;

        // the player draws one card and stands
        if (game.PlayerDoubleDown())
        {
            var outcome = game.ResolveAfterPlayerStand();

            PlayerHandText = game.PlayerHandText(0);
            PlayerHandB = game.HasSecondHand ? game.PlayerHandText(1) : "";
            DealerHandText = game.DealerHandText(true);
            PlayerCardScore = game.PlayerHandValue(game.ActiveHandIndex);
            
            //Double da money
            var net = BettingService.NetPayout(outcome, bank.CurrentBet);
            bank.Bank += net;
            bank.CurrentBet = 0;
            HttpContext.Session.SetJson(BankKey, bank);

            CurrentPlayerCards = game.CurrentPlayerCards;
            CurrentPlayerCardsB = game.CurrentPlayerCardsB;

            DealerCards = game.DealerCards;

            //No blackjack after doibling down -- just a player win
            if (outcome == BlackjackGame.Outcome.PlayerBlackjack && game.PlayerHandValue(0) > 2)
                outcome = BlackjackGame.Outcome.PlayerWin;

            
            Message = $"Doubled down! {outcome} | Net: {(net >= 0 ? "+" : "")}${net}";
            Bank = bank.Bank;

            HttpContext.Session.Remove(SessionKey);
            return Page();
        }

        HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));
        Message = $"Doubled.";
        RefreshView(game);
        return Page();
    }

    #endregion

    public IActionResult OnPostNewRun()
    {
        //Wipe Game, start new run
        HttpContext.Session.Remove(SessionKey);

        var bank = new BankState { Bank = 10m, CurrentBet = 0m };
        HttpContext.Session.SetJson(BankKey, bank);
        Bank = bank.Bank;

        CurrentPlayerCards = [];
        CurrentPlayerCardsB = [];
        DealerCards = [];
        RevealDealerHole = false;

        PlayerHandText = "";
        DealerHandText = "";
        PlayerCardScore = null;
        Message = "New run started. You have $10.";
        SetUi(UiState.PreDeal);
        LoadScore();

        return Page();
    }
    
    private void LoadScore()
    {
        PlayerTotalScore = HttpContext.Session.GetInt32(ScoreKey) ?? 0;
    }

    private void AddToScore(int amount)
    {
        var current = HttpContext.Session.GetInt32(ScoreKey) ?? 0;
        current += amount;
        HttpContext.Session.SetInt32(ScoreKey, current);
        PlayerTotalScore = current;
    }
    
    #region RefreshView
    private void RefreshView(BlackjackGame game)
    {
        PlayerHandText = game.PlayerHandText(0);
        PlayerHandB = game.HasSecondHand ? game.PlayerHandText(1) : "";
        DealerHandText = game.DealerHandText(false);
        
        PlayerCardScore = game.PlayerHandValue(game.ActiveHandIndex);
        ActiveHandIndex = game.ActiveHandIndex;
        
        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        DealerCards = game.DealerCards;
        
    }

    #endregion

}
