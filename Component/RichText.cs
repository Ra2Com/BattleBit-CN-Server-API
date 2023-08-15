namespace CommunityServerAPI;

public static class RichText
{
    // Colors
    // https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html#supported-colors
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

    public static string Size(string text, int sizeValue)
    {
        return $"<size={sizeValue}>{text}</size>";
    }
}