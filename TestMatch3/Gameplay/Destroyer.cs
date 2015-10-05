using Match3.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Gameplay
{
    class Destroyer
    {
        private const float SPEED = 300f;
        private const float DETONATE_TIMER_MS = 250f;
        private GuiGrid parent;
        private Vector2 location;
        private Texture2D texture;
        private double timer;
        public Direction Direction { get; private set; }
        public Point Position { get; private set; }
        public bool Remove { get; private set; }

        public Destroyer(GuiGrid parent, Vector2 location, Texture2D texture, Direction direction)
        {
            this.parent = parent;
            this.location = location;
            this.texture = texture;
            Direction = direction;
            Remove = false;
            Position = new Point(-1, -1);
        }

        /// <summary>
        /// Processes destroyer animation.
        /// </summary>
        /// <returns>Returns true if it reaches new block which should be destroyed</returns>
        internal bool Update(GameTime gameTime)
        {
            bool isNewBlockReached = SetPosition() &&
                Position.Y < 8 && Position.Y >= 0 && Position.X >= 0 && Position.X < 8;
            float dist = (float)(SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            switch (Direction)
            {
                case Direction.Up:
                    MoveUp(dist);
                    break;
                case Direction.Down:
                    MoveDown(dist);
                    break;
                case Direction.Left:
                    MoveLeft(dist);
                    break;
                case Direction.Right:
                    MoveRight(dist);
                    break;
                case Direction.Detonate:
                    Detonate(gameTime.ElapsedGameTime.TotalMilliseconds);
                    break;
                case Direction.SecondWave:
                    Detonate(gameTime.ElapsedGameTime.TotalMilliseconds);
                    break;
            }
            return isNewBlockReached;
        }

        /// <summary>
        /// Updated row and column values defined in <see cref="Position"/>.
        /// </summary>
        /// <returns>Returns true if destroyer reaches new cell</returns>
        private bool SetPosition()
        {
            bool r = false;
            int i = (int)((location.Y - parent.Rectangle.Y) / parent.CellSize.Y);
            int j = (int)((location.X - parent.Rectangle.X) / parent.CellSize.X);

            if (Position.X != j || Position.Y != i)
            {
                r = true;
            }
            Position = new Point(i, j);
            return r;
        }

        private void MoveUp(float dist)
        {
            location.Y -= dist;
            float end = parent.Rectangle.Top - parent.CellSize.Y;
            if (location.Y <= end)
            {
                location.Y = end;
                Remove = true;
            }
        }

        private void MoveDown(float dist)
        {
            location.Y += dist;
            float end = parent.Rectangle.Bottom;
            if (location.Y >= end)
            {
                location.Y = end;
                Remove = true;
            }
        }

        private void MoveLeft(float dist)
        {
            location.X -= dist;
            float end = parent.Rectangle.Left - parent.CellSize.X;
            if (location.X <= end)
            {
                location.X = end;
                Remove = true;
            }
        }

        private void MoveRight(float dist)
        {
            location.X += dist;
            float end = parent.Rectangle.Right;
            if (location.X >= end)
            {
                location.X = end;
                Remove = true;
            }
        }

        private void Detonate(double elapsedMilliseconds)
        {
            timer += elapsedMilliseconds;
            if (timer >= DETONATE_TIMER_MS)
            {
                Remove = true;
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, Color.White);
        }
    }
}
