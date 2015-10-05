using Match3.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Gameplay
{
    class Cell
    {
        private const double FADE_IN_SPEED = 2f;
        private const double FADE_OUT_SPEED = 2f;
        private const int SPEED_FALL_BASE = 35;
        private const int SPEED_FALL_MOD = 150;
        private const int SPEED_SWAP = 180;
        private const int SPEED_UNSWAP = 240;
        private readonly Color backColor = Consts.BUTTON_HOVER;

        private GuiGrid parent;
        private Vector2 location;
        private Point size;
        private Vector2 moveDestination;
        private ShapesAtlas shapeTexture;
        private Texture2D backTexture;
        private float opacity;
        private int speed;

        public Animation Animation { get; private set; }
        public bool IsSelected { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Shape Shape { get; set; }
        public Bonus Bonus { get; set; }
        public GuiElementState State { get; set; }

        public Cell(GuiGrid parent, Shape shape, ShapesAtlas texture, Texture2D backTexture, int row, int column)
        {
            this.parent = parent;
            Shape = shape;
            shapeTexture = texture;
            this.backTexture = backTexture;
            Row = row;
            Column = column;

            size = parent.CellSize;
            location = new Vector2((Column * size.X) + parent.Rectangle.X, (Row * size.Y) + parent.Rectangle.Y);

            Animation = Animation.FadeIn;
            State = GuiElementState.Normal;
            Bonus = Bonus.None;
            IsSelected = false;
        }

        /// <summary>
        /// Processes cell animation
        /// </summary>
        /// <returns>Returns true if animation is not finished</returns>
        internal bool Update(GameTime gameTime)
        {
            if (Animation == Animation.None)
            {
                return false;
            }
            switch (Animation)
            {
                case Animation.FadeIn:
                    FadeIn(gameTime);
                    break;
                case Animation.FadeOut:
                    FadeOut(gameTime);
                    break;
                case Animation.Fall:
                    Fall(gameTime);
                    break;
                case Animation.Swap:
                    Swap(gameTime);
                    break;
            }
            return true;
        }

        private void FadeIn(GameTime gameTime)
        {
            opacity += (float)(FADE_IN_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            if (opacity >= 1f)
            {
                opacity = 1f;
                Animation = Animation.None;
            }
        }

        private void FadeOut(GameTime gameTime)
        {
            opacity -= (float)(FADE_OUT_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            if (opacity <= 0f)
            {
                Shape = Shape.Empty;
                opacity = 1f;
                Animation = Animation.None;
            }
        }

        private void Fall(GameTime gameTime)
        {
            location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
            if (location.Y >= moveDestination.Y)
            {
                location.Y = moveDestination.Y;
                Animation = Animation.None;
            }
        }

        private void Swap(GameTime gameTime)
        {
            if (location.X == moveDestination.X)
            {
                if (location.Y == moveDestination.Y)
                {
                    Animation = Animation.None;
                    opacity = 1f;
                }
                else
                {
                    MoveHorizontal(gameTime);
                }
            }
            else
            {
                MoveVertical(gameTime);
            }
        }

        private void MoveHorizontal(GameTime gameTime)
        {
            if (location.Y < moveDestination.Y)
            {
                location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                if (location.Y >= moveDestination.Y)
                {
                    location.Y = moveDestination.Y;
                    Animation = Animation.None;
                    opacity = 1f;
                }
            }
            else
            {
                location.Y -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                if (location.Y <= moveDestination.Y)
                {
                    location.Y = moveDestination.Y;
                    Animation = Animation.None;
                    opacity = 1f;
                }
            }
        }

        private void MoveVertical(GameTime gameTime)
        {
            if (location.X < moveDestination.X)
            {
                location.X += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                if (location.X >= moveDestination.X)
                {
                    location.X = moveDestination.X;
                    Animation = Animation.None;
                    opacity = 1f;
                }
            }
            else
            {
                location.X -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                if (location.X <= moveDestination.X)
                {
                    location.X = moveDestination.X;
                    Animation = Animation.None;
                    opacity = 1f;
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = new Rectangle((int)location.X, (int)location.Y, size.X, size.Y);
            if (backTexture != null)
            {
                switch (State)
                {
                    case GuiElementState.Normal:
                        break;
                    case GuiElementState.Hover:
                        spriteBatch.Draw(backTexture, rectangle, backColor);
                        break;
                    case GuiElementState.Pressed:
                        spriteBatch.Draw(backTexture, rectangle, Color.White);
                        break;
                }
                if (IsSelected)
                {
                    spriteBatch.Draw(backTexture, rectangle, Color.White);
                }
            }
            shapeTexture.Draw(spriteBatch, Shape, Bonus, location, opacity);
        }

        internal void Destroy()
        {
            if (Shape != Shape.Empty && Animation != Animation.FadeOut)
            {
                State = GuiElementState.Normal;
                Animation = Animation.FadeOut;
                parent.AddDestroyer(Row, Column, Bonus);
            }
        }

        internal void Spawn(Shape shape)
        {
            State = GuiElementState.Normal;
            Shape = shape;
            opacity = 0f;
            Animation = Animation.FadeIn;
        }

        internal void FallInto(Cell cell)
        {
            cell.State = GuiElementState.Normal;
            cell.Shape = Shape;
            Shape = Shape.Empty;
            cell.Bonus = Bonus;
            Bonus = Bonus.None;

            cell.moveDestination = cell.location;
            cell.location = location;
            cell.speed = (cell.Row * SPEED_FALL_BASE) + SPEED_FALL_MOD;

            cell.Animation = Animation.Fall;
        }

        public bool IsCloseTo(Cell cell)
        {
            return (Column == cell.Column && (Row == cell.Row - 1 || Row == cell.Row + 1)) ||
                (Row == cell.Row && (Column == cell.Column - 1 || Column == cell.Column + 1)) ? true : false;
        }

        internal void SwitchSelection()
        {
            IsSelected = !IsSelected;
        }

        internal void SwapWith(Cell cell, bool unswap)
        {
            int swapSpeed = unswap ? SPEED_UNSWAP : SPEED_SWAP;

            State = GuiElementState.Normal;
            cell.State = GuiElementState.Normal;

            Vector2 tempLocation = location;
            location = cell.location;
            cell.location = tempLocation;

            cell.moveDestination = location;
            moveDestination = cell.location;

            Shape tempShape = Shape;
            Shape = cell.Shape;
            cell.Shape = tempShape;

            Bonus tempBonus = Bonus;
            Bonus = cell.Bonus;
            cell.Bonus = tempBonus;

            speed = swapSpeed;
            cell.speed = swapSpeed;
            opacity = 0.5f;
            cell.opacity = 0.5f;
            Animation = Animation.Swap;
            cell.Animation = Animation.Swap;
        }

    }
}
