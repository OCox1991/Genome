using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
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

        private WorldDrawer drawer;
        private WorldInputHandler inputHandler;

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
            creatureList.Sort(sortCreaturesBySpeed);
            for (int i = 0; i < creatureList.Count; i++)
            {
                Creature c = creatureList[i];
                int[] clearTile = getRandomClearTile();

                c.setLocation(clearTile[0], clearTile[1]);
                getTile(clearTile[1], clearTile[0]).addCreature(c);
            }
        }

        /// <summary>
        /// Finds a random clear tile, or failing that, the next clear tile from the top left
        /// </summary>
        /// <returns>A randomly found clear tile or the next clear tile</returns>
        private int[] getRandomClearTile()
        {
            int xLoc = -1;
            int yLoc = -1;
            int attempts = 0;
            bool found = false;

            while (!found && attempts < 5) //This is the number of times the program will attempt to place the creature before giving up
            {
                xLoc = randomNumberGenerator.Next(worldX);
                yLoc = randomNumberGenerator.Next(worldY);
                if (tileIsClear(yLoc, xLoc))
                {
                    found = true;
                }
                else
                {
                    attempts++;
                }
            }
            if (!found) //then find the top left most clear tile
            {
                xLoc = 0;
                yLoc = 0;
                while (!found)
                {
                    while (yLoc < tiles.Length)
                    {
                        while (xLoc < tiles[yLoc].Length)
                        {
                            xLoc++;
                            if (tileIsClear(yLoc, xLoc))
                            {
                                xLoc--;
                                yLoc--;
                                found = true;
                            }
                        }
                        yLoc++;
                    }
                }
            }
            return new int[] { xLoc, yLoc };
        }

        /// <summary>
        /// Scans a specified radius from a given location and returns all creatures and food sources within that radius
        /// </summary>
        /// <param name="locationXY">The location to centre the scan on</param>
        /// <param name="radius">The radius to extend the scan</param>
        /// <returns>A list of all the interesting things found in the scan</returns>
        public ArrayList scan(int[] locationXY, int radius)
        {
            ArrayList a = new ArrayList();
            a[0] = new List<Creature>();
            a[1] = new List<Plant>();
            a[2] = new List<Remains>();
            a[3] = new List<Obstacle>();

            int x = locationXY[0];
            int y = locationXY[1];
            for (int lookX = x - radius; lookX <= x + radius; lookX++)
            {
                if (lookX < 0)
                {
                    lookX = 0;
                }
                else if (lookX > worldX)
                {
                    lookX = x + radius + 1;
                }
                else
                {
                    for (int lookY = y - radius; lookY <= y + radius; lookY++)
                    {
                        if (lookY < 0)
                        {
                            lookY = 0;
                        }
                        else if (lookY > worldY)
                        {
                            lookY = y + radius + 1;
                        }
                        else
                        {
                            if (!tileIsClear(lookY, lookX))
                            {
                                Tile t = getTile(lookY, lookX);
                                if (t.creaturePresent())
                                {
                                    Creature c = t.getCreature();
                                }
                                else if (t.plantPresent())
                                {
                                    List<Plant> lp = (List<Plant>)a[1];
                                    lp.Add(t.getPlant());
                                }
                                else if (t.remainsPresent())
                                {
                                    List<Remains> lr = (List<Remains>)a[2];
                                    lr.Add(t.getRemains());
                                }
                                else if (t.obstaclePresent())
                                {
                                    List<Obstacle> lo = (List<Obstacle>)a[3];
                                    lo.Add(t.getObstacle());
                                }
                            }
                        }
                    }
                }
            }
            return a;
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
        /// Adds a creature at the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        public void addCreature(int x, int y, Creature c)
        {
            getTile(y, x).addCreature(c);
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
            foreach (Plant p in plantList)
            {
                p.tick();
            }
            foreach (Remains r in remainsList)
            {
                r.tick();
                if (r.fullyDecayed())
                {
                    int[] xy = r.getLocationXY();
                    getTile(xy[1], xy[0]).clearTile();
                    remainsList.Remove(r);
                    r.setLocation(-1, -1);
                }
            }
            for (int i = 0; i < creatureList.Count; i++)
            {
                creatureList[i].tick();
            }

            Simulation.tick();
        }

        /// <summary>
        /// Handles the code for killing a specified creature
        /// </summary>
        /// <param name="c">The creature to kill</param>
        public void killCreature(Creature c)
        {
            int[] loc = c.getLocationXY();
            creatureList.Remove(c);
            getTile(loc[1], loc[0]).clearTile();
            c.setLocation(-1, -1);

            Remains r = new Remains(randomNumberGenerator);
            r.setLocation(loc[0], loc[1]);
            getTile(loc[1], loc[0]).addRemains(r);
            deadList.Push(c);
        }

        /// <summary>
        /// Gets the list of currently live creatures
        /// </summary>
        /// <returns>The list of live creatures</returns>
        public List<Creature> getLiveCreatures()
        {
            return creatureList;
        }

        /// <summary>
        /// Gets the list of currently dead creatures
        /// </summary>
        /// <returns>The list of dead creatures</returns>
        public Stack<Creature> getDeadCreatures()
        {
            return deadList;
        }

        /// <summary>
        /// Checks if there is a creature in a specified tile
        /// </summary>
        /// <param name="x">The x index of the tile to look at</param>
        /// <param name="y">The y index of the tile to look at</param>
        /// <returns></returns>
        public bool creatureAt(int x, int y)
        {
            return getTile(y, x).creaturePresent();
        }

        /// <summary>
        /// Gets the creature in a specified tile
        /// </summary>
        /// <param name="x">The x index of the tile</param>
        /// <param name="y">The y index of the tile</param>
        /// <returns></returns>
        public Creature getCreatureAt(int x, int y)
        {
            return getTile(y, x).getCreature();
        }

        /// <summary>
        /// Clears a specified tile, removing all of its contents
        /// </summary>
        /// <param name="x">The x index of the tile</param>
        /// <param name="y">The y index of the tile</param>
        public void clearTile(int x, int y)
        {
            getTile(y, x).clearTile();
        }

        /// <summary>
        /// A method to sort the creatures by their speed
        /// </summary>
        /// <returns></returns>
        public static int sortCreaturesBySpeed(Creature c1, Creature c2)
        {
            int val = 0;
            if (c1 == null && c2 == null)
            {
                val = 0;
            }
            else if (c1 == null)
            {
                val = -1;
            }
            else if (c2 == null)
            {
                val = 1;
            }
            else
            {
                if (c1.getSpeed() < c2.getSpeed())
                {
                    val = -1;
                }
                else if (c1.getSpeed() > c2.getSpeed())
                {
                    val = 1;
                }
                else
                {
                    val = 0;
                }
            }
            return val;
        }

        public override void update(GameTime gameTime)
        {
            for (int i = 0; i < Simulation.getSpeed(); i++)
            {
                tick(); //tick x times based on the speed specified by the simulation
            }
            inputHandler.update(gameTime);
        }

        public override void draw()
        {
            drawer.draw();
        }
    }
}
