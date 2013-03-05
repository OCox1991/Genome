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
namespace Genome
{
    abstract class Button
    {
        private Vector2 topLeft;
        private Vector2 size;
        private string texString;
        private Texture2D texture;
        protected static MouseState prevState;
        protected static MouseState state;

        public Button(Vector2 topLeft, Vector2 size, string texString)
        {
            texture = Simulation.getTexture(texString);
            setLocation(topLeft, size);
        }

        public void setLocation(Vector2 topLeft, Vector2 size)
        {
            this.topLeft = topLeft;
            this.size = size;
        }

        public bool hovered(MouseState mouseState)
        {
            return mouseState.X > topLeft.X && mouseState.X < topLeft.X + size.X && mouseState.Y > topLeft.Y && mouseState.Y < topLeft.Y + size.Y;
        }

        public void update(GameTime gameTime, MouseState mouseState)
        {
            prevState = state;
            state = mouseState;
            if (hovered(state))
            {
                if (state.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed)
                {
                    clicked();
                }
            }
        }

        public Rectangle toRectangle()
        {
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)size.X, (int)size.Y);
        }

        public Texture2D getTexture()
        {
            return texture;
        }

        protected abstract void clicked();
    }
}
