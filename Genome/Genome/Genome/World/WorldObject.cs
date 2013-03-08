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
            int[] xy = getRelativeLocation(o);

            return Math.Abs(xy[0]) + Math.Abs(xy[1]);
        }

        public double getEuclideanDistanceFrom(WorldObject o)
        {
            int[] xy = getRelativeLocation(o);
            int a2 = xy[0] * xy[0];
            int b2 = xy[1] * xy[1];

            return Math.Sqrt(a2 + b2);
        }

        public int[] getRelativeLocation(WorldObject o)
        {
            int locX = o.getLocationXY()[0] - this.getLocationXY()[0];
            int locY = o.getLocationXY()[1] - this.getLocationXY()[1];
            return new int[] { locX, locY };
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
