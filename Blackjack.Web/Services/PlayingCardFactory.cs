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


        var sb = new StringBuilder();
        sb.Append("""
          <rect x="2" y="2" width="196" height="276" rx="12" class="card-bg"/>
        """);

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

        if (Rank is Rank.Jack or Rank.Queen or Rank.King)
        {
            sb.Append("""<text x="100" y="160" text-anchor="middle" class="face">""" + rankText[0] + "</text>");
        }

        output.Content.SetHtmlContent(sb.ToString());

        //TODO: Place icons on cards
        //TODO: Change materials and coats
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
        _ => ((int)r).ToString()
    };
}
