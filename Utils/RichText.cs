namespace CommunityServerAPI.Utils
{
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
        public const string MediumVioletRed = "<color=#C71585>";
        public const string DeepPink = "<color=#FF1493>";
        public const string PaleVioletRed = "<color=#DB7093>";
        public const string HotPink = "<color=#FF69B4>";
        public const string LightPink = "<color=#FFB6C1>";
        public const string Pink = "<color=#FFC0CB>";
        public const string DarkRed = "<color=#8B0000>";
        public const string Firebrick = "<color=#B22222>";
        public const string Crimson = "<color=#DC143C>";
        public const string IndianRed = "<color=#CD5C5C>";
        public const string LightCoral = "<color=#F08080>";
        public const string Salmon = "<color=#FA8072>";
        public const string DarkSalmon = "<color=#E9967A>";
        public const string LightSalmon = "<color=#FFA07A>";
        public const string OrangeRed = "<color=#FF4500>";
        public const string Tomato = "<color=#FF6347>";
        public const string DarkOrange = "<color=#FF8C00>";
        public const string Coral = "<color=#FF7F50>";
        public const string DarkKhaki = "<color=#BDB76B>";
        public const string Gold = "<color=#FFD700>";
        public const string Khaki = "<color=#F0E68C>";
        public const string PeachPuff = "<color=#FFDAB9>";
        public const string PaleGoldenrod = "<color=#EEE8AA>";
        public const string Moccasin = "<color=#FFE4B5>";
        public const string PapayaWhip = "<color=#FFEFD5>";
        public const string LightGoldenrodYellow = "<color=#FAFAD2>";
        public const string LemonChiffon = "<color=#FFFACD>";
        public const string LightYellow = "<color=#FFFFE0>";
        public const string SaddleBrown = "<color=#8B4513>";
        public const string Sienna = "<color=#A0522D>";
        public const string Chocolate = "<color=#D2691E>";
        public const string DarkGoldenrod = "<color=#B8860B>";
        public const string Peru = "<color=#CD853F>";
        public const string RosyBrown = "<color=#BC8F8F>";
        public const string Goldenrod = "<color=#DAA520>";
        public const string SandyBrown = "<color=#F4A460>";
        public const string Tan = "<color=#D2B48C>";
        public const string Burlywood = "<color=#DEB887>";
        public const string Wheat = "<color=#F5DEB3>";
        public const string NavajoWhite = "<color=#FFDEAD>";
        public const string Bisque = "<color=#FFE4C4>";
        public const string BlanchedAlmond = "<color=#FFEBCD>";
        public const string Cornsilk = "<color=#FFF8DC>";
        public const string Indigo = "<color=#4B0082>";
        public const string DarkMagenta = "<color=#8B008B>";
        public const string DarkViolet = "<color=#9400D3>";
        public const string DarkSlateBlue = "<color=#483D8B>";
        public const string BlueViolet = "<color=#8A2BE2>";
        public const string DarkOrchid = "<color=#9932CC>";
        public const string SlateBlue = "<color=#6A5ACD>";
        public const string MediumSlateBlue = "<color=#7B68EE>";
        public const string MediumOrchid = "<color=#BA55D3>";
        public const string MediumPurple = "<color=#9370DB>";
        public const string Orchid = "<color=#DA70D6>";
        public const string Violet = "<color=#EE82EE>";
        public const string Plum = "<color=#DDA0DD>";
        public const string Thistle = "<color=#D8BFD8>";
        public const string Lavender = "<color=#E6E6FA>";
        public const string MidnightBlue = "<color=#191970>";
        public const string MediumBlue = "<color=#0000CD>";
        public const string RoyalBlue = "<color=#4169E1>";
        public const string SteelBlue = "<color=#4682B4>";
        public const string DodgerBlue = "<color=#1E90FF>";
        public const string DeepSkyBlue = "<color=#00BFFF>";
        public const string CornflowerBlue = "<color=#6495ED>";
        public const string SkyBlue = "<color=#87CEEB>";
        public const string LightSkyBlue = "<color=#87CEFA>";
        public const string LightSteelBlue = "<color=#B0C4DE>";
        public const string PowderBlue = "<color=#B0E0E6>";
        public const string DarkCyan = "<color=#008B8B>";
        public const string LightSeaGreen = "<color=#20B2AA>";
        public const string CadetBlue = "<color=#5F9EA0>";
        public const string DarkTurquoise = "<color=#00CED1>";
        public const string MediumTurquoise = "<color=#48D1CC>";
        public const string Turquoise = "<color=#40E0D0>";
        public const string Aquamarine = "<color=#7FFFD4>";
        public const string PaleTurquoise = "<color=#AFEEEE>";
        public const string LightCyan = "<color=#E0FFFF>";
        public const string DarkGreen = "<color=#006400>";
        public const string DarkOliveGreen = "<color=#556B2F>";
        public const string ForestGreen = "<color=#228B22>";
        public const string SeaGreen = "<color=#2E8B57>";
        public const string OliveDrab = "<color=#6B8E23>";
        public const string MediumSeaGreen = "<color=#3CB371>";
        public const string LimeGreen = "<color=#32CD32>";
        public const string SpringGreen = "<color=#00FF7F>";
        public const string MediumSpringGreen = "<color=#00FA9A>";
        public const string DarkSeaGreen = "<color=#8FBC8F>";
        public const string MediumAquamarine = "<color=#66CDAA>";
        public const string YellowGreen = "<color=#9ACD32>";
        public const string LawnGreen = "<color=#7CFC00>";
        public const string Chartreuse = "<color=#7FFF00>";
        public const string LightGreen = "<color=#90EE90>";
        public const string GreenYellow = "<color=#ADFF2F>";
        public const string PaleGreen = "<color=#98FB98>";
        public const string MistyRose = "<color=#FFE4E1>";
        public const string AntiqueWhite = "<color=#FAEBD7>";
        public const string Linen = "<color=#FAF0E6>";
        public const string Beige = "<color=#F5F5DC>";
        public const string WhiteSmoke = "<color=#F5F5F5>";
        public const string LavenderBlush = "<color=#FFF0F5>";
        public const string OldLace = "<color=#FDF5E6>";
        public const string AliceBlue = "<color=#F0F8FF>";
        public const string Seashell = "<color=#FFF5EE>";
        public const string GhostWhite = "<color=#F8F8FF>";
        public const string Honeydew = "<color=#F0FFF0>";
        public const string FloralWhite = "<color=#FFFAF0>";
        public const string Azure = "<color=#F0FFFF>";
        public const string MintCream = "<color=#F5FFFA>";
        public const string Snow = "<color=#FFFAFA>";
        public const string Ivory = "<color=#FFFFF0>";
        public const string DarkSlateGray = "<color=#2F4F4F>";
        public const string DimGray = "<color=#696969>";
        public const string SlateGray = "<color=#708090>";
        public const string Gray = "<color=#808080>";
        public const string LightSlateGray = "<color=#778899>";
        public const string DarkGray = "<color=#A9A9A9>";
        public const string LightGray = "<color=#D3D3D3>";
        public const string Gainsboro = "<color=#DCDCDC>";

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

        public const string BR = "<br>";
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
}