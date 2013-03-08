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
        private Texture2D texture;
        protected MouseState prevState;
        protected MouseState state;

        public Button(Vector2 topLeft, Vector2 size, TextureNames texString)
        {
            texture = Display.getTexture(texString);
            setLocation(topLeft, size);
        }

        public void setLocation(Vector2 topLeft, Vector2 size)
        {
            this.topLeft = topLeft;
            this.size = size;
        }

        public bool hovered()
        {
            return this.toRectangle().Contains(new Point((int)state.X, (int)state.Y));
        }

        public void update(GameTime gameTime)
        {
            prevState = state;
            state = Mouse.GetState();
            if (prevState.LeftButton == ButtonState.Released && state.LeftButton == ButtonState.Pressed && this.hovered())
            {
                clicked();
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

        public float getHeight()
        {
            return size.Y;
        }

        public float getWidth()
        {
            return size.X;
        }

        public Vector2 getLocation()
        {
            return topLeft;
        }

        protected abstract void clicked();
    }
}
