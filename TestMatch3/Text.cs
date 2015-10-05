using System.Collections.Generic;

namespace Match3
{
    static class Text
    {
        private static readonly Dictionary<string, Dictionary<string, string>> textResource = new Dictionary<string, Dictionary<string, string>>
        {
            { "en", new Dictionary<string, string>
            {
                {"play", "Play" },
                {"ok", "Ok" },
                {"score", "Score: "},
                {"time", "Time left: "},
                {"gameover", "Game Over" }
            }
            }
        };

        public static string GetString(string locale, string text)
        {
            string r = null;
            Dictionary<string, string> tempDict;
            if (textResource.TryGetValue(locale, out tempDict))
            {
                tempDict.TryGetValue(text, out r);
            }
            return r;
        }
    }
}
