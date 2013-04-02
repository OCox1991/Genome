using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    //An Obstacle represents an obstacle in the world, something that a creature cannot move through but which has no benefits and does not move
    class Obstacle : WorldObject
    {
        /// <summary>
        /// Initialises the obstacle. This is empty since obstacle does not need to do anything but be placed in the world
        /// </summary>
        public Obstacle()
        {
        }
    }
}
