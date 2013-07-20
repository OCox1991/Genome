using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// A Cell represents a single part of the genome, it has a dominant and recessive colour and a way to select one of these at random for breeding purposes
    /// </summary>
    class Cell
    {
        int dominantColour; //colour range is 1 to 7
        int nonDominantColour;

        /// <summary>
        /// Initialises the cell and selects which part will be the dominant and which part will be the non dominant colours
        /// </summary>
        /// <param name="colour1">A colour for the cell in the form of an int</param>
        /// <param name="colour2">Another colour for the cell in the form of an int</param>
        public Cell(int colour1, int colour2)
        {
            if (colour1 == colour2) //this could be gotten rid of if needed, but this has a better best case efficiency
            {
                dominantColour = colour1;
                nonDominantColour = colour2;
            }
            else
            {
                Boolean oneIsDominant = false;
                for (int i = 1; i <= 3; i++) //check if colour2 is in the next 3 numbers after colour 1
                {
                    if(((colour1 + i) % 7) == colour2)
                    {
                        oneIsDominant = true; //so if colour1 + [1|2|3] is equal to colour2 then colour2 is dominated by colour1, otherwise it isn't. % 7 is used to make it wrap so 6 checks 0 1 2.
                    }
                }
                if (oneIsDominant)
                {
                    dominantColour = colour1;
                    nonDominantColour = colour2;
                }
                else
                {
                    dominantColour = colour2;
                    nonDominantColour = colour1;
                }
            }
        }

        #region Methods
        /// <summary>
        /// Gets the dominant colour of the cell, used when drawing the cell
        /// </summary>
        /// <returns>The dominant colour of the cell</returns>
        public int getDomColour()
        {
            return dominantColour;
        }

        /// <summary>
        /// Gets a random part of the cell, with the odds skewed towards the dominant part.
        /// </summary>
        /// <returns>A random part of the cell</returns>
        public int getRandomPart()
        {
            Random r = new Random();
            int col = -1;
            int prob = r.Next(100) + 1;
            if(prob <=60) //VARIABLE
            {
                col = dominantColour;
            }
            else
            {
                col = nonDominantColour;
            }
            return col;
        }
        #endregion
    }
}
