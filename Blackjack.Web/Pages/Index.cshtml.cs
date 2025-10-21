using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Blackjack.Web.Services;

namespace Blackjack.Web.Pages;

public class IndexModel : PageModel
{
    private readonly DeckFactory _factory;
    public IndexModel(DeckFactory factory) => _factory = factory;

    [BindProperty(SupportsGet = true)]

    //Get the text from the cards in the deck
    public string DeckText { get; private set; } = string.Empty;

    public void OnGet()
    {
        //Show initial deck on load
        DeckText = _factory.CreateDeckText();
    }

    //build a deck, then get the values of it -- see DeckFactory service
    public IActionResult OnPostBuild()
    {
        DeckText = _factory.CreateDeckText();
        return Page();
    }

    public async Task<IActionResult> OnPostDeal()
    {
        

        return Page();
    }
}
