using System;
using Blackjack.Core;

namespace Blackjack.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            decimal bank = 10m;
            var gameNumber = 1;
            

            Console.WriteLine(@"                    _____   _____                /$$$$$$$  /$$                     /$$                               /$$      ");
            Console.WriteLine(@"            _____  |Q  ww| | ^ {)|              | $$__  $$| $$                    | $$                              | $$      ");
            Console.WriteLine(@"     _____ |J  ww| | ^ {(| |(.)%%| _____        | $$  \ $$| $$  /$$$$$$   /$$$$$$$| $$   /$$ /$$  /$$$$$$   /$$$$$$$| $$   /$$");
            Console.WriteLine(@"    |10 ^ || ^ {)| |(.)%%| | |%%%||A .  |       | $$$$$$$ | $$ |____  $$ /$$_____/| $$  /$$/|__/ |____  $$ /$$_____/| $$  /$$/");
            Console.WriteLine(@"    |^ ^ ^||(.)% | | |%%%| |_%%%>|| /.\ |       | $$__  $$| $$  /$$$$$$$| $$      | $$$$$$/  /$$  /$$$$$$$| $$      | $$$$$$/ ");
            Console.WriteLine(@"    |^ ^ ^|| | % | |_%%%O|        |(_._)|       | $$  \ $$| $$ /$$__  $$| $$      | $$_  $$ | $$ /$$__  $$| $$      | $$_  $$ ");
            Console.WriteLine(@"    |^ ^ ^||__%%[|                |  |  |       | $$$$$$$/| $$|  $$$$$$$|  $$$$$$$| $$ \  $$| $$|  $$$$$$$|  $$$$$$$| $$ \  $$");
            Console.WriteLine(@"    |___0I|                       |____V|       |_______/ |__/ \_______/ \_______/|__/  \__/| $$ \_______/ \_______/|__/  \__/");
            Console.WriteLine(@"                                                                                       /$$  | $$                              ");
            Console.WriteLine(@"                                                                                      |  $$$$$$/         ");
            Console.WriteLine(@"                                                                                       \______/         ");

            Console.WriteLine("You start with $10.\n");

            while (bank > 0)
            {
                Console.WriteLine($"\n--- Hand {gameNumber} ---");
                Console.WriteLine($"Current bank: ${bank}");
                Console.Write("Enter your bet (or 0 to quit): $");

                if (!decimal.TryParse(Console.ReadLine(), out decimal bet) || bet <= 0)
                    break;
                if (bet > bank)
                {
                    Console.WriteLine("You can't bet more than you have!");
                    continue;
                }

                var game = new BlackjackGame(1);
                game.DealInitial();

                PrintHands(game, revealDealer: false);

                //Player turn
                bool bust = false;
                while (true)
                {
                    Console.Write("\n(H)it or (S)tand? ");
                    var key = Console.ReadKey(true).KeyChar.ToString().ToLower();

                    if (key == "h")
                    {
                        bust = game.PlayerHit();
                        PrintHands(game, revealDealer: false);
                        if (bust)
                        {
                            Console.WriteLine("\nYou bust!");
                            break;
                        }
                    }
                    else if (key == "s")
                    {
                        break;
                    }
                }

                //Dealer turn
                var outcome = bust ? BlackjackGame.Outcome.PlayerBust : game.ResolveAfterPlayerStand();
                var payout = ComputeNet(outcome, bet);
                bank += payout;

                PrintHands(game, revealDealer: true);

                switch (outcome)
                {
                    case BlackjackGame.Outcome.PlayerBlackjack:
                        Console.WriteLine("Blackjack! You win!");
                        break;
                    case BlackjackGame.Outcome.DealerBlackjack:
                        Console.WriteLine("Dealer Blackjack! You lose.");
                        break;
                    case BlackjackGame.Outcome.PlayerBust:
                        Console.WriteLine("You busted!");
                        break;
                    case BlackjackGame.Outcome.DealerBust:
                        Console.WriteLine("Dealer busted!");
                        break;
                    case BlackjackGame.Outcome.PlayerWin:
                        Console.WriteLine("You win!");
                        break;
                    case BlackjackGame.Outcome.DealerWin:
                        Console.WriteLine("Dealer wins! You lose.");
                        break;
                    case BlackjackGame.Outcome.Push:
                        Console.WriteLine("Push! No winner.");
                        break;
                }

                Console.WriteLine($"Net: {(payout >= 0 ? "+" : "")}${payout}  | Bank: ${bank}");

                gameNumber++;
            }

            Console.WriteLine("\nGame over!");
        }

        static void PrintHands(BlackjackGame game, bool revealDealer)
        {
            Console.WriteLine($"\nDealer: {game.DealerHandText(revealDealer)}");

            Console.WriteLine($"Player: {game.PlayerHandText(1)}  (Total: {game.PlayerHandTotal()})");
        }

        static decimal ComputeNet(BlackjackGame.Outcome outcome, decimal bet)
        {
            return outcome switch
            {
                BlackjackGame.Outcome.PlayerBlackjack => bet * 1.5m,
                BlackjackGame.Outcome.PlayerWin       => bet,
                BlackjackGame.Outcome.DealerBust      => bet,
                BlackjackGame.Outcome.Push            => 0m,
                _                                     => -bet
            };
        }
    }
}
