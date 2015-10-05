using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Match3.Gui
{
    class GuiButton : GuiElement, IDisposable
    {
        private Color backColor;
        private Color backColorHover;
        private Color backColorPressed;
        private GuiElementState state;
        private Texture2D backTexture;
        private GuiLabel label;

        public event EventHandler OnClick;

        public GuiButton(Rectangle position, Color backColor, Color backColorHover, Color backColorPressed, GuiLabel label)
        {
            Rectangle = position;
            this.backColor = backColor;
            this.backColorHover = backColorHover;
            this.backColorPressed = backColorPressed;
            if (label != null)
            {
                this.label = label;
                Point textPosition = CalculateTextPosition();
                this.label.Parent = this;
                this.label.SetRelativePosition(textPosition);
            }
            state = GuiElementState.Normal;
        }

        private Point CalculateTextPosition()
        {
            int x = (Rectangle.Width <= label.Rectangle.Width) ? 0 : (Rectangle.Width - label.Rectangle.Width) / 2;
            int y = (Rectangle.Height <= label.Rectangle.Height) ? 0 : (Rectangle.Height - label.Rectangle.Height) / 2;
            return new Point(x, y);
        }

        internal override void LoadContent(GraphicsDeviceManager graphics, ContentManager content)
        {
            backTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            backTexture.SetData(new[] { Color.White });
        }

        internal override void UnloadContent()
        {
            backTexture?.Dispose();
        }

        internal override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (Rectangle.Contains(mouseState.Position))
            {
                if (state == GuiElementState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    state = GuiElementState.Hover;
                    OnClick?.Invoke(this, null);
                }
                else
                {
                    state = mouseState.LeftButton == ButtonState.Pressed ? GuiElementState.Pressed : GuiElementState.Hover;
                }
            }
            else
            {
                state = GuiElementState.Normal;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            Color drawButtonColor = backColor;
            switch (state)
            {
                case GuiElementState.Normal:
                    drawButtonColor = backColor;
                    break;
                case GuiElementState.Hover:
                    drawButtonColor = backColorHover;
                    break;
                case GuiElementState.Pressed:
                    drawButtonColor = backColorPressed;
                    break;
            }
            spriteBatch.Draw(backTexture, Rectangle, drawButtonColor);
            label?.Draw(spriteBatch);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    backTexture?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
