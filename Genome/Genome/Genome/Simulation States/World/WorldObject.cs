using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// A WorldObject represents any object that can be placed in the world
    /// </summary>
    abstract class WorldObject
    {
        private Vector2 location;

        /// <summary>
        /// Initialises the WorldObject, but doesn't do anything else
        /// </summary>
        public WorldObject()
        {
        }

        /// <summary>
        /// Sets the location of the WorldObject
        /// </summary>
        /// <param name="x">The x location of the WorldObject</param>
        /// <param name="y">The y location of the WorldObject</param>
        public void setLocation(int x, int y)
        {
#if DEBUG
            if (x == -1 && y == -1)
            {

            }
#endif
            location = new Vector2(x, y);
        }

        /// <summary>
        /// Gets the location of the WorldObject as an array of ints
        /// </summary>
        /// <returns>An array of ints representing the location of the WorldObject, with 
        /// the x location as the 0th element and the y location as the 1st element</returns>
        public int[] getLocationXY()
        {
            return new int[] { (int)location.X, (int)location.Y };
        }

        /// <summary>
        /// Gets the manhattan distance of this object from a given object
        /// </summary>
        /// <param name="o">The object to calculate the distance to</param>
        /// <returns>The distance as an int</returns>
        public int getDistanceFrom(WorldObject o) //a + b = c
        {
            int[] xy = getRelativeLocation(o);

            return Math.Abs(xy[0]) + Math.Abs(xy[1]);
        }

        /// <summary>
        /// Gets the euclidean distance of this object from a given object
        /// </summary>
        /// <param name="o">The object to get the distance from</param>
        /// <returns>The euclidean distance from this object to the specified object as a
        /// double</returns>
        public double getEuclideanDistanceFrom(WorldObject o) //a^2 + b^2 = c^2
        {
            int[] xy = getRelativeLocation(o);
            int a2 = xy[0] * xy[0];
            int b2 = xy[1] * xy[1];

            return Math.Sqrt(a2 + b2);
        }

        /// <summary>
        /// Gets the relative location of a specified WorldObject from this one, that is, gets the
        /// location of the specified WorldObject as if this WorldObject was as 0,0
        /// </summary>
        /// <param name="o">The WorldObject to get the relative location of</param>
        /// <returns>The relative location of the specified object as a 2 element array of ints</returns>
        public int[] getRelativeLocation(WorldObject o)
        {
            int locX = o.getLocationXY()[0] - this.getLocationXY()[0];
            int locY = o.getLocationXY()[1] - this.getLocationXY()[1];
            return new int[] { locX, locY };
        }

        /// <summary>
        /// Gets the direction to a specifed WorldObject from this one
        /// </summary>
        /// <param name="o">The object to get the direction to</param>
        /// <returns>The direction from this object to get to the specified object</returns>
        public Direction getDirectionTo(WorldObject o)
        {
            int[] relativeLoc = getRelativeLocation(o);
            Vector2 v = new Vector2(relativeLoc[0], relativeLoc[1]);
            if (Math.Abs(v.X) > Math.Abs(v.Y))
            {
                Vector2 temp = new Vector2(v.X / Math.Abs(v.X), v.Y / Math.Abs(v.X));
                v = temp;
            }
            else
            {
                Vector2 temp = new Vector2(v.X / Math.Abs(v.Y), v.Y / Math.Abs(v.Y));
                v = temp;
            }

            Direction d = Direction.EAST;
            //now work out the direction
            if (v.X == 1) // -->
            {
                if (v.Y > 0.5) // V
                {
                    d = Direction.SOUTHEAST;
                }
                else if (v.Y < -0.5) // ^
                {
                    d = Direction.NORTHEAST;
                }
                else
                {
                    d = Direction.EAST;
                }
            }
            else if (v.X == -1) // <--
            {
                if (v.Y > 0.5) // V
                {
                    d = Direction.SOUTHWEST;
                }
                else if (v.Y < -0.5) // ^
                {
                    d = Direction.NORTHWEST;
                }
                else
                {
                    d = Direction.WEST;
                }
            }
            else if (v.Y == 1) // V
            {
                if (v.X > 0.5) // -->
                {
                    d = Direction.SOUTHEAST;
                }
                else if (v.X < -0.5) // <--
                {
                    d = Direction.SOUTHWEST;
                }
                else
                {
                    d = Direction.SOUTH;
                }
            }
            else if (v.Y == -1) // ^
            {
                if (v.X > 0.5) // -->
                {
                    d = Direction.NORTHEAST;
                }
                else if (v.X < -0.5) // <--
                {
                    d = Direction.NORTHWEST;
                }
                else
                {
                    d = Direction.NORTH;
                }
            }
            return d;
        }

        /// <summary>
        /// Checks if this object is adjacent to another object
        /// </summary>
        /// <param name="o">The object to check if this object is adjacent to</param>
        /// <returns>True if this object is adjacent to the other and false otherwise</returns>
        public bool isAdjacent(WorldObject o)
        {
            int[] loc = this.getRelativeLocation(o);
            int locX = loc[0];
            int locY = loc[1];
            bool b = false;
            if (Math.Abs(locX) <= 1 && Math.Abs(locY) <= 1)
            {
                b = true;
            }
            return b;
        }
    }
}
