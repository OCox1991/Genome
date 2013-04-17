using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// Direction represents the various directions a creature can move
    /// </summary>
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

    /// <summary>
    /// Extension methods for the Directions to allow us to find other directions related to the original
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Finds the opposite direction to a specified direction
        /// </summary>
        /// <param name="d">The direction to find the opposite to</param>
        /// <returns>The opposite direction to the one given, for example NORTH would return SOUTH</returns>
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

        /// <summary>
        /// Returns the direction to the right of a given direction
        /// </summary>
        /// <param name="d">The direction to find the direction to the right of</param>
        /// <returns>The direction 1 turn to the right from the specified direction for example NORTH would give NORTHEAST</returns>
        public static Direction right(this Direction d)
        {
            Direction dir = Direction.NORTH;
            switch (d)
            {
                case Direction.EAST: dir = Direction.SOUTHEAST; break;
                case Direction.NORTHEAST: dir = Direction.EAST; break;
                case Direction.NORTH: dir = Direction.NORTHEAST; break;
                case Direction.NORTHWEST: dir = Direction.NORTH; break;
                case Direction.WEST: dir = Direction.NORTHWEST; break;
                case Direction.SOUTHWEST: dir = Direction.WEST; break;
                case Direction.SOUTH: dir = Direction.SOUTHWEST; break;
                case Direction.SOUTHEAST: dir = Direction.SOUTH; break;
            }
            return dir;
        }

        public static Direction left(this Direction d)
        {
            Direction dir = Direction.NORTH;
            switch (d)
            {
                case Direction.EAST: dir = Direction.NORTHEAST; break;
                case Direction.NORTHEAST: dir = Direction.NORTH; break;
                case Direction.NORTH: dir = Direction.NORTHWEST; break;
                case Direction.NORTHWEST: dir = Direction.WEST; break;
                case Direction.WEST: dir = Direction.SOUTHWEST; break;
                case Direction.SOUTHWEST: dir = Direction.SOUTH; break;
                case Direction.SOUTH: dir = Direction.SOUTHEAST; break;
                case Direction.SOUTHEAST: dir = Direction.EAST; break;
            }
            return dir;
        }

        /// <summary>
        /// Returns a random direction
        /// </summary>
        /// <param name="d">A given direction, which is not actually used by the method</param>
        /// <returns>A random direction</returns>
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
