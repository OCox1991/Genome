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
    /// The Display class stores variables and methods that are used in various ways to draw the SimulationStates
    /// </summary>
    class Display
    {
        private static Dictionary<TextureNames, Texture2D> textures = new Dictionary<TextureNames, Texture2D>();
        private static SpriteFont spriteFont;
        private static SpriteBatch spriteBatch;
        private static bool drawCreaturesAsGenes = false;

        private Display()
        {
        }

        /// <summary>
        /// Produces a 10x10 texture representation of a given genome, based on a method found at: 
        /// http://core-fusion.googlecode.com/svn-history/r154/trunk/Game2084/CoreFusion/Graphics/ColorTexture.cs
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        public static Texture2D drawGenome(Gene dna)
        {
            GraphicsDeviceManager gdm = Simulation.getGraphicsDeviceManager();
            GraphicsDevice graphicsDevice = gdm.GraphicsDevice;
            Color[] colourMap = Simulation.getColours();
            Color[] geneColours = new Color[dna.getSizeX() * dna.getSizeY()];

            Texture2D tex = new Texture2D(graphicsDevice, dna.getSizeX(), dna.getSizeY());
            int nextFree = 0;
            for(int col = 0; col < dna.getSizeY(); col++)
            {
                for (int row = 0; row < dna.getSizeX(); row++)
                {
                    Color c = new Color(colourMap[dna.getColour(row, col)].ToVector3());
                    geneColours[nextFree] = c;

                    nextFree++;
                }
            }

            tex.SetData(geneColours);
            return tex;
        }

        /// <summary>
        /// A method that takes a creature and returns a 10 x 10 texture representation of the creature's genome
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public static Texture2D drawGenome(Creature creature)
        {
            return drawGenome(creature.getDna());
        }

        /// <summary>
        /// Returns a 1x1 texture of the specified colour
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected Texture2D getColourAsTexture(GraphicsDevice graphicsDevice, Color c)
        {
            Texture2D tex = new Texture2D(graphicsDevice, 1, 1);
            Color[] colourMap = new Color[1];
            colourMap[0] = new Color(c.ToVector3());
            tex.SetData(colourMap);
            return tex;
        }

        /// <summary>
        /// The size of the sprite for the tiles, used in some calculations
        /// </summary>
        /// <returns>The size of the sprite for the tiles, 30</returns>
        public static float getTileSize()
        {
            return 40;
        }

        /// <summary>
        /// Sets the Dictionary of loaded textures that the Display class stores to a specified Dictionary
        /// </summary>
        /// <param name="theTextures">The Dictionary to set the ones stored in the Display to</param>
        public static void setTextures(Dictionary<TextureNames, Texture2D> theTextures)
        {
            textures = theTextures;
        }

        /// <summary>
        /// Gets a texture associated with a specified TextureNames item
        /// </summary>
        /// <param name="texDescription">The TextureNames item that describes the texture</param>
        /// <returns>The texture associated with that TextureNames item</returns>
        public static Texture2D getTexture(TextureNames texDescription)
        {
            return textures[texDescription];
        }

        /// <summary>
        /// Sets the spriteFont stored by this class to a specified value
        /// </summary>
        /// <param name="font">The spriteFont to set this item to</param>
        public static void setFont(SpriteFont font)
        {
            spriteFont = font;
        }

        /// <summary>
        /// Gets the spriteFont stored by the Display class
        /// </summary>
        /// <returns>The SpriteFont stored in the Display class</returns>
        public static SpriteFont getFont()
        {
            return spriteFont;
        }

        /// <summary>
        /// Sets the SpriteBatch stored by this class to a specified value
        /// </summary>
        /// <param name="font">The SpriteBatch to set this item to</param>
        public static void setSpriteBatch(SpriteBatch batch)
        {
            spriteBatch = batch;
        }

        /// <summary>
        /// Gets the SpriteBatch stored by the Display class
        /// </summary>
        /// <returns>The SpriteBatch stored in the Display class</returns>
        public static SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        /// <summary>
        /// Uses the sprite font to measure a provided string and returns the size as a Vector2
        /// </summary>
        /// <param name="s">The string to measure</param>
        /// <returns>The size of the string as a Vector2</returns>
        public static Vector2 measureString(string s)
        {
            return spriteFont.MeasureString(s);
        }

        /// <summary>
        /// Draws a button, using the size and location variables in the button to work out where to put it
        /// </summary>
        /// <param name="b">The Button to draw</param>
        public static void drawButton(Button b)
        {
            if (b.isVisible())
            {
                spriteBatch.Begin();
                if (b.hovered())
                {
                    spriteBatch.Draw(b.getTexture(), new Rectangle((int)b.getLocation().X, (int)b.getLocation().Y, (int)b.getWidth(), (int)b.getHeight()), Color.Gray);
                }
                else
                {
                    spriteBatch.Draw(b.getTexture(), new Rectangle((int)b.getLocation().X, (int)b.getLocation().Y, (int)b.getWidth(), (int)b.getHeight()), Color.White);
                }
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draws a text button using the size, location and text stored in the text button
        /// </summary>
        /// <param name="b">The TextButton to draw</param>
        public static void drawButton(TextButton b)
        {
            if (b.isVisible())
            {
                drawButton((Button)b);
                spriteBatch.Begin();
                Vector2 newLoc = new Vector2(b.getLocation().X + 5, b.getLocation().Y + 5);
                spriteBatch.DrawString(spriteFont, b.Text, newLoc, Color.Black);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Gets the width of the window
        /// </summary>
        /// <returns>The width of the window in pixels as an int</returns>
        public static int getWindowWidth()
        {
            return 1024;
        }

        /// <summary>
        /// Gets the height of the window
        /// </summary>
        /// <returns>The height of the window in pixels as an int</returns>
        public static int getWindowHeight()
        {
            return 768;
        }

        /// <summary>
        /// Gets a bool representing if the Creatures visible in the World should be drawn as icons or
        /// as their genes
        /// </summary>
        /// <returns>A bool representing whether to draw the Creatures in the world as Creatures or as genes</returns>
        public static bool getDrawCreaturesAsGenes()
        {
            return drawCreaturesAsGenes;
        }

        /// <summary>
        /// Sets whether to draw the Creatures in the world as creatures or as their genes
        /// </summary>
        /// <param name="val">A bool to set the parameter that governs what the creatures are drawn as to</param>
        public static void setDrawCreaturesAsGenes(bool val)
        {
            drawCreaturesAsGenes = val;
        }
    }
}
