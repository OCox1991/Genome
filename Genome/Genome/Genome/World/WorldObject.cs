using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    abstract class WorldObject
    {
        private Vector2 location;

        public WorldObject()
        {
        }

        public void setLocation(int x, int y)
        {
            location = new Vector2(x, y);
        }

        public int[] getLocationXY()
        {
            return new int[] { (int)location.X, (int)location.Y };
        }

        /// <summary>
        /// Gets the manhattan distance of this object from a given object
        /// </summary>
        /// <param name="o">The object to calculate the distance to</param>
        /// <returns>The distance as an int</returns>
        public int getDistanceFrom(WorldObject o)
        {
            return Math.Abs(this.getLocationXY()[0] - o.getLocationXY()[0]) + Math.Abs(this.getLocationXY()[1] - o.getLocationXY()[1]);
        }

        public int[] getRelativeLocation(WorldObject o)
        {
            return new int[] { o.getLocationXY()[0] - this.getLocationXY()[0], o.getLocationXY()[1] - this.getLocationXY()[1] };
        }

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
    }
}
