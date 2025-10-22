using System.Text.Json;

namespace Blackjack.Web;

public static class SessionExtensions
{
    public static void SetJson<T>(this ISession session, string key, T value) => session.SetString(key, JsonSerializer.Serialize(value));

    public static T? GetJson<T>(this ISession session, string key) =>
        session.GetString(key) is { } json ?
            JsonSerializer.Deserialize<T>(json) :
        default;
}
