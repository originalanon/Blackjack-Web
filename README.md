# Praeses Blackjack
This project was a week-long homework assignment by Praeses, in which I needed to create a Blackjack program in C#/ASP.NET. This is what I came up with -- I may have went slightly overboard.

I used Razor Pages for the UI, and I've included a UML diagram of the main classes below. To start the UI  version, run:
>dotnet run --project .\Blackjack.Web

I've also created a console-only version, just in case I did, in fact, get a bit too eager with the UI. You can run that with:

>dotnet run --project .\Blackjack.ConsoleApp

# Different Cards
I was inspired by Balatro for this project, so I wanted to include different types of cards that would give the player different score multipliers. There are six types of card materials, which give bonuses to a card's base score (the card's rank):

*Standard  =  +0
Stone  =  +3
Silver  =  +5
Gold  =  +7
Platinum  =  +10
Ethereum  =  +15*

There's also five different "coats" cards can have, which improve their base multiplier:

*Standard  =  1
Foil  =  2
Holographic  =  3
Prismatic  =  4
Specular  =  5*

If these effects are hard to see in the UI, simply hover over the card and you'll see a tooltip!

# Design

![UML Diagram](https://github.com/originalanon/Blackjack-Web/blob/master/Design/Blackjack.png)


# Credit

All code is by me, written in VSCode. I used C# Snippets and Extensions for some auto-complete features and suggestions. 

Cool card CSS inspired by: https://github.com/simeydotme/pokemon-cards-css
