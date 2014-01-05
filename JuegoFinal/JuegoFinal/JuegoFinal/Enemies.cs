using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JuegoFinal
{
    class Enemies
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;

        public bool isVisible = true;

        Random random = new Random();
        int randX, randY;

        public Enemies(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;

            randY = random.Next(-4, 4);
            randX = random.Next(-4, -1);

            velocity = new Vector2(randX, randY);
        }
        public void Update(GraphicsDevice graphics)
        {
            position += velocity;

            if (position.Y <= 0 || position.Y >= graphics.Viewport.Height - texture.Height)
                velocity.Y = -velocity.Y;

            if (position.X < 0 - texture.Width)
                isVisible = false;
        }
        public void Draw(SpriteBatch spriteBach) {
            spriteBach.Draw(texture, position, Color.White);
        }
    }
}

