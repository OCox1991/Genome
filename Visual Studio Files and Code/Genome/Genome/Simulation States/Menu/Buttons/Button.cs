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
    /// <summary>
    /// An  abstract superclass for all the buttons that handles getting clicked on as well as storing all the location information the button needs
    /// to store.
    /// </summary>
    abstract class Button
    {
        private Vector2 topLeft;
        private Vector2 size;
        private Texture2D texture;
        protected MouseState prevState;
        protected MouseState state;
        private bool visible;

        /// <summary>
        /// Sets up the button, setting it to a given location, size and texture
        /// </summary>
        /// <param name="topLeft">The location to place the button as a Vector2</param>
        /// <param name="size">The size to make the button as a Vector2</param>
        /// <param name="texString">The TextureNames enum associated with the texture for the button</param>
        public Button(Vector2 topLeft, Vector2 size, TextureNames texString)
        {
            texture = Display.getTexture(texString);
            setLocation(topLeft, size);
            visible = true;
        }

        /// <summary>
        /// Sets the location of the button to a given location and size
        /// </summary>
        /// <param name="topLeft">The location to set the button's location as</param>
        /// <param name="size">The size to make the button</param>
        public void setLocation(Vector2 topLeft, Vector2 size)
        {
            this.topLeft = topLeft;
            this.size = size;
        }

        /// <summary>
        /// Sets if the button is visible or not
        /// </summary>
        /// <param name="visible">A bool representing if the button should be visible or not. If a button is not visible it
        /// won't be updated a drawn</param>
        public void setVisible(bool visible)
        {
            this.visible = visible;
        }

        /// <summary>
        /// Checks if the button is visible
        /// </summary>
        /// <returns>True if the button is visible, false otherwise</returns>
        public bool isVisible()
        {
            return visible;
        }

        /// <summary>
        /// Checks if the button is being hovered over by the mouse
        /// </summary>
        /// <returns>True if the button is being hovered over, false otherwise</returns>
        public bool hovered()
        {
            return this.toRectangle().Contains(new Point((int)state.X, (int)state.Y));
        }

        /// <summary>
        /// Updates the button if it is visible
        /// </summary>
        /// <param name="gameTime">The time since the last update call was made by the Game class, not used in this method</param>
        public void update(GameTime gameTime)
        {
            if (isVisible())
            {
                prevState = state;
                state = Mouse.GetState();
                if (prevState.LeftButton == ButtonState.Released && state.LeftButton == ButtonState.Pressed && this.hovered())
                {
                    clicked();
                }
            }
        }

        /// <summary>
        /// Returns a rectangle with the dimensions and location of the button, used for drawing
        /// </summary>
        /// <returns>A rectangle with the size and location of the button</returns>
        public Rectangle toRectangle()
        {
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)size.X, (int)size.Y);
        }

        /// <summary>
        /// Returns the texture associated with this button
        /// </summary>
        /// <returns>The texture stored in this button</returns>
        public Texture2D getTexture()
        {
            return texture;
        }

        /// <summary>
        /// Gets the height of the button
        /// </summary>
        /// <returns>The Y dimension of the size of the button</returns>
        public float getHeight()
        {
            return size.Y;
        }

        /// <summary>
        /// Gets the width of the button
        /// </summary>
        /// <returns>The X dimension of the size of the button</returns>
        public float getWidth()
        {
            return size.X;
        }

        /// <summary>
        /// Gets the location of the button as a Vector2
        /// </summary>
        /// <returns>The location associated with this button</returns>
        public Vector2 getLocation()
        {
            return topLeft;
        }

        /// <summary>
        /// Deals with what happens when the button is clicked, must be overridden by any subclasses of Button
        /// </summary>
        public abstract void clicked();
    }
}
