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

namespace shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region declaraciones
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Jugador jugador;
        KeyboardState KeyboardActual;
        KeyboardState KeyboardPrevio;
        float jugadorVel;
        // Image used to display the static background
        Texture2D mainBackground;
        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;
        // Enemies

        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear

        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator

        Random random;

        // Textura de un proyectil
        Texture2D projectileTexture;
        // Lista de proyectiles
        List<Projectile> projectiles;
        // Tasa de proyectiles a disparar
        TimeSpan fireTime;
        // Tiempo de disparo anterior
        TimeSpan previousFireTime;
        Texture2D explosionTexture;
        List<Animation> explosions;
        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The music played during gameplay
        Song gameplayMusic;
        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        Vector2 velocity;
Vector2 gravity = new Vector2(0, -9.8f);
        Vector2 position;


        #endregion
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
            jugador = new Jugador();
            jugadorVel = 8.0f;

            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            projectiles = new List<Projectile>();
            // Un laser se dispara cada cuarto de segundo
            fireTime = TimeSpan.FromSeconds(.15f);
            explosions = new List<Animation>();
            //Set player’s score to zero
            score = 0;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Imagenes/animacionNave");
            playerAnimation.Initialize(playerTexture,
          Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(
                GraphicsDevice.Viewport.TitleSafeArea.X +
                GraphicsDevice.Viewport.TitleSafeArea.Width / 2,
                GraphicsDevice.Viewport.TitleSafeArea.Y +
                GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            jugador.Initialize(playerAnimation, playerPosition);

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Imagenes/fondoCapa1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "Imagenes/fondoCapa2", GraphicsDevice.Viewport.Width, -2);

            mainBackground = Content.Load<Texture2D>("Imagenes/animacionNave");
            enemyTexture = Content.Load<Texture2D>("Imagenes/animacionMina");
            projectileTexture = Content.Load<Texture2D>("Imagenes/laser");
            explosionTexture = Content.Load<Texture2D>("Imagenes/animacionExplosion");
            // Load the music
            gameplayMusic = Content.Load<Song>("Sonidos/gameMusic");

            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("Sonidos/efectoLaser");
            explosionSound = Content.Load<SoundEffect>("Sonidos/efectoExplosion");

            // Start the music right away
            PlayMusic(gameplayMusic);
            // Load the score font
            font = Content.Load<SpriteFont>("Fuente/gameFont");
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport,
            projectileTexture, position);
            projectiles.Add(projectile);
        }
        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position,
                134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    if (enemies[i].Health <= 0)
                    {
                        // Agregamos una explosion
                        AddExplosion(enemies[i].Position);
                        score += enemies[i].Value;
                        explosionSound.Play();
                    }

                    enemies.RemoveAt(i);
                    //Add to the player’s score

                }
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void UpdateJugador(GameTime gameTime)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            //currentState.IsConnected && currentState.Buttons.A == ButtonState.Pressed
            jugador.Update(gameTime);
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
                
                //// Update:
                //float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                //velocity += gravity * time;
                //position += velocity * time;

                jugador.Position.Y -= jugadorVel;
            }
            if (KeyboardActual.IsKeyDown(Keys.Down) || currentState.IsButtonDown(Buttons.LeftThumbstickDown))
            {
                jugador.Position.Y += jugadorVel;
            }
            if (KeyboardActual.IsKeyDown(Keys.Space) || currentState.IsConnected && currentState.Triggers.Right == 1)
            {
                // Disparar bajo el valor de fireTime
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    // Reiniciamos nuestro tiempo actual
                    previousFireTime = gameTime.TotalGameTime;
                    // Agregamos el proyectil al frente de la nave
                    AddProjectile(jugador.Position + new Vector2(jugador.Width / 2, 0));
                    // Play the laser sound
                    laserSound.Play();
                }

            }
            jugador.Position.X = MathHelper.Clamp(jugador.Position.X, 0,
                GraphicsDevice.Viewport.Width - jugador.Width);
            jugador.Position.Y = MathHelper.Clamp(jugador.Position.Y, 0,
                GraphicsDevice.Viewport.Height - jugador.Height);


            // reset score if player health goes to zero
            if (jugador.Health <= 0)
            {
                jugador.Health = 100;
                score = 0;
            }

        }

        private void UpdateProjectiles()
        {
            // Actualizar los Proyectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // Use the Rectangle’s built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)jugador.Position.X,
            (int)jugador.Position.Y,
            jugador.Width,
            jugador.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    jugador.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (jugador.Health <= 0)
                        jugador.Active = false;
                    // explosions[i].Draw(spriteBatch);
                }

            }
            // Colision entre Proyectiles vs Enemigos
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Creamos los rectangulos para validar si hay colision
                    rectangle1 = new Rectangle(
                        (int)projectiles[i].Position.X -
                        projectiles[i].Width / 2,
                        (int)projectiles[i].Position.Y -
                        projectiles[i].Height / 2,
                        projectiles[i].Width,
                        projectiles[i].Height);

                    rectangle2 = new Rectangle(
                        (int)enemies[j].Position.X -
                        enemies[j].Width / 2,
                        (int)enemies[j].Position.Y -
                        enemies[j].Height / 2,
                        enemies[j].Width,
                        enemies[j].Height);

                    // Determinamos si se intersectan
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardPrevio = KeyboardActual;
            KeyboardActual = Keyboard.GetState();
            UpdateJugador(gameTime);

            // Update the parallaxing background
            bgLayer1.Update();
            bgLayer2.Update();

            // Update the enemies
            UpdateEnemies(gameTime);
            // Update the collision
            UpdateCollision();
            // Actualizar proyectiles
            UpdateProjectiles();
            // Actualizando las explosiones
            UpdateExplosions(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(mainBackground, Vector2.UnitY, Color.White);
            // Draw the moving background
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);
            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            jugador.Draw(spriteBatch);
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }

            // Dibujamos las explosiones
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }
            // Draw the score
            spriteBatch.DrawString(font, "puntos: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "vida: " + jugador.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
