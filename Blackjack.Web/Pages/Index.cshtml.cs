using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Blackjack.Core; 

namespace Blackjack.Web.Pages;

public class IndexModel : PageModel
{
    // Values to render
    public string PlayerHand { get; private set; } = "";
    public string DealerHand { get; private set; } = "";
    public string Message { get; private set; } = "";
    public string Score { get; private set; } = "";

    public BlackjackGame game = new BlackjackGame(decks: 1);

    public void OnGet() { }


    //On post, start the game
    //TODO: This doesn't persist yet, so use session cookies/TempData to persist between games
    public IActionResult OnPostStart()
    {
        game.DealInitial();

        PlayerHand = game.PlayerHandText();
        DealerHand = game.DealerHandText(revealHole: false);
        Score = game.PlayerHandTotal().ToString();
        Message = "Game start!";

        return Page();
    }


    public IActionResult OnPostPlayerHit()
    {
        game.PlayerHit();
        return Page();
    }

    //TODO: Player Stand should reveal the Dealer's hand
    public IActionResult OnPostPlayerStand()
    {
        return Page();
    }

    //TODO: Split
    //TODO: Double-down
    //TODO: Bet?

}
