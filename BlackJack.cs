// See https://aka.ms/new-console-template for more information

using blackjack;

namespace BlackJack
{
    public class Game
    {
        public static void Main(string[] args)
        {
            //Card testCard = new Card(Rank.King, Suit.Hearts);
            //Console.Out.WriteLine(testCard.ToString());

            Deck deck = new Deck();
            Console.WriteLine(deck.ToString());
        }
    }
}
