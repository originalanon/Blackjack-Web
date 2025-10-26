using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Blackjack.Core;
using Blackjack.Web.Models;
using static Blackjack.Web.SessionExtensions;
using Blackjack.Web.Services;

namespace Blackjack.Web.Pages;

public class IndexModel : PageModel
{
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

    private const string SessionKey = "BLACKJACK_STATE";

    public void OnGet() { }


    //On post, start the game
    //TODO: This doesn't persist yet, so use session cookies/TempData to persist between games

    public IActionResult OnPostStart()
    {

        BlackjackGame game = new BlackjackGame(decks: 1);

        game.DealInitial();

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

        Message = "Game start!";

        return Page();
    }


    public IActionResult OnPostPlayerHit()
    {
        var state = HttpContext.Session.GetJson<GameState>("BLACKJACK_STATE");
        if (state == null)
        {
            Message = "No game in progress.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);
        var busted = game.PlayerHit();

        HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));

        PlayerHandText = game.CurrentPlayerHandText();
        DealerHandText = game.DealerHandText(false);

        PlayerCardScore = game.PlayerHandTotal();
        DealerCardScoreUnknown = game.DealerHandTotalUnknown();

        PlayerHandSplittable = game.PlayerHandSplittable();
        PlayerHandDouble = game.PlayerHandDouble();

        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        Message = busted ? "Bust!" : "Hit or stand?";

        return Page();
    }

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
        var outcome = game.ResolveAfterPlayerStand();

        HttpContext.Session.Remove(SessionKey);

        PlayerHandText = game.CurrentPlayerHandText();
        DealerHandText = game.DealerHandText(revealHole: true);

        PlayerCardScore = game.PlayerHandTotal();
        DealerCardScoreKnown = game.DealerHandTotalKnown();

        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;

        Message = outcome.ToString();


        return Page();
    }

    //TODO: Split
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
            Message = "You can’t split this hand.";
        }
        else
        {
            game.PlayerSplit();
            HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));
            Message = "Split! Playing Hand #1.";
        }

        RefreshView(game);
        return Page();

    }

    //TODO: Double-down
    public IActionResult OnPostPlayerDouble()
    {
        var state = HttpContext.Session.GetJson<GameState>(SessionKey);

        if (state == null)
        {
            Message = "Error! Null state.";
            return Page();
        }

        var game = GameStateMapper.FromState(state);

        if (!game.PlayerHandDouble())
        {
            Message = "You can’t double down on this hand.";
            RefreshView(game);
            return Page();
        }

        if (game.PlayerDoubleDown())
        {
            var outcome = game.ResolveAfterPlayerStand();
            HttpContext.Session.Remove(SessionKey);
            PlayerHandText = game.PlayerHandText(0);
            PlayerHandB = game.HasSecondHand ? game.PlayerHandText(1) : "";
            DealerHandText = game.DealerHandText(true);
            PlayerCardScore = game.PlayerHandValue(game.ActiveHandIndex);
            
            CurrentPlayerCards = game.CurrentPlayerCards;
            CurrentPlayerCardsB = game.CurrentPlayerCardsB;
            Message= $"Doubled down! {outcome}";
            return Page();
        }

            HttpContext.Session.SetJson(SessionKey, GameStateMapper.ToState(game));
            Message = $"Doubled. Now playing Hand #{game.ActiveHandIndex + 1}.";
            RefreshView(game);
            return Page();
    }

    private void RefreshView(BlackjackGame game)
    {
        PlayerHandText  = game.PlayerHandText(0);
        PlayerHandB = game.HasSecondHand ? game.PlayerHandText(1) : "";
        DealerHandText  = game.DealerHandText(false);
        PlayerCardScore   = game.PlayerHandValue(game.ActiveHandIndex);
        ActiveHandIndex = game.ActiveHandIndex;
        CurrentPlayerCards = game.CurrentPlayerCards;
        CurrentPlayerCardsB = game.CurrentPlayerCardsB;
    }

    //TODO: Bet?

}
