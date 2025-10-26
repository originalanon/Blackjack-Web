//Class to build the playing card UIs using SVG
//TODO: Document
using System;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Blackjack.Core;

namespace Blackjack.Web.Services;

[HtmlTargetElement("playing-card")]
public class PlayingCardTagHelper : TagHelper
{
    [HtmlAttributeName("rank")] public Rank Rank { get; set; }
    [HtmlAttributeName("suit")] public Suit Suit { get; set; }


  public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "svg";
        output.Attributes.SetAttribute("viewBox", "0 0 200 280");
        output.Attributes.SetAttribute("class", $"card {(IsRed(Suit) ? "red" : "black")}");

        output.Attributes.SetAttribute("width", "140");
        output.Attributes.SetAttribute("height", "196");
        
        
        
        var pips = PipLayout(Rank);

        var sb = new StringBuilder();
        sb.Append("""<rect x="2" y="2" width="196" height="276" rx="12" fill="#fff" stroke="#222" stroke-width="3"/>""");

        // corner indices
        var rankText = RankToText(Rank);
        var suitGlyph = SuitToGlyph(Suit);
        sb.Append($$"""
          <text x="16" y="26" class="idx">{{rankText}}</text>
          <text x="16" y="46" class="suit">{{suitGlyph}}</text>

            <g transform="rotate(180 100 140)">
              <text x="16" y="26" class="idx">{{rankText}}</text>
              <text x="16" y="46" class="suit">{{suitGlyph}}</text>
            </g>
          """);

        // Add the pips
        foreach (var (x, y, flip) in pips)
        {
            if (flip)
                sb.Append($$"""<text x="{{x}}" y="{{y}}" class="pip" transform="rotate(180 {{x}} {{y}})">{{suitGlyph}}</text>""");
            else
                sb.Append($$"""<text x="{{x}}" y="{{y}}" class="pip">{{suitGlyph}}</text>""");
        }

        if (Rank is Rank.Jack or Rank.Queen or Rank.King)
        {
            sb.Append("""<text x="100" y="160" text-anchor="middle" class="face">""" + rankText[0] + "</text>");
        }

        output.Content.SetHtmlContent(sb.ToString());
    }

    private static bool IsRed(Suit s) => s is Suit.Hearts or Suit.Diamonds;
    private static string SuitToGlyph(Suit s) => s switch
    {
        Suit.Spades => "♠",
        Suit.Hearts => "♥",
        Suit.Diamonds => "♦",
        Suit.Clubs => "♣",
        _ => "?"
    };
    private static string RankToText(Rank r) => r switch
    {
        Rank.Ace => "A",
        Rank.Jack => "J",
        Rank.Queen => "Q",
        Rank.King => "K",
        _ => r.GetValue().ToString()
    };

    private static (int x, int y, bool flip)[] PipLayout(Rank rank)
    {

        int midX = 100;
        int leftX = 60, rightX = 140;
        int topY = 70, midY = 140, botY = 210;

        return rank switch
        {

          Rank.Ace => [(midX, midY, false)],

          Rank.Two => [(midX, topY, false), (midX, botY, true)],

          Rank.Three => [(midX, topY, false), (midX, midY, false), (midX, botY, true)],

          Rank.Four => [ (leftX, topY, false), (rightX, topY, false),
                         (leftX, botY, true),  (rightX, botY, true) ],

          /**********
          * God bless ASCII art
          *   _____
          *  |5    |
          *  | v v |
          *  |  v  |
          *  | v v |
          *  |____S|
          */
          Rank.Five => [ (leftX, topY, false), (rightX, topY, false),
                                  (midX, midY, false),
                         (leftX, botY, true),  (rightX, botY, true) ],

          Rank.Six => [ (leftX, topY, false), (rightX, topY, false),
                        (leftX, midY, false), (rightX, midY, false),
                        (leftX, botY, true),  (rightX, botY, true) ],


          Rank.Seven => [(leftX, topY, false),                     (rightX, topY, false),
                         (leftX, midY, false), (midX, 50, false), (rightX, midY, false),
                         (leftX, botY, true),                      (rightX, botY, true) ],

          Rank.Eight => [ (leftX, 50, false),   (rightX, 50, false),
                          (leftX, topY, false), (rightX, topY, false),
                          (leftX, botY, true),  (rightX, botY, true),
                          (leftX, 230, true),   (rightX, 230, true) ],

          //TODO: Fix Ten
          Rank.Nine => [          (leftX, 50, false),                      (rightX, 50, false),
                                  (leftX, topY, false), (midX, midY, false), (rightX, topY, false),
                                  (leftX, botY, true),                     (rightX, botY, true),
                                  (leftX, 230, true),                      (rightX, 230, true) ],

          Rank.Ten => [           (leftX, 50, false),                      (rightX, 50, false),
                                  (leftX, topY, false), (midX, 50, false), (rightX, topY, false),
                                  (leftX, botY, true),                     (rightX, botY, true),
                                  (leftX, 230, true),                      (rightX, 230, true)

                      ],

          //TODO: Display face cards
          Rank.Jack or Rank.Queen or Rank.King => Array.Empty<(int, int, bool)>(),

          _ => Array.Empty<(int, int, bool)>()
        };
    }
}
