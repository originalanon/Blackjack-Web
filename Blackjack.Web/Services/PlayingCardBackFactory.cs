using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Blackjack.Web.Services;

[HtmlTargetElement("playing-card-back")]
public class PlayingCardBackTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "svg";
        output.Attributes.SetAttribute("viewBox", "0 0 200 280");
        output.Attributes.SetAttribute("class", "card back");
        output.Attributes.SetAttribute("width", "140");
        output.Attributes.SetAttribute("height", "196");

        output.Content.SetHtmlContent("""
          <rect x="2" y="2" width="196" height="276" rx="12" fill="#fff" stroke="#222" stroke-width="3"/>
          <defs>
            <pattern id="diag" width="12" height="12" patternUnits="userSpaceOnUse" patternTransform="rotate(45)">
              <rect width="12" height="12" fill="#1f3a5f"/>
              <rect width="12" height="2" y="0" fill="#efefef" opacity=".25"/>
            </pattern>
          </defs>
          <rect x="10" y="10" width="180" height="260" rx="10" fill="url(#diag)" stroke="#0f2540" stroke-width="2"/>
        """);
    }
}
