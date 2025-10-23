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
    public string PlayerHand { get; private set; } = "";
    public string DealerHand { get; private set; } = "";
    public string Message { get; private set; } = "";
    //Renamed bc "Score" should mean player score, not cards value
    public int? CardScore { get; private set; }
    public int PlayerScore { get; private set; } = 0;

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

        PlayerHand = game.PlayerHandText();
        DealerHand = game.DealerHandText(revealHole: false);
        CardScore = game.PlayerHandTotal();
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

        PlayerHand = game.PlayerHandText();
        DealerHand = game.DealerHandText(false);
        CardScore  = game.PlayerHandTotal();
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

        PlayerHand = game.PlayerHandText();
        DealerHand = game.DealerHandText(revealHole: true);
        CardScore  = game.PlayerHandTotal();
        Message = outcome.ToString();


        return Page();
    }

    //TODO: Split
    //TODO: Double-down
    //TODO: Bet?

}
