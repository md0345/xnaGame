using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
namespace JuegoFinal
{
    class Jugador
    {
        public void Initialize(Animation animation, Vector2 position)
        {
            //PlayerTexture = texture;
            PlayerAnimation = animation;
            // Asignar la posicion del jugador
            Position = position;
            // Activar el jugador
            Active = true;
            // Inicializar la vida del jugador
            Health = 10;
        }

        public void Update(GameTime gameTime)
        {

            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

        // Animacion representando al jugador
        //public Texture2D PlayerTexture;
        public Animation PlayerAnimation;
        // Posicion del jugador
        public Vector2 Position;

        // Estado del jugador
        public bool Active;

        // Cantidad de vida del jugador
        public int Health;

        // Obtener el ancho del jugador
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        // Obtener el alto del jugador
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }
    }
}
