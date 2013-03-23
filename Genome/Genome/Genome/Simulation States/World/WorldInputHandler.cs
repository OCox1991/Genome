﻿using System;
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
        private bool start; //the start 
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

        public void update(GameTime gameTime)
        {
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
            if (viewingACreature())
            {
                if (creatureView.getHealth() <= 0)
                {
                    deView();
                }
            }
            prevState = currentState;
            currentState = Mouse.GetState();
            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Escape))
            {
                Simulation.goToMenu();
            }
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
            if (currentState.X > topLeft.X && currentState.Y > topLeft.Y && currentState.X < topLeft.X + size.X && currentState.Y < getBottomY())
            {
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

        public void draw()
        {
            drawer.draw();
        }

        private float getBottomY()
        {
            float bottom = topLeft.Y + size.Y;
            if (viewing)
            {
                bottom -= 150;
            }
            return bottom;
        }

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

        private void centreOnTile(int x, int y)
        {
            centreOn(new Vector2(x * Display.getTileSize(), y * Display.getTileSize()));
        }

        public List<Button> getButtons()
        {
            return buttons;
        }

        public Vector2 getTopLeft()
        {
            return topLeft;
        }

        public Vector2 getSize()
        {
            return size;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        private Vector2 getMouseLocation()
        {
            Vector2 loc = getLocation();
            return new Vector2(loc.X + currentState.X - topLeft.X, loc.Y + currentState.Y - topLeft.Y);
        }

        public bool viewingSomething()
        {
            return viewing;
        }

        public bool viewingACreature()
        {
            return creatureView != null;
        }

        public bool viewingAPlant()
        {
            return plantView != null;
        }

        public bool viewingSomeRemains()
        {
            return remainsView != null;
        }

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
        /// Doubles the speed, up to 32x, or sets it to one if it is at 0
        /// </summary>
        public void speedUp()
        {
            if (speed < 64 && speed > 0)
            {
                speed *= 2;
            }
            else if (speed <= 0)
            {
                speed = 1;
            }
            
        }

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

        public void viewCreature(Creature c)
        {
            viewing = true;
            maxInfo = creatureMaxInfo;
            buttons[4].setVisible(true);
            buttons[5].setVisible(true);
            creatureView = c;
        }

        private void viewPlant(Plant p)
        {
            viewing = true;
            maxInfo = plantMaxInfo;
            plantView = p;
        }
        private void viewRemains(Remains r)
        {
            viewing = true;
            maxInfo = remainsMaxInfo;
            remainsView = r;
        }

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

        public Creature getCreature()
        {
            return creatureView;
        }

        public Plant getPlant()
        {
            return plantView;
        }

        public Remains getRemains()
        {
            return remainsView;
        }

        public int getCreaturesAlive()
        {
            return world.getLiveCreatures().Count;
        }

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

        public string[] getPlantInfo(Plant p)
        {
            string[] status = new string[1];
            switch (DataViewed)
            {
                case 0: status = new string[] { "Food Units Remaining: " + p.getFoodRemaining(), "Food value: " + p.getFoodValue(), "Regrows in: " + p.getTicksRemainingBeforeAction() + " ticks" }; break;
            }
            return status;
        }

        public string[] getRemainsInfo(Remains r)
        {
            string[] status = new string[1];
            switch (DataViewed)
            {
                case 0: status = new string[] { "Food Units Remaining: " + r.getFoodRemaining(), "Food value: " + r.getFoodValue(), "Decays in: " + r.getTicksRemainingBeforeAction() + " ticks" }; break;
            }
            return status;
        }

        public void nextInfo()
        {
            dataViewed = (dataViewed + 1) % maxInfo;
        }

        public int getSeed()
        {
            return world.Seed;
        }
    }
}
