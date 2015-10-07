using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Match3.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Match3.Screens;

namespace Match3.Gui
{
    class GuiGrid : GuiElement, IDisposable
    {
        public Cell[,] cells;
        public List<Destroyer> destroyers;
        private Cell currentCell;
        private Cell selectedCell;
        private Texture2D backTexture;
        private Texture2D fireTexture;
        private ShapesAtlas shapesAtlas;
        private Random random;
        private Array shapes;

        public Point CellSize { get; private set; }
        public bool IsAnimating { get; private set; }

        public GuiGrid(GuiScreen screen)
        {
            this.screen = screen;
            cells = new Cell[8, 8];
            CellSize = new Point(60, 60);
            SetSize(new Point(cells.GetLength(0) * CellSize.X, cells.GetLength(1) * CellSize.Y));
            destroyers = new List<Destroyer>();
            random = new Random();
            shapes = Enum.GetValues(typeof(Shape));
            IsAnimating = false;
        }

        internal override void LoadContent(GraphicsDeviceManager graphics, ContentManager content)
        {
            shapesAtlas = new ShapesAtlas(content.Load<Texture2D>("shapes"));
            backTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            backTexture.SetData(new[] { Color.White });
            fireTexture = content.Load<Texture2D>("fire");

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Cell cell = new Cell(this, Shape.Empty, shapesAtlas, backTexture, i, j);
                    cells[i, j] = cell;
                }
            }
        }

        internal override void UnloadContent()
        {
            backTexture?.Dispose();
        }

        internal override void Update(GameTime gameTime)
        {
            IsAnimating = false;
            UpdateCells(gameTime);
            UpdateDestroyers(gameTime);
        }

        /// <summary>
        /// Runs cells animation.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCells(GameTime gameTime)
        {
            foreach (var cell in cells)
            {
                if (cell.Animation != Animation.None)
                {
                    IsAnimating = true;
                }
                cell.Update(gameTime);
            }
        }

        /// <summary>
        /// Runs destroyers animation, removes blocks reached by destroyer, and removes unused destroyers.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values</param>
        private void UpdateDestroyers(GameTime gameTime)
        {
            List<Point> toDestroy = new List<Point>(destroyers.Count);
            foreach (var destroyer in destroyers)
            {
                IsAnimating = true;
                if (destroyer.Update(gameTime))
                {
                    toDestroy.Add(new Point(destroyer.Position.X, destroyer.Position.Y));
                }
            }
            var detonated = destroyers.FindAll(d => d.Remove && d.Direction == Direction.Detonate);
            AddSecondWaveDestroyers(detonated);
            destroyers.RemoveAll(d => d.Remove);
            int scoreFromDestroyers = toDestroy.Count(c => cells[c.X, c.Y].Animation == Animation.None);
            scoreFromDestroyers = scoreFromDestroyers * scoreFromDestroyers * Consts.SCORE_BONUS;
            ((GameScreen)screen).AddScore(scoreFromDestroyers);
            toDestroy.ForEach(point => cells[point.X, point.Y].Destroy());
        }

        /// <summary>
        /// Adds destroyers around every bomb after 250ms
        /// </summary>
        /// <param name="detonated">List of bomb destroyers placed 250ms ago</param>
        private void AddSecondWaveDestroyers(List<Destroyer> detonated)
        {
            foreach (var item in detonated)
            {
                for (int i = item.Position.X - 1; i <= item.Position.X + 1; i++)
                {
                    for (int j = item.Position.Y - 1; j <= item.Position.Y + 1; j++)
                    {
                        if (i == item.Position.X && j == item.Position.Y)
                        {
                            continue;
                        }
                        destroyers.Add(new Destroyer(this, new Vector2(j * CellSize.X + Rectangle.X, i * CellSize.Y + Rectangle.Y),
                            fireTexture, Direction.SecondWave));
                    }
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in cells)
            {
                cell.Draw(spriteBatch);
            }
            foreach (var destroyer in destroyers)
            {
                destroyer.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Loads grid from file. File is 8x8 pattern with "bs" elements, where 'b' - is digit from 0 to 3 describing bonus,
        /// 's' - is digit from 0 to 5 describing shape, and space or newline as separator for elements.
        /// </summary>
        /// <param name="testFile">Path to file</param>
        /// <returns>Return true if file format is ok</returns>
        internal bool LoadFromFile(string testFile)
        {
            string[,] temp = new string[8, 8];
            using (StreamReader sr = File.OpenText(testFile))
            {
                int row = 0;
                while (sr.Peek() >= 0)
                {
                    sr.ReadLine();
                    row++;
                }
                if (row != 8)
                {
                    return false;
                }
                sr.BaseStream.Position = 0;
                sr.DiscardBufferedData();
                row = 0;
                while (sr.Peek() >= 0)
                {
                    string[] tempLine = sr.ReadLine().Split(' ');
                    if (tempLine.Length != 8)
                    {
                        return false;
                    }
                    for (int i = 0; i < tempLine.Length; i++)
                    {
                        if (tempLine[i].Length != 2 && tempLine[i].All(c => c >= '0' && c <= '9'))
                        {
                            return false;
                        }
                        temp[row, i] = tempLine[i];
                    }
                    row++;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int shapeCode, bonusCode;
                    int.TryParse(temp[i, j][1].ToString(), out shapeCode);
                    int.TryParse(temp[i, j][0].ToString(), out bonusCode);
                    cells[i, j].Shape = (Shape)shapeCode;
                    cells[i, j].Bonus = (Bonus)bonusCode;
                }
            }
            return true;
        }

        /// <summary>
        /// Fills grid cells with blocks with random shape.
        /// </summary>
        /// <returns>Returns true if at least one block has been added.</returns>
        internal bool SpawnBlocks()
        {
            bool r = false;

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].Shape == Shape.Empty)
                    {
                        r = true;
                        Shape shape = (Shape)shapes.GetValue(random.Next(shapes.Length - 1) + 1);
                        cells[i, j].Spawn(shape);
                        cells[i, j].Bonus = Bonus.None;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// Looks for horizontal and vertical matches, destroys matched blocks and spawns bonuses.
        /// </summary>
        /// <returns>Returns score points earned in this destroy cycle. Zero score means no match.</returns>
        internal int MatchAndDestroy()
        {
            int score = 0;
            List<Cell> toDestroy = new List<Cell>(64);
            List<Cell> newBonuses = new List<Cell>();

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                List<Cell> temp = new List<Cell>(8) { cells[i, 0] };
                for (int j = 1; j < cells.GetLength(1); j++)
                {
                    bool stop = false;
                    if (cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || j == cells.GetLength(1) - 1)
                    {
                        if (temp.Count >= Consts.MATCH_MIN)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(SpawnBonus(temp, Bonus.LineHorizontal));
                            }
                            toDestroy.AddRange(temp);
                            score += temp.Count * temp.Count * Consts.SCORE_BONUS;
                        }
                        temp.Clear();
                        temp.Add(cells[i, j]);
                    }
                }
            }
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                List<Cell> temp = new List<Cell>(8) { cells[0, j] };
                for (int i = 1; i < cells.GetLength(0); i++)
                {
                    bool stop = false;
                    if (cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || i == cells.GetLength(0) - 1)
                    {
                        if (temp.Count >= Consts.MATCH_MIN)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(SpawnBonus(temp, Bonus.LineVertical));
                            }
                            var intersect = toDestroy.Intersect(temp);
                            List<Cell> intersectList = intersect.ToList();
                            foreach (var bonusCell in intersectList)
                            {
                                temp.Remove(bonusCell);
                                toDestroy.Remove(bonusCell);
                                bonusCell.Bonus = Bonus.Bomb;
                            }
                            toDestroy.AddRange(temp);
                            score += temp.Count * temp.Count * Consts.SCORE_BONUS;
                        }
                        temp.Clear();
                        temp.Add(cells[i, j]);
                    }
                }
            }
            newBonuses.ForEach(c => toDestroy.Remove(c));
            toDestroy.ForEach(cell => cell.Destroy());
            if (selectedCell != null && score > 0)
            {
                selectedCell = null;
            }
            return score;
        }

        /// <summary>
        /// Checks bonus spawn condition after successful match.
        /// </summary>
        /// <param name="matchedCells">Matched cells</param>
        /// <param name="orientation">Determines horizontal or vertical line bonus</param>
        /// <returns>Returns <see cref="Cell"/> with newly spawned bonus</returns>
        private Cell SpawnBonus(List<Cell> matchedCells, Bonus orientation)
        {
            Cell bonusCell = null;
            switch (matchedCells.Count)
            {
                case Consts.MATCH_LINE:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != Bonus.None)
                    {
                        AddDestroyer(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = orientation;
                    break;
                case Consts.MATCH_BOMB:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != Bonus.None)
                    {
                        AddDestroyer(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = Bonus.Bomb;
                    break;
            }
            return bonusCell;
        }

        /// <summary>
        /// Adds destroyers depending on triggered bonus
        /// </summary>
        internal void AddDestroyer(int row, int column, Bonus bonus)
        {
            switch (bonus)
            {
                case Bonus.LineVertical:
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row - 0.5f) * CellSize.Y + Rectangle.Y),
                        fireTexture, Direction.Up));
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row + 0.5f) * CellSize.Y + Rectangle.Y),
                        fireTexture, Direction.Down));
                    break;
                case Bonus.LineHorizontal:
                    destroyers.Add(new Destroyer(this, new Vector2((column - 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        fireTexture, Direction.Left));
                    destroyers.Add(new Destroyer(this, new Vector2((column + 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        fireTexture, Direction.Right));
                    break;
                case Bonus.Bomb:
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        fireTexture, Direction.Detonate));
                    break;
            }
        }

        /// <summary>
        /// Triggers all cells to start falling if there is free space
        /// </summary>
        internal void DropBlocks()
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                for (int i = cells.GetLength(0) - 1; i > 0; i--)
                {
                    if (cells[i, j].Shape == Shape.Empty)
                    {
                        int k = i - 1;
                        while (k >= 0 && cells[k, j].Shape == Shape.Empty)
                        {
                            k--;
                        }
                        if (k < 0) break;
                        cells[k, j].FallInto(cells[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Process user interactions. Highlight hovered over cells and selects clicked cells. Checks for swap action.
        /// </summary>
        /// <returns>Returns true if user swapped blocks</returns>
        internal bool UserInput()
        {
            MouseState mouseState = Mouse.GetState();

            if (Rectangle.Contains(mouseState.Position))
            {
                int i = (mouseState.Position.Y - Rectangle.Y) / CellSize.Y;
                int j = (mouseState.Position.X - Rectangle.X) / CellSize.X;

                if (currentCell != null && cells[i, j] != currentCell)
                {
                    currentCell.State = GuiElementState.Normal;
                }
                currentCell = cells[i, j];

                if (mouseState.LeftButton == ButtonState.Released && currentCell.State == GuiElementState.Pressed)
                {
                    currentCell.State = GuiElementState.Hover;
                    if (currentCell.IsSelected)
                    {
                        DiselectCurrentCell();
                    }
                    else if (selectedCell != null && currentCell.IsCloseTo(selectedCell))
                    {
                        //Swap condition reached
                        return true;
                    }
                    else
                    {
                        SelectCurrentCell();
                    }
                }
                else if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    currentCell.State = GuiElementState.Pressed;
                }
                else
                {
                    currentCell.State = GuiElementState.Hover;
                }
            }
            else
            {
                if (currentCell != null)
                {
                    currentCell.State = GuiElementState.Normal;
                }
            }
            return false;
        }

        private void SelectCurrentCell()
        {
            currentCell.SwitchSelection();
            selectedCell?.SwitchSelection();
            selectedCell = currentCell;
        }

        private void DiselectCurrentCell()
        {
            currentCell.SwitchSelection();
            selectedCell = null;
        }

        internal void SwapBlocks()
        {
            selectedCell.SwapWith(currentCell, false);
            selectedCell.SwitchSelection();
        }

        internal void UnswapBlocks()
        {
            currentCell.SwapWith(selectedCell, true);
            selectedCell = null;
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
