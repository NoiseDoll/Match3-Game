using Match3.Gameplay;
using Match3.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Screens
{
    class GameScreen : GuiScreen
    {
        private readonly string SCORE_TEXT = Text.GetString("en", "score");
        private readonly string TIMER_TEXT = Text.GetString("en", "time");
        private const int LABEL_SCORE_ID = 0;
        private const int LABEL_TIMER_ID = 1;
        private const int GRID_ID = 2;

        private GuiLabel labelTimer, labelScore;
        private GuiGrid _grid;
        private double currentTime;
        private int totalScore;
        private GameState gameState;

        public GameScreen(Match3Game game) : base(game)
        {
            gameState = game.TestFile != null ? GameState.LoadFromFile : GameState.Spawn;
            currentTime = Consts.GAME_TIME;
        }

        internal override void LoadContent()
        {
            type = "GameScreen";
            MakeGui();
            foreach (var element in childElements)
            {
                element.Value.LoadContent(game.Graphics, game.Content);
            }
            labelTimer = (GuiLabel)GetElement(LABEL_TIMER_ID);
            labelScore = (GuiLabel)GetElement(LABEL_SCORE_ID);
            _grid = (GuiGrid)GetElement(GRID_ID);
        }

        private void MakeGui()
        {
            GuiLabel scoreLabel = new GuiLabel(SCORE_TEXT, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point scorePosition = new Point(10, 5);
            scoreLabel.SetRelativePosition(scorePosition);
            AddElement(LABEL_SCORE_ID, scoreLabel);

            GuiLabel timerLabel = new GuiLabel(TIMER_TEXT, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point timerPosition = new Point(Consts.SCREEN_WIDTH - 150, 5);
            timerLabel.SetRelativePosition(timerPosition);
            AddElement(LABEL_TIMER_ID, timerLabel);

            GuiGrid grid = new GuiGrid(this);
            grid.SetRelativePosition(new Point(Consts.SCREEN_HORIZ_CENTER - grid.Rectangle.Width / 2,
                Consts.SCREEN_VERT_CENTER - grid.Rectangle.Height / 2));
            AddElement(GRID_ID, grid);
        }

        internal override void UnloadContent()
        {
            foreach (var element in childElements)
            {
                element.Value.UnloadContent();
            }
            game.Content.Unload();
        }

        internal override void Update(GameTime gameTime)
        {
            UpdateGuiText();
            RunGameLogic();

            foreach (var element in childElements)
            {
                element.Value.Update(gameTime);
            }

            currentTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime <= 0)
            {
                EndScreen endScreen = (EndScreen)game.ChangeScreen(typeof(EndScreen));
                endScreen.TotalScore = totalScore;
            }
        }

        private void UpdateGuiText()
        {
            labelScore.SetText(SCORE_TEXT + totalScore.ToString());
            labelTimer.SetText(TIMER_TEXT + ((int)currentTime).ToString());
        }

        /// <summary>
        /// This function performs various game events depending on <see cref="gameState"/>.
        /// After executing task it changes gameState for new <see cref="Update"/> cycle.
        /// Also it adds score points earned in this cycle to the entire pool (<see cref="totalScore"/>).
        /// </summary>
        private void RunGameLogic()
        {
            if (!_grid.IsAnimating)
            {
                int score;
                switch (gameState)
                {
                    case GameState.LoadFromFile:
                        gameState = _grid.LoadFromFile(game.TestFile) ? GameState.MatchAfterSpawn : GameState.Spawn;
                        break;
                    case GameState.Spawn:
                        gameState = _grid.SpawnBlocks() ? GameState.MatchAfterSpawn : GameState.Input;
                        break;
                    case GameState.MatchAfterSpawn:
                        score = _grid.MatchAndDestroy();
                        if (score > 0)
                        {
                            totalScore += score;
                            gameState = GameState.Fall;
                        }
                        else
                        {
                            gameState = GameState.Input;
                        }
                        break;
                    case GameState.Fall:
                        _grid.DropBlocks();
                        gameState = GameState.Spawn;
                        break;
                    case GameState.Input:
                        if (_grid.UserInput())
                        {
                            gameState = GameState.Swap;
                        }
                        break;
                    case GameState.Swap:
                        _grid.SwapBlocks();
                        gameState = GameState.MatchAfterSwap;
                        break;
                    case GameState.MatchAfterSwap:
                        score = _grid.MatchAndDestroy();
                        if (score > 0)
                        {
                            totalScore += score;
                            gameState = GameState.Fall;
                        }
                        else
                        {
                            gameState = GameState.Unswap;
                        }
                        break;
                    case GameState.Unswap:
                        _grid.UnswapBlocks();
                        gameState = GameState.Input;
                        break;
                }
            }
        }

        internal override void Draw()
        {
            game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            foreach (var element in childElements)
            {
                element.Value.Draw(game.SpriteBatch);
            }
            game.SpriteBatch.End();
        }

        internal void AddScore(int points)
        {
            totalScore += points;
        }
    }
}
