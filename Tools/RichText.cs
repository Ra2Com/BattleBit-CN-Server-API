namespace CommunityServerAPI.Tools;

public static class RichText
{
    // https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html#supported-colors
    // http://digitalnativestudios.com/textmeshpro/docs/rich-text/
    // 增加了一些 Unity 游戏内自带的 Emoji 和格式 2023年8月16日 22:52:33

    public const string Aqua = "<color=#00ffff>";
    public const string Black = "<color=#000000>";
    public const string Blue = "<color=#0000ff>";
    public const string Brown = "<color=#a52a2a>";
    public const string Cyan = "<color=#00ffff>";
    public const string DarkBlue = "<color=#0000a0>";
    public const string Fuchsia = "<color=#ff00ff>";
    public const string Green = "<color=#008000>";
    public const string Grey = "<color=#808080>";
    public const string LightBlue = "<color=#add8e6>";
    public const string Lime = "<color=#00ff00>";
    public const string Magenta = "<color=#ff00ff>";
    public const string Maroon = "<color=#800000>";
    public const string Navy = "<color=#000080>";
    public const string Olive = "<color=#808000>";
    public const string Orange = "<color=#ffa500>";
    public const string Purple = "<color=#800080>";
    public const string Red = "<color=#ff0000>";
    public const string Silver = "<color=#c0c0c0>";
    public const string Teal = "<color=#008080>";
    public const string White = "<color=#ffffff>";
    public const string Yellow = "<color=#ffff00>";

    //icons
    public const string Moderator = "<sprite index=0>";
    public const string Patreon = "<sprite index=1>";
    public const string Creator = "<sprite index=2>";
    public const string DiscordBooster = "<sprite index=3>";
    public const string Special = "<sprite index=4>";
    public const string PatreonFirebacker = "<sprite index=5>";
    public const string Vip = "<sprite index=6>";
    public const string Supporter = "<sprite index=7>";
    public const string Developer = "<sprite index=8>";
    public const string Veteran = "<sprite index=9>";
    public const string Misc1 = "<sprite index=10>";
    public const string Misc2 = "<sprite index=11>";
    public const string Misc3 = "<sprite index=12>";
    public const string Misc4 = "<sprite index=13>";
    public const string Misc5 = "<sprite index=14>";
    public const string Misc6 = "<sprite index=15>";

    //emojis
    public const string Blush = "<sprite=\"EmojiOne\" index=0>";
    public const string Yum = "<sprite=\"EmojiOne\" index=1>";
    public const string HeartEyes = "<sprite=\"EmojiOne\" index=2>";
    public const string Sunglasses = "<sprite=\"EmojiOne\" index=3>";
    public const string Grinning = "<sprite=\"EmojiOne\" index=4>";
    public const string Smile = "<sprite=\"EmojiOne\" index=5>";
    public const string Joy = "<sprite=\"EmojiOne\" index=6>";
    public const string Smiley = "<sprite=\"EmojiOne\" index=7>";
    public const string Grin = "<sprite=\"EmojiOne\" index=8>";
    public const string SweatSmile = "<sprite=\"EmojiOne\" index=9>";
    public const string Tired = "<sprite=\"EmojiOne\" index=10>";
    public const string TongueOutWink = "<sprite=\"EmojiOne\" index=11>";
    public const string Kiss = "<sprite=\"EmojiOne\" index=12>";
    public const string Rofl = "<sprite=\"EmojiOne\" index=13>";
    public const string SlightSmile = "<sprite=\"EmojiOne\" index=14>";
    public const string SlightFrown = "<sprite=\"EmojiOne\" index=15>";

    public const string LineBreak = "<br>";
    public const string EndColor = "</color>";

    public static string Bold(string text)
    {
        return $"<b>{text}</b>";
    }

    public static string Italic(string text)
    {
        return $"<i>{text}</i>";
    }

    public static string Underline(string text)
    {
        return $"<u>{text}</u>";
    }

    public static string Strike(string text)
    {
        return $"<s>{text}</s>";
    }

    public static string SuperScript(string text)
    {
        return $"<sup>{text}</sup>";
    }

    public static string SubScript(string text)
    {
        return $"<sub>{text}</sub>";
    }

    public static string StyleH1(string text)
    {
        return $"<style=\"H1\">{text}</style>";
    }

    public static string StyleH2(string text)
    {
        return $"<style=\"H2\">{text}</style>";
    }

    public static string StyleH3(string text)
    {
        return $"<style=\"H3\">{text}</style>";
    }

    public static string StyleC1(string text)
    {
        return $"<style=\"C1\">{text}</style>";
    }

    public static string StyleC2(string text)
    {
        return $"<style=\"C2\">{text}</style>";
    }

    public static string StyleC3(string text)
    {
        return $"<style=\"C3\">{text}</style>";
    }

    public static string StyleNormal(string text)
    {
        return $"<style=\"Normal\">{text}</style>";
    }

    public static string StyleTitle(string text)
    {
        return $"<style=\"Title\">{text}</style>";
    }

    public static string StyleQuote(string text)
    {
        return $"<style=\"Quote\">{text}</style>";
    }

    public static string StyleLink(string text)
    {
        return $"<style=\"Link\">{text}</style>";
    }

    public static string Highlight(string text, string color)
    {
        return $"<mark={color}>{text}</mark>";
    }

    public static string VerticalOffset(string text, float amount)
    {
        return $"<voffset={amount}em>{text}</voffset>";
    }
    public static string Size(string text, int sizeValue)
    {
        return $"<size={sizeValue}>{text}</size>";
    }

}