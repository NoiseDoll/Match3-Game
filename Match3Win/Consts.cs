using Microsoft.Xna.Framework;

namespace Match3
{
    static class Consts
    {
        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HIEGHT = 600;
        public const int SCREEN_HORIZ_CENTER = SCREEN_WIDTH / 2;
        public const int SCREEN_VERT_CENTER = SCREEN_HIEGHT / 2;

        public static readonly Color BUTTON_NORMAL = new Color(0, 230, 0);
        public static readonly Color BUTTON_HOVER = new Color(0, 200, 0);
        public static readonly Color BUTTON_PRESSED = Color.DarkGreen;
        public static readonly Color BACKGROUND = Color.DarkSlateGray;

        public const int MATCH_MIN = 3;
        public const int MATCH_LINE = 4;
        public const int MATCH_BOMB = 5;

        public const int SCORE_BONUS = 10;
        public const double GAME_TIME = 60f;

        public const int RESOURCE_FONT = 0;
    }
}
