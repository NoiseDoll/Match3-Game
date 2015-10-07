using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Gui
{
    class TextureAtlas
    {
        public Texture2D Texture { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }

        public TextureAtlas(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Columns = columns;
            Rows = rows;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, int row, int column, float opacity)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

            spriteBatch.Draw(Texture, location, sourceRectangle, new Color(Color.White, opacity));
        }

    }
}
