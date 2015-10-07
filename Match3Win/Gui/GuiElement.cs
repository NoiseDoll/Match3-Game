using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Gui
{
    abstract class GuiElement
    {
        protected GuiElement parent;
        protected GuiScreen screen;

        public GuiElement Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                SetRelativePosition(Rectangle.Location);
            }
        }
        /// <summary>
        /// Sets and returns absolute rectangle
        /// </summary>
        public Rectangle Rectangle { get; set; }

        public void SetRelativePosition(Point point)
        {
            if (parent != null)
            {
                point.X += parent.Rectangle.X;
                point.Y += parent.Rectangle.Y;
            }
            Rectangle = new Rectangle(point, Rectangle.Size);
        }

        public Vector2 GetRelativePosition()
        {
            return parent != null ? new Vector2(Rectangle.X - parent.Rectangle.X, Rectangle.Y - parent.Rectangle.Y) : GetAbsolutePosition();
        }

        public Vector2 GetAbsolutePosition()
        {
            return new Vector2(Rectangle.X, Rectangle.Y);
        }

        public void SetSize(Point size)
        {
            Rectangle = new Rectangle(Rectangle.Location, size);
        }

        internal virtual void LoadContent(GraphicsDeviceManager graphics, ContentManager content) { }
        internal virtual void UnloadContent() { }
        internal virtual void Update(GameTime gameTime) { }
        internal virtual void Draw(SpriteBatch spriteBatch) { }

    }
}
