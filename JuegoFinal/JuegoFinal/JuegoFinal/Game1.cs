using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace JuegoFinal
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>



    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Don't forget to put this above the Game1 class!
        public enum GameState
        {
            INTRO,
            LEVEL_CHANGE,
            START,
            PAUSE,
            PLAY,
            END
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Jugador jugador;
        float jugadorVel;
        KeyboardState KeyboardActual;
        KeyboardState KeyboardPrevio;
        GameState gameState = new GameState();
        int score = 0;
        Video myVideoFile;
        VideoPlayer videoPlayer = new VideoPlayer();
        bool playIntroMovie = true;
        Texture2D videoTexture;
        Texture2D mainBackground;
        Texture2D enemy1;
        Texture2D enemy2;
        SplashScreen splashScreen;
        SpriteFont font;
        string textToDraw;
        string secondaryTextToDraw;
        SpriteFont spriteFont;
        SpriteFont secondarySpriteFont;

        List<Enemies> enemies = new List<Enemies>();
        Random random = new Random();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameState.INTRO;
            jugador = new Jugador();
            jugadorVel = 8.0f;

            // Splash screen component
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.SetData("",
                gameState);



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Imagenes/S3");
            playerAnimation.Initialize(playerTexture,
          Vector2.Zero, 151, 49, 2, 38, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(
               GraphicsDevice.Viewport.TitleSafeArea.X +
               GraphicsDevice.Viewport.TitleSafeArea.Width / 2,
               GraphicsDevice.Viewport.TitleSafeArea.Y +
               GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            jugador.Initialize(playerAnimation, playerPosition);
            mainBackground = Content.Load<Texture2D>("Imagenes/intro");
            myVideoFile = Content.Load<Video>("Videos/Wildlife");
            spriteFont = Content.Load<SpriteFont>(@"fonts\SplashScreenFontLarge");
            secondarySpriteFont = Content.Load<SpriteFont>(@"fonts\SplashScreenFont");
            font = Content.Load<SpriteFont>(@"fonts\SplashScreenFont");
            // Start the music right away
            enemy1 = Content.Load<Texture2D>("Imagenes/bala");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void ChangeGameState(GameState state, int level)
        {
            gameState = state;

            switch (gameState)
            {
                case GameState.LEVEL_CHANGE:
                    splashScreen.SetData("Level " + (level + 1),
                        GameState.LEVEL_CHANGE);
                    //modelManager.Enabled = false;
                    //modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    // Stop the soundtrack loop
                    //trackCue.Stop(AudioStopOptions.Immediate);
                    break;

                case GameState.PLAY:
                    //modelManager.Enabled = true;
                    //modelManager.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;

                    //if (trackCue.IsPlaying)
                    //    trackCue.Stop(AudioStopOptions.Immediate);

                    // To play a stopped cue, get the cue from the soundbank again
                    //trackCue = soundBank.GetCue("Tracks");
                    //trackCue.Play();
                    break;

                case GameState.END:
                    splashScreen.SetData("Game Over.\nLevel: " + (level + 1) +
                        "\nScore: " + score, GameState.END);
                    //modelManager.Enabled = false;
                    //modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    // Stop the soundtrack loop
                    // trackCue.Stop(AudioStopOptions.Immediate);
                    break;
            }
        }

        private void UpdateJugador(GameTime gameTime)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            //currentState.IsConnected && currentState.Buttons.A == ButtonState.Pressed
            jugador.Update(gameTime);

            switch (gameState)
            {
                case GameState.INTRO:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        videoPlayer.Stop();
                        videoTexture = null;
                        gameState = GameState.START;
                    }
                    break;
                case GameState.START:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (gameState == Game1.GameState.LEVEL_CHANGE ||
                           gameState == Game1.GameState.START)
                            ChangeGameState(Game1.GameState.PLAY, 0);

                            // If we are in end game, exit
                        else if (gameState == Game1.GameState.END)
                            this.Exit();
                    }
                    break;
                case GameState.PLAY:
                    if (KeyboardActual.IsKeyDown(Keys.Left) || currentState.IsButtonDown(Buttons.LeftThumbstickLeft))
                    {
                        jugador.Position.X -= jugadorVel;
                    }
                    if (KeyboardActual.IsKeyDown(Keys.Right) || currentState.IsButtonDown(Buttons.LeftThumbstickRight))
                    {
                        jugador.Position.X += jugadorVel;
                    }
                    if (KeyboardActual.IsKeyDown(Keys.Up) || currentState.IsButtonDown(Buttons.LeftThumbstickUp))
                    {

                        jugador.Position.Y -= jugadorVel;
                    }
                    if (KeyboardActual.IsKeyDown(Keys.Down) || currentState.IsButtonDown(Buttons.LeftThumbstickDown))
                    {
                        jugador.Position.Y += jugadorVel;
                    }
                    if (KeyboardActual.IsKeyDown(Keys.Space) || currentState.IsConnected && currentState.Triggers.Right == 1)
                    {
                        // Disparar bajo el valor de fireTime
                        //if (gameTime.TotalGameTime - previousFireTime > fireTime)
                        //{
                        //    // Reiniciamos nuestro tiempo actual
                        //    //previousFireTime = gameTime.TotalGameTime;
                        //    // Agregamos el proyectil al frente de la nave
                        //   // AddProjectile(jugador.Position + new Vector2(jugador.Width / 2, 0));
                        //    // Play the laser sound
                        //    //laserSound.Play();
                        //}

                    }
                    jugador.Position.X = MathHelper.Clamp(jugador.Position.X, 0,
                        GraphicsDevice.Viewport.Width - jugador.Width);
                    jugador.Position.Y = MathHelper.Clamp(jugador.Position.Y, 0,
                        GraphicsDevice.Viewport.Height - jugador.Height);

                    if (jugador.Health <= 0)
                    {
                        gameState = GameState.END;
                    }
                    break;
                case GameState.END:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        ChangeGameState(Game1.GameState.PLAY, 0);
                        jugador.Health = 100;
                        score = 0;
                    }
                    break;


            }







        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        float spawn = 0;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardPrevio = KeyboardActual;
            KeyboardActual = Keyboard.GetState();

            if (gameState == GameState.INTRO)
            {
                if (playIntroMovie)
                {
                    videoPlayer.Play(myVideoFile);
                    playIntroMovie = false;
                    videoPlayer.Volume = 1;
                }
                if (videoPlayer.State == MediaState.Stopped)
                {
                    videoPlayer.Stop();
                    videoTexture = null;
                    gameState = GameState.START;
                }


            }
            if (gameState == GameState.PLAY)
            {
                spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (Enemies enemy in enemies) {
                    enemy.Update(graphics.GraphicsDevice);
                }
                LoadEnemies();
            }

            UpdateJugador(gameTime);

            // Update the parallaxing background
            //bgLayer1.Update();
            //bgLayer2.Update();

            // Update the enemies
            //UpdateEnemies(gameTime);
            //// Update the collision
            //UpdateCollision();
            //// Actualizar proyectiles
            //UpdateProjectiles();
            //// Actualizando las explosiones
            //UpdateExplosions(gameTime);
            base.Update(gameTime);

        }

        public void LoadEnemies()
        {
            int ranY = random.Next(100, 400);

            if (spawn >= 1) {
                spawn = 0;
                if (enemies.Count() < 4)
                    enemies.Add(new Enemies(enemy1, new Vector2(1100,ranY)));
            }
            for(int i=0; i<enemies.Count;i++)
                if (!enemies[i].isVisible) {
                    enemies.RemoveAt(i);
                    i--;
                }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.INTRO:

                    if (videoPlayer.State != MediaState.Stopped)
                        videoTexture = videoPlayer.GetTexture();

                    // Drawing to the rectangle will stretch the 
                    // video to fill the screen
                    Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X,
                        GraphicsDevice.Viewport.Y,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height);

                    // Draw the video, if we have a texture to draw.
                    if (videoTexture != null)
                    {
                        spriteBatch.Draw(videoTexture, screen, Color.White);
                    }
                    string ti = "Escuadron201";
                    Vector2 TitleSiz = spriteFont.MeasureString(ti);
                    secondaryTextToDraw = "Press ESCAPE to skip";
                    spriteBatch.DrawString(secondarySpriteFont,
                       secondaryTextToDraw,
                       new Vector2(Window.ClientBounds.Width / 2
                           - secondarySpriteFont.MeasureString(
                               secondaryTextToDraw).X / 2,
                           Window.ClientBounds.Height / 2 +
                           TitleSiz.Y + 10),
                           Color.Gold);

                    break;

                case Game1.GameState.START:
                    textToDraw = "";
                    secondaryTextToDraw = "Press ENTER to begin";
                    spriteBatch.Draw(mainBackground, Vector2.UnitY, Color.White);
                    // Get size of string
                    string tit = "Escuadron201";
                    Vector2 TitleSize = spriteFont.MeasureString(tit);

                    // Draw main text
                    spriteBatch.DrawString(spriteFont, textToDraw, new Vector2(Window.ClientBounds.Width / 2
                            - TitleSize.X / 2,
                            Window.ClientBounds.Height / 2),
                            Color.Gold);

                    // Draw subtext
                    spriteBatch.DrawString(secondarySpriteFont,
                        secondaryTextToDraw,
                        new Vector2(Window.ClientBounds.Width / 2
                            - secondarySpriteFont.MeasureString(
                                secondaryTextToDraw).X / 2,
                            Window.ClientBounds.Height / 2 +
                            TitleSize.Y + 10),
                            Color.Gold);
                    break;
                case GameState.LEVEL_CHANGE:
                    break;
                case GameState.PLAY:
                    spriteBatch.DrawString(font, "puntos: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                    // Draw the player health
                    spriteBatch.DrawString(font, "vida: " + jugador.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
                    jugador.Draw(spriteBatch);
                    foreach (Enemies enemy in enemies) {
                        enemy.Draw(spriteBatch);
                    }
                    break;
                case GameState.END:
                    //spriteBatch.DrawString(font, "puntos: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

                    textToDraw = "";
                    secondaryTextToDraw = "Press SPACE to play again";
                    //spriteBatch.Draw(mainBackground, Vector2.UnitY, Color.White);
                    // Get size of string
                    string tit2 = "Escuadron201";
                    Vector2 TitleSize2 = spriteFont.MeasureString(tit2);

                    // Draw main text
                    spriteBatch.DrawString(spriteFont, "puntos: " + score, new Vector2(Window.ClientBounds.Width / 2
                            - TitleSize2.X / 2,
                            Window.ClientBounds.Height / 2),
                            Color.Gold);

                    // Draw subtext
                    spriteBatch.DrawString(secondarySpriteFont,
                        secondaryTextToDraw,
                        new Vector2(Window.ClientBounds.Width / 2
                            - secondarySpriteFont.MeasureString(
                                secondaryTextToDraw).X / 2,
                            Window.ClientBounds.Height / 2 +
                            TitleSize2.Y + 10),
                            Color.Gold);
                    // Draw the player health
                    break;
            }

            // TODO: Add your drawing code here
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
