using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Gui
{
    class GuiLabel : GuiElement
    {
        public string Text { get; private set; }
        public SpriteFont TextFont { get; private set; }
        public Color TextColor { get; set; }

        public GuiLabel(string text, SpriteFont font) : this(text, font, Color.Black) { }

        public GuiLabel(string text, SpriteFont font, Color color)
        {
            TextFont = font;
            SetText(text);
            TextColor = color;
        }

        public GuiLabel(string text, SpriteFont font, Color color, Point position) : this(text, font, color)
        {
            SetRelativePosition(position);
        }

        public void SetText(string text)
        {
            Text = text;
            Vector2 newSize = TextFont.MeasureString(text);
            SetSize(new Point((int)newSize.X, (int)newSize.Y));
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(TextFont, Text, GetAbsolutePosition(), TextColor);
        }
    }
}
