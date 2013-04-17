using System;
using System.Collections.Generic;
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
    /// <summary>
    /// The WorldInputHandler deals with input from the user that might affect the world, as well as serving as an intermediary between the WorldState and the WorldDrawer, cropping the tiles of
    /// the WorldState so that the WorldDrawer only sees what it needs to draw
    /// </summary>
    class WorldInputHandler
    {
        private static int speed; //the current speed of the simulation
        public static int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        private Vector2 topLeft; //Location of the top left of the actual area to draw the world
        private Vector2 size; //The size of the area to draw the world in
        private Vector2 location; //The current location we are looking at in the world
        private bool dragging; //if the world is being dragged or not at the moment
        private MouseState prevState; //the previous mouse state
        private MouseState currentState; //the current mouse state
        private List<Button> buttons; //the buttons present on the world view
        private WorldState world; //the world we are viewing
        private bool viewing; //if we are viewing a specific creature/plant/remains
        private bool following; //if we are following something
        private Creature creatureView; //the creature being viewed
        private Remains remainsView; //the remains being viewed
        private Plant plantView; //the plant being viewed
        private WorldDrawer drawer; //the drawing class for the world
        private bool start; //A flag to check if we have just started this state
        private int dataViewed;
        public int DataViewed
        {
            get { return dataViewed; }
        }
        private int maxInfo;
        private static int creatureMaxInfo = 4;
        private static int plantMaxInfo = 1;
        private static int remainsMaxInfo = 1;

        public WorldInputHandler(Vector2 topLeft, Vector2 size, WorldState world)
        {
            speed = 0;
            this.topLeft = topLeft;
            this.size = size;
            location = new Vector2(0, 0);
            dragging = false;
            following = false;
            this.world = world;
            buttons = new List<Button>();
            buttons.Add(new MenuButton(new Vector2(1, 1)));
            buttons.Add(new SlowDownButton(new Vector2(Display.measureString("Speed:        ").X, 55), this));
            buttons.Add(new SpeedUpButton(new Vector2(Display.measureString("Speed:        x00 ").X + 35 + 5, 55), this));
            buttons.Add(new ViewMoreInfoButton(new Vector2(Display.getWindowWidth() - (110 + ((Display.getWindowWidth() - 800) / 2)), Display.getWindowHeight() - 130), this));
            buttons.Add(new KillButton(this, new Vector2(Display.getWindowWidth() - (110 + ((Display.getWindowWidth() - 800) / 2)) - buttons[3].getWidth() - 5, Display.getWindowHeight() - 130)));
            buttons.Add(new CloneButton(this, new Vector2(Display.getWindowWidth() - (110 + ((Display.getWindowWidth() - 800) / 2)) - buttons[3].getWidth() - 5 - buttons[4].getWidth() - 5, Display.getWindowHeight() - 130)));
            drawer = new WorldDrawer(this);
            buttons[3].setVisible(false);
            buttons[4].setVisible(false);
            buttons[5].setVisible(false);
            start = false;
        }

        /// <summary>
        /// Handles update code for the inputhandler, mostly it involves dealing with the mouse and keyboard, but it also needs to make sure it doesn't keep viewing creatures that are dead or creatures from the previous
        /// generation
        /// </summary>
        /// <param name="gameTime">The time since the update method was last called, not used for this method</param>
        public void update(GameTime gameTime)
        {
            //Checks if we are at the start of a round, if so we need to make sure we deView any creature we may have been looking at from last round, but we should 
            //only do this once so we can view new creatures while paused
            if(Simulation.getNumTicks() == 0)
            {
                if (start) //check if we have already done this
                {
                    start = false;
                    deView();
                }
            }
            else if (Simulation.getNumTicks() == 1)
            {
                start = true; //reset the start flag here
            }
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].update(gameTime);
            }
            //Check if the creature we are following is dead
            if (viewingACreature())
            {
                if (creatureView.getHealth() <= 0)
                {
                    deView();
                }
            }
            //Update the keyboard states, to check if Esc was pressed (if so go to main menu)
            prevState = currentState;
            currentState = Mouse.GetState();
            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Escape))
            {
                Simulation.goToMenu();
            }
            //If we are following an object then we should centre on it
            if (following)
            {
                WorldObject o = null;
                if (viewingACreature())
                {
                    o = creatureView;
                }
                else if (viewingAPlant())
                {
                    o = plantView;
                }
                else if (viewingSomeRemains())
                {
                    o = remainsView;
                }
                int[] l = o.getLocationXY();
                centreOnTile(l[0], l[1]);
            }
            //Check the location of the mouse to see if it is within the boundaries of the world tiles
            if (currentState.X > topLeft.X && currentState.Y > topLeft.Y && currentState.X < topLeft.X + size.X && currentState.Y < getBottomY())
            {
                //If we are dragging the mouse
                if (currentState.LeftButton.Equals(ButtonState.Pressed) && prevState.LeftButton.Equals(ButtonState.Pressed))
                {
                    Vector2 cStateLoc = new Vector2(currentState.X, currentState.Y);
                    Vector2 prevStateLoc = new Vector2(prevState.X, prevState.Y);
                    if (Math.Abs(cStateLoc.X - prevStateLoc.X) > 0 || Math.Abs(cStateLoc.X - prevStateLoc.X) > 0)
                    {
                        dragging = true;
                        following = false;
                        updateLocation(prevStateLoc.X - cStateLoc.X, prevStateLoc.Y - cStateLoc.Y);
                    }
                }
                //If we just clicked the mouse or released it when we had been dragging
                else if (currentState.LeftButton.Equals(ButtonState.Released) && prevState.LeftButton.Equals(ButtonState.Pressed))
                {
                    if (dragging)
                    {
                        dragging = false;
                    }
                    else
                    {
                        int x = getTileLoc()[0];
                        int y = getTileLoc()[1];
                        clicked(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the world as interpreted by the input handler
        /// </summary>
        public void draw()
        {
            drawer.draw();
        }

        /// <summary>
        /// Gets the bottom of the tile area, with methods to avoid dragging if the viewing window is up
        /// </summary>
        /// <returns>The location of the bottom of the tile area as a float</returns>
        private float getBottomY()
        {
            float bottom = topLeft.Y + size.Y;
            if (viewing)
            {
                bottom -= 150;
            }
            return bottom;
        }

        /// <summary>
        /// Centres the viewing are on a given Vector2 location
        /// </summary>
        /// <param name="location">The location to centre the viewing area on as a Vector2</param>
        private void centreOn(Vector2 location)
        {
            Vector2 centre = new Vector2(12 * Display.getTileSize(), 6 * Display.getTileSize());
            float lx = location.X - centre.X;
            float ly = location.Y - centre.Y;
            if (lx < 0)
            {
                lx = 0;
            }
            else if (lx > world.getSize().X * Display.getTileSize() - size.X)
            {
                lx = world.getSize().X * Display.getTileSize() - size.X;
            }
            if (ly < 0)
            {
                ly = 0;
            }
            else if (ly > world.getSize().Y * Display.getTileSize() - size.Y)
            {
                ly = world.getSize().Y * Display.getTileSize() - size.Y;
            }
            this.location = new Vector2(lx, ly);
        }

        /// <summary>
        /// Centres the viewing area on a specified tile using the centreOnMethod
        /// </summary>
        /// <param name="x">The x location of the tile to centre on</param>
        /// <param name="y">The y location of the tile to centre on</param>
        private void centreOnTile(int x, int y)
        {
            centreOn(new Vector2(x * Display.getTileSize(), y * Display.getTileSize()));
        }

        /// <summary>
        /// Gets all the buttons associated with this InputHandler
        /// </summary>
        /// <returns>A typed list of all the buttons associated with this object, used by the drawer</returns>
        public List<Button> getButtons()
        {
            return buttons;
        }

        /// <summary>
        /// Gets the top left location of the world viewing area, that is, the area that the tiles start appearing, not the top menu
        /// </summary>
        /// <returns></returns>
        public Vector2 getTopLeft()
        {
            return topLeft;
        }

        /// <summary>
        /// Gets the size of the viewing area, that is, the area in which tiles appear
        /// </summary>
        /// <returns></returns>
        public Vector2 getSize()
        {
            return size;
        }

        /// <summary>
        /// Gets the location we are viewing, that is the location the top left of the viewing area is looking at in the tiles, for example if it was 40, 40 we would be viewing
        /// 40 pixels by 40 pixels away from the 0, 0 point of the world.
        /// </summary>
        /// <returns></returns>
        public Vector2 getLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets the location of the mouse in relation to the location the viewing area is looking at and the topleft as a Vector2
        /// </summary>
        /// <returns>The location of the mouse as a Vector2</returns>
        private Vector2 getMouseLocation()
        {
            Vector2 loc = getLocation();
            return new Vector2(loc.X + currentState.X - topLeft.X, loc.Y + currentState.Y - topLeft.Y);
        }

        /// <summary>
        /// Returns if the input handler is viewing anything
        /// </summary>
        /// <returns>True if the input handler is viewing something, false otherwise</returns>
        public bool viewingSomething()
        {
            return viewing;
        }

        /// <summary>
        /// Returns if the input handler is viewing a creature
        /// </summary>
        /// <returns>True if the input handler is viewing a creature, false otherwise</returns>
        public bool viewingACreature()
        {
            return creatureView != null;
        }

        /// <summary>
        /// Returns if the input handler is viewing a plant
        /// </summary>
        /// <returns>True if the input handler is viewing a plant, false otherwise</returns>
        public bool viewingAPlant()
        {
            return plantView != null;
        }

        /// <summary>
        /// Returns if the input handler is viewing remains
        /// </summary>
        /// <returns>True if the input handler is viewing remains, false otherwise</returns>
        public bool viewingSomeRemains()
        {
            return remainsView != null;
        }

        /// <summary>
        /// Gets the tile the mouse if hovering over as an array of ints
        /// </summary>
        /// <returns>The tile the mouse if hovering over with the 0th element as the x location and the 1st element as the y location</returns>
        public int[] getTileLoc()
        {
            Vector2 loc = getMouseLocation();
            float x = loc.X;
            float y = loc.Y;
            x /= Display.getTileSize();
            y /= Display.getTileSize();
            x -= x % 1;
            y -= y % 1;
            return new int[] { (int)x, (int)y };
        }

        /// <summary>
        /// Updates the location in the world the top left of the viewing area is over, making sure the viewing are doesn't go over the edge of the world
        /// </summary>
        /// <param name="difX">The difference in the X part of the location</param>
        /// <param name="difY">The difference in the Y part of the location</param>
        public void updateLocation(float difX, float difY)
        {
            difX *= 5;
            difY *= 5;
            if(location.X + difX < 0)
            {
                location.X = 0;
            }
            else if (location.X + difX > world.getSize().X * Display.getTileSize() - size.X)
            {
                location.X = world.getSize().X * Display.getTileSize() - size.X;
            }
            else
            {
                location.X += difX;
            }
            if (location.Y + difY < 0)
            {
                location.Y = 0;
            }
            else if (location.Y + difY > world.getSize().Y * Display.getTileSize() - size.Y)
            {
                location.Y = world.getSize().Y * Display.getTileSize() - size.Y;
            }
            else
            {
                location.Y += difY;
            }
        }

        /// <summary>
        /// Deals with what happens if parts of the viewing area are clicked
        /// </summary>
        /// <param name="x">The x location of the tile that was clicked</param>
        /// <param name="y">The y location of the tile that was clicked</param>
        public void clicked(int x, int y)
        {
#if DEBUG
            Console.WriteLine("clicked! (" + x + "," + y + ")");
            
#endif
            deView();
            if (!world.tileIsClear(y, x))
            {
                following = true;
                buttons[3].setVisible(true);
                if (world.creatureAt(x, y))
                {
#if DEBUG
                    Console.WriteLine("CREATURE");
#endif
                    viewCreature(world.getCreatureAt(x, y));
                }
                else if (world.plantAt(x, y))
                {
#if DEBUG
                    Console.WriteLine("PLANT");
#endif
                    viewPlant(world.getPlantAt(x, y));
                }
                else if (world.remainsAt(x, y))
                {
#if DEBUG
                    Console.WriteLine("REMAINS");
#endif
                    viewRemains(world.getRemainsAt(x, y));
                }
                else
                {
                    buttons[3].setVisible(false);
                    following = false;
                }
            }
        }

        /// <summary>
        /// Gets the current speed value
        /// </summary>
        /// <returns>The current speed</returns>
        public int getSpeed()
        {
            return speed;
        }

        /// <summary>
        /// Doubles the speed, up to 64x, or sets it to one if it is at 0
        /// </summary>
        public void speedUp()
        {
            if (speed < 128 && speed > 0)
            {
                speed *= 2;
            }
            else if (speed <= 0)
            {
                speed = 1;
            }
            
        }

        /// <summary>
        /// Gets the tiles visible in the World for drawing, cropping out the tiles that are not visible
        /// </summary>
        /// <returns>A 2d array of tiles that are visible based on the location and size of the viewing area</returns>
        public Tile[][] getTilesVisible()
        {
            float x = size.X / Display.getTileSize(); //number of tiles to display in each direction
            float y = size.Y / Display.getTileSize();
            if (x % 1 != 0)
            {
                x = x + (1 - Math.Abs(x % 1));
            }
            if (y % 1 != 0)
            {
                y = y + (1 - Math.Abs(y % 1));
            }

            Tile[][] tiles = new Tile[(int)x + 1][];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile[(int)y + 1];
            }

            float locX = location.X;
            float locY = location.Y;
            locX /= Display.getTileSize();
            locY /= Display.getTileSize();
            locX -= locX % 1;
            locY -= locY % 1;

            for (int x2 = 0; x2 < tiles.Length; x2++)
            {
                for (int y2 = 0; y2 < tiles[x2].Length; y2++)
                {
                    Tile t = world.getTile((int)locY + y2, (int)locX + x2);
                    if(t != null)
                    {
                        tiles[x2][y2] = t;
                    }
                }
            }
            return tiles;
        }

        /// <summary>
        /// Halves the speed or sets it to 0 if it is at 1 or below
        /// </summary>
        public void slowDown()
        {
            if (speed > 0)
            {
                speed /= 2;
            }
        }

        /// <summary>
        /// Stops the simulation, setting the speed to 0
        /// </summary>
        public void stop()
        {
            speed = 0;
        }

        /// <summary>
        /// Views a creature, sets viewing to be true and sets up the information viewing commands
        /// </summary>
        /// <param name="c">The Creature to be viewed</param>
        public void viewCreature(Creature c)
        {
            viewing = true;
            maxInfo = creatureMaxInfo;
            buttons[4].setVisible(true);
            buttons[5].setVisible(true);
            creatureView = c;
        }

        /// <summary>
        /// Views a plant, sets viewing to be true and sets up the information viewing commands
        /// </summary>
        /// <param name="c">The Plant to be viewed</param>
        private void viewPlant(Plant p)
        {
            viewing = true;
            maxInfo = plantMaxInfo;
            plantView = p;
        }

        /// <summary>
        /// Views remains, sets viewing to be true and sets up the information viewing commands
        /// </summary>
        /// <param name="c">The Remains to be viewed</param>
        private void viewRemains(Remains r)
        {
            viewing = true;
            maxInfo = remainsMaxInfo;
            remainsView = r;
        }

        /// <summary>
        /// Cancels views, sets viewing to be false and all the viewing variables etc. to be null
        /// </summary>
        private void deView()
        {
            viewing = false;
            creatureView = null;
            plantView = null;
            remainsView = null;
            following = false;
            dataViewed = 0;
            buttons[3].setVisible(false);
            buttons[4].setVisible(false);
            buttons[5].setVisible(false);
        }

        /// <summary>
        /// Gets the creature being viewed
        /// </summary>
        /// <returns>The creature that is being viewed</returns>
        public Creature getCreature()
        {
            return creatureView;
        }

        /// <summary>
        /// Gets the plant being viewed
        /// </summary>
        /// <returns>The plant that is being viewed</returns>
        public Plant getPlant()
        {
            return plantView;
        }

        /// <summary>
        /// Gets the remains being viewed
        /// </summary>
        /// <returns>The remains that are being viewed</returns>
        public Remains getRemains()
        {
            return remainsView;
        }

        /// <summary>
        /// Gets the number of creatures still alive
        /// </summary>
        /// <returns>The number of creatures still alive based on the list of live creatures in the World</returns>
        public int getCreaturesAlive()
        {
            return world.getLiveCreatures().Count;
        }

        /// <summary>
        /// Gets an array of strings that contain information about the creature
        /// </summary>
        /// <param name="c">The creature to get information about</param>
        /// <returns>An array of strings containing a set of information about the creature</returns>
        public string[] getCreatureInfo(Creature c)
        {
            string[] status = new string[1];
            switch (DataViewed)
            {
                case 0: status = new string[] { "Health: " + c.getHealth(), "Energy :" + c.getEnergy(), "Current Scenario: " + c.CurrentScenario, "Response: " + c.CurrentResponse, "Stamina: " + c.getStamina() + "/" + c.getMaxStamina(), "Location: (" + c.getLocationXY()[0] + "," + c.getLocationXY()[1] + ")" }; break;
                case 1: int[] colours = c.getDna().getColourCount();
                    status = new string[] { "Number of...", "0s: " + colours[0], "1s: " + colours[1], "2s: " + colours[2], "3s: " + colours[3], "4s: " + colours[4], "5s: " + colours[5], "6s: " + colours[6] };
                    break;
                case 2: Color[] cols = Simulation.getColours();
                    status = new string[cols.Length];
                    for (int i = 0; i < cols.Length; i++ )
                    {
                        status[i] = i + ": " + cols[i].ToString();
                    }
                    break;
                case 3: string dietString = "Diet: " + c.getDiet();
                    if (c.getDiet() < 0.5)
                    {
                        dietString += " (HERBIVORE)";
                    }
                    else
                    {
                        dietString += " (CARNIVORE)";
                    }
                    status = new string[] { "Stats:", "STR: " + c.getStrength(), "SPD: " + c.getSpeed(), "STL: " + c.getStealthVal(), "AWA: " + c.getAwareness(), "DEF: " + c.getDefence(), dietString };
                    break;
            }
            return status;
        }

        /// <summary>
        /// Gets an array of strings that contain information about the plant
        /// </summary>
        /// <param name="c">The plant to get information about</param>
        /// <returns>An array of strings containing a set of information about the plant</returns>
        public string[] getPlantInfo(Plant p)
        {
            string[] status = new string[1];
            switch (DataViewed)
            {
                case 0: status = new string[] { "Food Units Remaining: " + p.getFoodRemaining(), "Food value: " + p.getFoodValue(), "Regrows in: " + p.getTicksRemainingBeforeAction() + " ticks" }; break;
            }
            return status;
        }

        /// <summary>
        /// Gets an array of strings that contain information about some remains
        /// </summary>
        /// <param name="c">The remains to get information about</param>
        /// <returns>An array of strings containing a set of information about some remains</returns>
        public string[] getRemainsInfo(Remains r)
        {
            string[] status = new string[1];
            switch (DataViewed)
            {
                case 0: status = new string[] { "Food Units Remaining: " + r.getFoodRemaining(), "Food value: " + r.getFoodValue(), "Decays in: " + r.getTicksRemainingBeforeAction() + " ticks" }; break;
            }
            return status;
        }

        /// <summary>
        /// Increments the number of the information to get with the get*Info methods or sets it to 0 if it is at a certain value
        /// </summary>
        public void nextInfo()
        {
            dataViewed = (dataViewed + 1) % maxInfo;
        }

        /// <summary>
        /// Gets the seed of the world
        /// </summary>
        /// <returns>The seed of the world, an int</returns>
        public int getSeed()
        {
            return world.Seed;
        }
    }
}
