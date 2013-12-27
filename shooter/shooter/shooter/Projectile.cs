using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shooter
{
    class Projectile
    {
        // Imagen del proyectil
        public Texture2D Texture;

        // Posicion del proyectil
        public Vector2 Position;

        // Estado del proyectil
        public bool Active;

        // Cantidad de daño del proyectil
        public int Damage;

        // Limite visible del juego
        Viewport viewport;

        // Ancho del proyectil
        public int Width
        {
            get { return Texture.Width; }
        }

        // Alto del proyectil
        public int Height
        {
            get { return Texture.Height; }
        }

        // Velocidad de desplazamiento
        float projectileMoveSpeed;
       
        public void Initialize(Viewport viewport,
    Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;
            Active = true;
            Damage = 2;
            projectileMoveSpeed = 8f;
        }

        public void Update()
        {
            // Projectiles always move to the right
            Position.X += projectileMoveSpeed;
            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
                Active = false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
            new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
