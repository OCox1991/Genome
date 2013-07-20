using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// A Tile represents a single space in the world and can hold a variety of objects like creatures, plants, remains or obstacles.
    /// </summary>
    class Tile
    {
        private Creature creature;
        private Plant plant;
        private Remains remains;
        private Obstacle obstacle;
        private bool isBlocked;

        /// <summary>
        /// Initialises the tile, calls the clearTile method to set up all the variables
        /// </summary>
        public Tile()
        {
            clearTile();
        }

        /// <summary>
        /// Returns if the Tile is impassible, using the isBlocked variable
        /// </summary>
        /// <returns>true if there is anything in the tile, false otherwise</returns>
        public bool isImpassible()
        {
            return isBlocked;
        }

        /// <summary>
        /// Adds a creature to the tile, sets the creature variable to the specified creature and sets the tile to be blocked
        /// </summary>
        /// <param name="c">The creature to add to the tile</param>
        public void addCreature(Creature c)
        {
            creature = c;
            isBlocked = true;
        }

        /// <summary>
        /// Gets the creature currently in the tile
        /// </summary>
        /// <returns>The creature currently in the tile or null if the tile doesn't contain a creature</returns>
        public Creature getCreature()
        {
            return creature;
        }

        /// <summary>
        /// Adds a plant to the tile, sets the plant variable to the specified plant and sets the tile to be blocked
        /// </summary>
        /// <param name="c">The plant to add to the tile</param>
        public void addPlant(Plant p)
        {
            plant = p;
            isBlocked = true;
        }

        /// <summary>
        /// Gets the plant currently in the tile
        /// </summary>
        /// <returns>The plant currently in the tile or null if the tile doesn't contain a plant</returns>
        public Plant getPlant()
        {
            return plant;
        }

        /// <summary>
        /// Adds remains to the tile, sets the remains variable to the specified remains and sets the tile to be blocked
        /// </summary>
        /// <param name="c">The remains to add to the tile</param>
        public void addRemains(Remains r)
        {
            remains = r;
            isBlocked = true;
        }

        /// <summary>
        /// Gets the remains currently in the tile
        /// </summary>
        /// <returns>The remains currently in the tile or null if the tile doesn't contain remains</returns>
        public Remains getRemains()
        {
            return remains;
        }

        /// <summary>
        /// Adds an obstacle to the tile, sets the obstacle variable to the specified obstacle and sets the tile to be blocked
        /// </summary>
        /// <param name="c">The obstacle to add to the tile</param>
        public void addObstacle(Obstacle o)
        {
            obstacle = o;
            isBlocked = true;
        }

        /// <summary>
        /// Gets the obstacle currently in the tile
        /// </summary>
        /// <returns>The obstacle currently in the tile or null if the tile doesn't contain obstacle</returns>
        public Obstacle getObstacle()
        {
            return obstacle;
        }

        /// <summary>
        /// Checks if a plant is present in the tile, checks if the plant variable is null
        /// </summary>
        /// <returns>true if there is a plant in the tile, false otherwise</returns>
        public bool plantPresent()
        {
            return plant != null;
        }

        /// <summary>
        /// Checks if remains are present in the tile, checks if the remains variable is null
        /// </summary>
        /// <returns>true if there are remains in the tile, false otherwise</returns>
        public bool remainsPresent()
        {
            return remains != null;
        }

        /// <summary>
        /// Checks if a creature is present in the tile, checks if the creature variable is null
        /// </summary>
        /// <returns>true if there is a creature in the tile, false otherwise</returns>
        public bool creaturePresent()
        {
            return creature != null;
        }

        /// <summary>
        /// Checks if an obstacle is present in the tile, checks if the obstacle variable is null
        /// </summary>
        /// <returns>true if there is an obstacle in the tile, false otherwise</returns>
        public bool obstaclePresent()
        {
            return obstacle != null;
        }

        /// <summary>
        /// Clears the tile, sets all the variables for tile contents to null, sets isBlocked to false.
        /// </summary>
        public void clearTile()
        {
            creature = null;
            plant = null;
            remains = null;
            obstacle = null;
            isBlocked = false;
        }
    }
}
