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
            GraphicsDevice graphicsDevice = Simulation.getGraphicsDeviceManager().GraphicsDevice;
            Color[] colourMap = Simulation.getColours();
            Color[] geneColours = new Color[100];

            Texture2D tex = new Texture2D(graphicsDevice, 10, 10);
            int nextFree = 0;
            for(int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 10; row++)
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

        public static void setTextures(Dictionary<TextureNames, Texture2D> theTextures)
        {
            textures = theTextures;
        }

        public static Texture2D getTexture(TextureNames texDescription)
        {
            return textures[texDescription];
        }

        public static void setFont(SpriteFont font)
        {
            spriteFont = font;
        }

        public static SpriteFont getFont()
        {
            return spriteFont;
        }

        public static void setSpriteBatch(SpriteBatch batch)
        {
            spriteBatch = batch;
        }

        public static SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public static Vector2 measureString(string s)
        {
            return spriteFont.MeasureString(s);
        }

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

        public static int getWindowWidth()
        {
            return 1024;
        }

        public static int getWindowHeight()
        {
            return 768;
        }

        public static bool getDrawCreaturesAsGenes()
        {
            return drawCreaturesAsGenes;
        }

        public static void setDrawCreaturesAsGenes(bool val)
        {
            drawCreaturesAsGenes = val;
        }
    }
}
