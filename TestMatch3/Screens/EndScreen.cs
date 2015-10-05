using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Match3.Gui;

namespace Match3.Screens
{
    class EndScreen : GuiScreen
    {
        private readonly string GAME_OVER_TEXT = Text.GetString("en", "gameover");
        private readonly string SCORE_TEXT = Text.GetString("en", "score");
        private readonly string OK_TEXT = Text.GetString("en", "ok");
        private const int LABEL_GAME_OVER_ID = 0;
        private const int LABEL_SCORE_ID = 1;
        private const int BUTTON_OK_ID = 2;

        private int totalScore;

        public int TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                totalScore = value;
                GuiLabel scoreLabel = ((GuiLabel)childElements[LABEL_SCORE_ID]);
                scoreLabel.SetText(SCORE_TEXT + totalScore.ToString());
                scoreLabel.SetRelativePosition(new Point(Consts.SCREEN_HORIZ_CENTER - scoreLabel.Rectangle.Width / 2,
                Consts.SCREEN_VERT_CENTER - scoreLabel.Rectangle.Height / 2));
            }
        }

        public EndScreen(Match3Game game) : base(game) { }

        internal override void LoadContent()
        {
            type = "EndScreen";
            MakeGui();
            foreach (var element in childElements)
            {
                element.Value.LoadContent(game.Graphics, game.Content);
            }
        }

        private void MakeGui()
        {
            string scoreText = SCORE_TEXT + totalScore.ToString();
            GuiLabel scoreLabel = new GuiLabel(scoreText, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point scorePosition = new Point(Consts.SCREEN_HORIZ_CENTER - scoreLabel.Rectangle.Width / 2,
                Consts.SCREEN_VERT_CENTER - scoreLabel.Rectangle.Height / 2);
            scoreLabel.SetRelativePosition(scorePosition);
            AddElement(LABEL_SCORE_ID, scoreLabel);

            GuiLabel gameOverLabel = new GuiLabel(GAME_OVER_TEXT, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point labelPosition = new Point(Consts.SCREEN_HORIZ_CENTER - gameOverLabel.Rectangle.Width / 2,
                scorePosition.Y - gameOverLabel.Rectangle.Height - 5);
            gameOverLabel.SetRelativePosition(labelPosition);
            AddElement(LABEL_GAME_OVER_ID, gameOverLabel);

            GuiLabel buttonLabel = new GuiLabel(OK_TEXT, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point buttonSize = new Point(200, 50);
            Point buttonPosition = new Point(Consts.SCREEN_HORIZ_CENTER - buttonSize.X / 2,
                scoreLabel.Rectangle.Bottom + 5);
            GuiButton okButton = new GuiButton(new Rectangle(buttonPosition, buttonSize),
                Consts.BUTTON_NORMAL, Consts.BUTTON_HOVER, Consts.BUTTON_PRESSED, buttonLabel);
            okButton.OnClick += RetryAction;
            AddElement(BUTTON_OK_ID, okButton);
        }

        private void RetryAction(object sender, EventArgs e)
        {
            game.ChangeScreen(typeof(StartScreen));
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
            foreach (var element in childElements)
            {
                element.Value.Update(gameTime);
            }
        }

        internal override void Draw()
        {
            game.SpriteBatch.Begin();
            foreach (var element in childElements)
            {
                element.Value.Draw(game.SpriteBatch);
            }
            game.SpriteBatch.End();
        }
    }
}
