using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    public enum Direction
    {
        NORTH,
        NORTHWEST,
        WEST,
        SOUTHWEST,
        SOUTH,
        SOUTHEAST,
        EAST,
        NORTHEAST
    }

    public static class DirectionExtensions
    {
        public static Direction opposite(this Direction d)
        {
            Direction dir = Direction.NORTH;
            switch (d)
            {
                case Direction.EAST: dir = Direction.WEST; break;
                case Direction.NORTHEAST: dir = Direction.SOUTHWEST; break;
                case Direction.NORTH: dir = Direction.SOUTH; break;
                case Direction.NORTHWEST: dir = Direction.SOUTHEAST; break;
                case Direction.WEST: dir = Direction.EAST; break;
                case Direction.SOUTHWEST: dir = Direction.NORTHEAST; break;
                case Direction.SOUTH: dir = Direction.NORTH; break;
                case Direction.SOUTHEAST: dir = Direction.NORTHWEST; break;
            }
            return dir;
        }

        public static Direction randomDirection(this Direction d)
        {
            Random r = new Random();
            int rand = r.Next(8);
            Direction dir = Direction.NORTH;
            switch (rand)
            {
                case 0: dir = Direction.WEST; break;
                case 1: dir = Direction.EAST; break;
                case 2: dir = Direction.NORTH; break;
                case 3: dir = Direction.SOUTH; break;
                case 4: dir = Direction.NORTHWEST; break;
                case 5: dir = Direction.SOUTHEAST; break;
                case 6: dir = Direction.SOUTHWEST; break;
                case 7: dir = Direction.NORTHEAST; break;
            }
            return dir;
        }
    }
}
