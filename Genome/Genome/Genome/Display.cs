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
        public Display()
        {
        }

        /// <summary>
        /// Produces a 10x10 texture representation of a given genome, based on a method found at: 
        /// http://core-fusion.googlecode.com/svn-history/r154/trunk/Game2084/CoreFusion/Graphics/ColorTexture.cs
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        public static Texture2D drawGenome(GraphicsDevice graphicsDevice, Gene dna)
        {
            Color[] colourMap = Simulation.getColours();
            Color[] geneColours = new Color[100];

            Texture2D tex = new Texture2D(graphicsDevice, 10, 10);
            int nextFree = 0;
            for(int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 10; row++)
                {
                    geneColours[nextFree] = new Color(colourMap[dna.getColour(row, col) - 1].ToVector3());

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
        public static Texture2D drawGenome(GraphicsDevice graphicsDevice, Creature creature)
        {
            return drawGenome(graphicsDevice, creature.getDna());
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
    }
}
