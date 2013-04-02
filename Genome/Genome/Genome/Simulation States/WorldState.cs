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

        private WorldInputHandler inputHandler;

        private List<Creature> creatureList;
        private List<Plant> plantList;
        private List<Remains> remainsList;

        private Stack<Creature> deadList;

        private static Random randomNumberGenerator;
        public static Random RandomNumberGenerator
        {
            get { return randomNumberGenerator; }
        }
        private int seed;
        public int Seed
        {
            get { return seed; }
        }
        private int creatureUpdated = 0;
        private int numToUpdatePerTick = 10;

        private long actSpeed = TimeSpan.TicksPerSecond/16;
        private long timer;
        /// <summary>
        /// Starts a new world with a random seed, 3 overloads to allow the specifying of a seed, a list of creatures or both
        /// </summary>
        public WorldState()
        {
            Random r = new Random();
            seed = r.Next(1000000);
            randomNumberGenerator = new Random(seed);
            setUpWorld();
            addCreatures();
        }

        /// <summary>
        /// Starts a new world with a random seed and a specified list of creatures
        /// </summary>
        /// <param name="creatures"></param>
        public WorldState(List<Creature> creatures)
        {
            Random r = new Random();
            seed = r.Next(1000000);
            randomNumberGenerator = new Random(seed);
            setUpWorld();
            addCreatures(creatures);
        }

        /// <summary>
        /// Starts a new world with a specified seed and list of creatures
        /// </summary>
        /// <param name="seed">The seed to initialise the random number generator with</param>
        /// <param name="creatures">The list of creatures to populate the world with</param>
        public WorldState(int seed, List<Creature> creatures)
        {
            this.seed = seed;
            randomNumberGenerator = new Random(seed);
            setUpWorld();
            addCreatures(creatures);
        }

        /// <summary>
        /// Resets the world and restarts it with a specified list of creatures
        /// </summary>
        /// <param name="newCreatureList">The list of creatures to use as the new creature list</param>
        public void reset(List<Creature> newCreatureList)
        {
            List<Creature> allCreatures = new List<Creature>();
            allCreatures.AddRange(creatureList.ToArray<Creature>());
            foreach (Creature c in allCreatures)
            {
                killCreature(c);
            }
            foreach (Plant p in plantList)
            {
                p.reset();
            }
            foreach (Remains r in remainsList)
            {
                int[] xy = r.getLocationXY();
                getTile(xy[1], xy[0]).clearTile();
            }
            creatureList = new List<Creature>();
            deadList = new Stack<Creature>();
            remainsList = new List<Remains>();
            addCreatures(newCreatureList);
        }

        /// <summary>
        /// Sets up the world, performing all actions common to all the constructors
        /// </summary>
        public void setUpWorld()
        {
            timer = 0;
            Vector2 TL = new Vector2(0, 150); //The top left of that area to actually draw the world in
            inputHandler = new WorldInputHandler(TL, new Vector2(1024 - TL.X, 768 - TL.Y), this);
            deadList = new Stack<Creature>();
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
            for (int plantNum = 0; plantNum < Simulation.getPlantPopulation(); plantNum++)
            {
                int[] xy = getRandomClearTile();
                if (xy[0] == -1 && xy[1] == -1)
                {
                    throw new Exception("World is full, cannot add any further content");
                }
                else
                {
                    Tile t = getTile(xy[1], xy[0]);
                    Plant p = new Plant(randomNumberGenerator);
                    p.setLocation(xy[0], xy[1]);
                    plantList.Add(p);
                    t.addPlant(p);
                }
            }
            for (int obstNum = 0; obstNum < Simulation.getNumObstacles(); obstNum++)
            {
                int[] xy = getRandomClearTile();
                if (xy[0] == -1 && xy[1] == -1)
                {
                    throw new Exception("World is full, cannot add any further content");
                }
                else
                {
                    Tile t = getTile(xy[1], xy[0]);
                    Obstacle o = new Obstacle();
                    o.setLocation(xy[0], xy[1]);
                    t.addObstacle(o);
                }
            }
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
            creatureList = new List<Creature>();
            creatureList.AddRange(creatures.ToArray<Creature>());
            placeCreatures();
        }

        /// <summary>
        /// Places all the creatures in the creature list at random locations in the world.
        /// </summary>
        private void placeCreatures()
        {
            creatureList.Sort(sortCreaturesBySpeed);
            foreach (Creature c in creatureList)
            {
                c.setWorld(this);
                addCreature(c);
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

            while (!found && attempts < 25) //This is the number of times the program will attempt to place the creature before giving up
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
                    while (yLoc < tiles.Length - 1)
                    {
                        while (xLoc < tiles[yLoc].Length - 1)
                        {
                            
                            if (tileIsClear(yLoc, xLoc))
                            {
                                xLoc--;
                                yLoc--;
                                found = true;
                            }
                            xLoc++;
                        }
                        yLoc++;
                        xLoc = 0;
                    }
                    if (!found)
                    {
                        found = true;
                        xLoc = -1;
                        yLoc = -1;
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
            ArrayList a = new ArrayList(4);
            a.Add(new List<Creature>());
            a.Add(new List<Plant>());
            a.Add(new List<Remains>());
            a.Add(new List<Obstacle>());

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
                        if (!tileIsClear(lookY, lookX))
                        {
                            Tile t = getTile(lookY, lookX);
                            if (t.creaturePresent())
                            {
                                Creature c = t.getCreature();
                                List<Creature> lc = (List<Creature>)a[0];
                                lc.Add(c);
                            }
                            else if (t.plantPresent())
                            {
                                Plant p = t.getPlant();
                                if (p.canBeEaten())
                                {
                                    List<Plant> lp = (List<Plant>)a[1];
                                    lp.Add(p);
                                }
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
            return a;
        }

        /// <summary>
        /// Gets a specified tile given a row and column
        /// </summary>
        /// <param name="row">The row of the tile to look at</param>
        /// <param name="col">The column of the tile to look at</param>
        /// <returns>The tile found at the given row and column</returns>
        public Tile getTile(int row, int col)
        {
            Tile t = null;
            if (col < 0 || col > tiles.Length - 1)
            {
                t = new Tile();
                Obstacle o = new Obstacle();
                o.setLocation(col, row);
                t.addObstacle(o);
            }
            else if (row < 0 || row > tiles[col].Length - 1)
            {
                t = new Tile();
                Obstacle o = new Obstacle();
                o.setLocation(col, row);
                t.addObstacle(o);
            }
            else
            {
                t = tiles[col][row];
            }
            return t;
        }

        /// <summary>
        /// Adds a creature at the specified location
        /// </summary>
        /// <param name="x">The x index to add the creature at</param>
        /// <param name="y">The y index to add the creature at</param>
        /// <param name="c">The creature to add</param>
        public void addCreature(int x, int y, Creature c)
        {
            getTile(y, x).addCreature(c);
            c.setLocation(x, y);
        }

        /// <summary>
        /// Adds a creature at a random clear location
        /// </summary>
        /// <param name="c">The creature to add</param>
        public void addCreature(Creature c)
        {
            int[] clearTile = getRandomClearTile();
            addCreature(clearTile[0], clearTile[1], c);
            if (!creatureList.Contains(c))
            {
                creatureList.Add(c);
            }
        }

        /// <summary>
        /// Checks if a specified tile is clear
        /// </summary>
        /// <param name="row">The row index of the specified tile</param>
        /// <param name="col">The column index of the specified tile</param>
        /// <returns>True is the tile is empty, false otherwise</returns>
        public bool tileIsClear(int row, int col)
        {
            return !(getTile(row, col).isImpassible());
        }

        /// <summary>
        /// Ticks the world foward
        /// </summary>
        public void tick()
        {
            if (creatureUpdated < creatureList.Count)
            {
                for (int i = 0; i < numToUpdatePerTick; i++)
                {
                    if (!(creatureUpdated < creatureList.Count))
                    {
                        i = numToUpdatePerTick + 1;
                    }
                    else
                    {
                        creatureList[creatureUpdated].tick();
                        creatureUpdated++;
                    }
                }
            }
            else
            {
                foreach (Plant p in plantList)
                {
                    p.tick();
                }
                List<Remains> removeList = new List<Remains>();
                foreach (Remains r in remainsList)
                {
                    r.tick();
                    if (r.fullyDecayed())
                    {
                        removeList.Add(r);
                    }
                }
                foreach (Remains r in removeList)
                {
                    int[] xy = r.getLocationXY();
                    getTile(xy[1], xy[0]).clearTile();
                    remainsList.Remove(r);
                    r.setLocation(-1, -1);
                }
                creatureUpdated = 0;
                Simulation.tick();
            }
            if (creatureList.Count == 0 && Simulation.judgeIfAllDead())
            {
                creatureUpdated = 0;
                Simulation.judge();
            }
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
            remainsList.Add(r);
            getTile(loc[1], loc[0]).addRemains(r);
            deadList.Push(c);
#if DEBUG
            Console.WriteLine("Remains added at: " + loc[0] + "," + loc[1]);
#endif
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
        /// Checks if there is a plant at the specified location
        /// </summary>
        /// <param name="x">The x of the location to check</param>
        /// <param name="y">The y of the location to checl</param>
        /// <returns>Whether there is a plant at the specified location</returns>
        public bool plantAt(int x, int y)
        {
            return getTile(y, x).plantPresent();
        }

        /// <summary>
        /// Gets the plant at a specified location
        /// </summary>
        /// <param name="x">The x part of the location to look for the plant at</param>
        /// <param name="y">The y part of the location to look for the plant at</param>
        /// <returns>The plant at the specified location</returns>
        public Plant getPlantAt(int x, int y)
        {
            return getTile(y, x).getPlant();
        }

        /// <summary>
        /// Checks if there are remains at the specified location
        /// </summary>
        /// <param name="x">The x of the location to check</param>
        /// <param name="y">The y of the location to checl</param>
        /// <returns>Whether there are remains at the specified location</returns>
        public bool remainsAt(int x, int y)
        {
            return getTile(y, x).remainsPresent();
        }

        /// <summary>
        /// Gets the remains at a specified location
        /// </summary>
        /// <param name="x">The x part of the location to look for the remains at</param>
        /// <param name="y">The y part of the location to look for the remains at</param>
        /// <returns>The remains at the specified location</returns>
        public Remains getRemainsAt(int x, int y)
        {
            return getTile(y, x).getRemains();
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
        /// Gets the no. of tiles in the World as a Vector 2
        /// </summary>
        /// <returns>A vector2 representing the number of tiles in the world in X and Y</returns>
        public Vector2 getSize()
        {
            return new Vector2(worldX, worldY);
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

        /// <summary>
        /// The update method of the world, uses a momentum based update system that updates more and
        /// more creatures while the Simulation is not running slowly and halves the number of
        /// Creatures being updated each time the Simulation starts running slowly
        /// </summary>
        /// <param name="gameTime">The time since the update method was last called</param>
        public override void update(GameTime gameTime)
        {
            int speed = inputHandler.getSpeed();
            timer += gameTime.ElapsedGameTime.Ticks;
            if (speed > 0 && timer > actSpeed / speed)
            {
                if (gameTime.IsRunningSlowly)
                {
                    if (numToUpdatePerTick > 1)
                    {
                        numToUpdatePerTick /= 2;
                    }
                }
                else
                {
                    numToUpdatePerTick++;
                }
                tick();
                timer = 0;
            }
            inputHandler.update(gameTime);
        }

        /// <summary>
        /// The drawing method, calls the draw method on the inputHandler for the world
        /// </summary>
        public override void draw()
        {
            inputHandler.draw();
        }
    }
}
