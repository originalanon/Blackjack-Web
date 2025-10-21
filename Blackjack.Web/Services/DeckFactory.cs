using Blackjack.Core;

namespace Blackjack.Web.Services;

public class DeckFactory
{
    public string CreateDeckText()
    {
      var deck = new Deck();      
      return deck.ToString(); 
    }
}
