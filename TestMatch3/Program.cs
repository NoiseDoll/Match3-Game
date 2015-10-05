using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Match3
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string testFile = null;
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    Match match = Regex.Match(arg, @"\-(?<argname>\w+):(?<argvalue>.+)");
                    if (match.Success && match.Groups["argname"].Value == "test" && File.Exists(match.Groups["argvalue"].Value))
                    {
                        testFile = match.Groups["argvalue"].Value;
                    }
                }
            }

            using (var game = new Match3Game())
            {
                game.TestFile = testFile;
                game.Run();
            }
        }
    }
#endif
}
