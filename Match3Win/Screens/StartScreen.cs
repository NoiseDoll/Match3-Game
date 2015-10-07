using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Match3.Gui;

namespace Match3.Screens
{
    class StartScreen : GuiScreen
    {
        private const int BUTTON_PLAY_ID = 0;
        private readonly string PLAY_TEXT = Text.GetString("en", "play");

        public StartScreen(Match3Game game) : base(game) { }

        internal override void LoadContent()
        {
            type = "StartScreen";
            MakeGui();
            foreach (var element in childElements)
            {
                element.Value.LoadContent(game.Graphics, game.Content);
            }
        }

        private void MakeGui()
        {
            GuiLabel buttonLabel = new GuiLabel(PLAY_TEXT, (SpriteFont)game.SharedContent[Consts.RESOURCE_FONT]);
            Point buttonSize = new Point(200, 50);
            Point buttonPosition = new Point(Consts.SCREEN_HORIZ_CENTER - buttonSize.X / 2, Consts.SCREEN_VERT_CENTER - buttonSize.Y / 2);
            GuiButton playButton = new GuiButton(new Rectangle(buttonPosition, buttonSize),
                Consts.BUTTON_NORMAL, Consts.BUTTON_HOVER, Consts.BUTTON_PRESSED, buttonLabel);
            playButton.OnClick += StartGameAction;

            AddElement(BUTTON_PLAY_ID, playButton);
        }

        private void StartGameAction(object sender, EventArgs e)
        {
            game.ChangeScreen(typeof(GameScreen));
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
