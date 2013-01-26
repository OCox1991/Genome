using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
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
    class WorldState : SimulationState
    {
        private Tile[][] tiles;
        private int worldX = 1000;
        private int worldY = 1000;

        private List<Creature> creatureList;
        private List<Plant> plantList;
        private List<Remains> remainsList;

        private Stack<Creature> deadList;

        private Random randomNumberGenerator;
        private int seed = 0;

        /// <summary>
        /// Starts a new world with a random seed
        /// </summary>
        public WorldState()
        {
            Random r = new Random();
            seed = r.Next(100000);
            randomNumberGenerator = new Random(seed);
            deadList = new Stack<Creature>();
            setUpWorld();
        }

        public void setUpWorld()
        {
            plantList = new List<Plant>();
            remainsList = new List<Remains>();
            tiles = new Tile[worldX][];

            for (int i = 0; i < worldX; i++)
            {
                tiles[i] = new Tile[worldY];

                for (int j = 0; j < worldY; j++)
                {
                    tiles[i][j] = new Tile();
                }
            }

            populateWorld();
        }

        /// <summary>
        /// Populates the world by adding plants and obstacles
        /// </summary>
        private void populateWorld()
        {

        }

        /// <summary>
        /// Adds creatures to the world. 1 Override to allow the user to provide a list of creatures to add
        /// </summary>
        public void addCreatures()
        {
            creatureList = new List<Creature>();
            for (int i = 0; i < Simulation.getPopulation(); i++)
            {
                Creature c = new Creature();
                creatureList.Add(c);
            }
            placeCreatures();
        }

        /// <summary>
        /// Adds a given list of creatures to the world
        /// </summary>
        /// <param name="creatures">The creatures to add to the world</param>
        public void addCreatures(List<Creature> creatures)
        {
            creatureList = creatures;
            placeCreatures();
        }

        /// <summary>
        /// Places all the creatures in the creature list at random locations in the world.
        /// </summary>
        private void placeCreatures()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                Creature c = creatureList[i];
                int attempts = 0;
                bool placed = false;
                while (!placed && attempts < 5) //This is the number of times the program will attempt to place the creature before giving up
                {
                    int xLoc = randomNumberGenerator.Next(worldX);
                    int yLoc = randomNumberGenerator.Next(worldY);
                    if (tileIsClear(yLoc, xLoc))
                    {
                        c.setLocation(xLoc, yLoc);
                        getTile(yLoc, xLoc).addCreature(c);
                        placed = true;
                    }
                    else
                    {
                        attempts++;
                    }
                }
                if (!placed) //if it is not placed that means it must have tried the specified number and still failed. If that is the case then place the creature in the first free spot.
                {

                }
            }
        }

        /// <summary>
        /// Scans a specified radius from a given location and returns all creatures and food sources within that radius
        /// </summary>
        /// <param name="locationXY">The location to centre the scan on</param>
        /// <param name="radius">The radius to extend the scan</param>
        /// <returns>A list of all the interesting things found in the scan</returns>
        public ArrayList scan(int[] locationXY, int radius)
        {

        }

        /// <summary>
        /// Gets a specified tile given a row and column
        /// </summary>
        /// <param name="row">The row of the tile to look at</param>
        /// <param name="col">The column of the tile to look at</param>
        /// <returns>The tile found at the given row and column</returns>
        private Tile getTile(int row, int col)
        {
            return tiles[col][row];
        }

        /// <summary>
        /// Checks if a specified tile is clear
        /// </summary>
        /// <param name="row">The row index of the specified tile</param>
        /// <param name="col">The column index of the specified tile</param>
        /// <returns>True is the tile is empty, false otherwise</returns>
        public bool tileIsClear(int row, int col)
        {
            return getTile(row, col).isImpassible();
        }

        /// <summary>
        /// Ticks the world foward
        /// </summary>
        public void tick()
        {
            foreach (Creature c in creatureList)
            {
                c.tick();
            }
            foreach (Plant p in plantList)
            {
                p.tick();
            }
            foreach (Remains r in remainsList)
            {
                r.tick();
            }

            //tick the simulation
        }

        /// <summary>
        /// Handles the code for killing a specified creature
        /// </summary>
        /// <param name="c">The creature to kill</param>
        public void killCreature(Creature c)
        {
            creatureList.Remove(c);
            int[] loc = c.getLocation();
            c.setLocation(-1, -1);
            getTile(loc[1], loc[0]).addRemains(new Remains());
            deadList.Push(c);
        }
    }
}
