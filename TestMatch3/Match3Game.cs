using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Match3.Screens;
using System.Collections.Generic;
using Match3.Gui;

namespace Match3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Match3Game : Game
    {
        private GuiScreen currentScreen;
        private ContentManager sharedContentManager;

        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public Dictionary<int, object> SharedContent { get; private set; }
        public string TestFile { get; set; }

        public Match3Game()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Consts.SCREEN_WIDTH,
                PreferredBackBufferHeight = Consts.SCREEN_HIEGHT
            };
            Content.RootDirectory = "Content";
            sharedContentManager = new ContentManager(Services, "ContentShared");
            SharedContent = new Dictionary<int, object>();
            IsMouseVisible = true;

            currentScreen = new StartScreen(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            LoadSharedContent();
            currentScreen.LoadContent();
        }

        private void LoadSharedContent()
        {
            SharedContent.Add(Consts.RESOURCE_FONT, sharedContentManager.Load<SpriteFont>("Match3Font"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            currentScreen.UnloadContent();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            currentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Consts.BACKGROUND);

            currentScreen.Draw();

            base.Draw(gameTime);
        }

        internal GuiScreen ChangeScreen(Type screenType)
        {
            GuiScreen screen = null;
            if (screenType.BaseType == typeof(GuiScreen))
            {
                currentScreen.UnloadContent();
                screen = (GuiScreen)Activator.CreateInstance(screenType, this);
                currentScreen = screen;
                currentScreen.LoadContent();
            }
            return screen;
        }
    }
}
