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
    class WorldInputHandler
    {
        private int speed; //the current speed of the simulation
        private Vector2 topLeft; //Location of the top left of the actual area to draw the world
        private Vector2 size; //The size of the area to draw the world in
        private Vector2 location; //The current location we are looking at in the world
        private bool dragging; //if the world is being dragged or not at the moment
        private MouseState prevState; //the previous mouse state
        private MouseState currentState; //the current mouse state
        private List<Button> buttons; //the buttons present on the world view
        private WorldState world; //the world we are viewing
        private bool viewing; //if we are viewing a specific creature/plant/remains
        private Creature creatureView; //the creature being viewed
        private Remains remainsView; //the remains being viewed
        private Plant plantView; //the plant being viewed

        public WorldInputHandler(Vector2 topLeft, Vector2 size, WorldState world)
        {
            speed = 1;
            this.topLeft = topLeft;
            this.size = size;
            location = new Vector2(0, 0);
            dragging = false;
            this.world = world;
            buttons.Add(new MenuButton(new Vector2(0, 0)));
            //buttons.Add(new SpeedUpButton(new Vector2(150, 55), this));
            //buttons.Add(new SlowDownButton(new Vector2(100, 55), this));
        }

        public void update(GameTime gameTime)
        {
            prevState = currentState;
            currentState = Mouse.GetState();
            if (currentState.X > topLeft.X && currentState.Y > topLeft.Y && currentState.X < topLeft.X + size.X && currentState.Y < getBottomY())
            {
                if (currentState.LeftButton.Equals(ButtonState.Pressed) && prevState.LeftButton.Equals(ButtonState.Pressed))
                {
                    Vector2 cStateLoc = new Vector2(currentState.X, currentState.Y);
                    Vector2 prevStateLoc = new Vector2(prevState.X, prevState.Y);
                    if (!cStateLoc.Equals(prevStateLoc))
                    {
                        dragging = true;
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
                        clicked(currentState.X, currentState.Y);
                    }
                }
                else
                {
                    foreach (Button b in buttons)
                    {
                        b.update(gameTime, currentState);
                    }
                }
            }
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

        public void updateLocation(float difX, float difY)
        {
            if (location.X + difX < 0)
            {
                location.X = 0;
            }
            else if (location.X + difX > world.getSize().X * Display.getTileSize() - this.size.X)
            {
                location.X = world.getSize().X * Display.getTileSize() - this.size.X;
            }
            else
            {
                location.X += difX;
            }
            if (location.Y + difY < 0)
            {
                location.Y = 0;
            }
            else if (location.Y + difY > world.getSize().Y * Display.getTileSize() - this.size.Y)
            {
                location.Y = world.getSize().Y * Display.getTileSize() - this.size.Y;
            }
            else
            {
                location.Y += difY;
            }
        }

        public void clicked(float x, float y)
        {
            float offsetX = location.X % 1;
            float offsetY = location.Y % 1;

            float mainOffsetX = location.X - offsetX;
            float mainOffsetY = location.Y - offsetY;
            
            //make it just a number not a number of pixels
            x /= Display.getTileSize();
            y /= Display.getTileSize();

            //Then make it point to the correct tile
            x -= offsetX;
            y -= offsetY;

            x += mainOffsetX;
            y += mainOffsetY;

            

            int ix = (int)x;
            int iy = (int)y;
            //next get the tile
            deView();
            if (world.creatureAt(ix, iy))
            {
                viewingCreature(world.getCreatureAt(ix, iy));
            }
            else if(world.plantAt(ix, iy))
            {
                viewingPlant(world.getPlantAt(ix, iy));
            }
            else if (world.remainsAt(ix, iy))
            {
                viewingRemains(world.getRemainsAt(ix, iy));
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
            if (speed < 32 && speed > 0)
            {
                speed *= 2;
            }
            else if (speed == 0)
            {
                speed = 1;
            }
        }

        public Tile[][] getTilesVisible()
        {
            float x = size.X / Display.getTileSize();
            float y = size.Y / Display.getTileSize();
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

            for (int X = 0; x < tiles.Length; X++)
            {
                for (int Y = 0; Y < tiles[X].Length; Y++)
                {
                    tiles[X][Y] = world.getTile((int)locY + Y, (int)locX + X);
                }
            }
            return tiles;
        }

        /// <summary>
        /// Halves the speed or sets it to 0 if it is at 1 or below
        /// </summary>
        public void slowDown()
        {
            if (speed > 1)
            {
                speed /= 2;
            }
            else if (speed <= 1)
            {
                speed = 0;
            }
        }

        private void viewingCreature(Creature c)
        {
            viewing = true;
            creatureView = c;
        }

        private void viewingPlant(Plant p)
        {
            viewing = true;
            plantView = p;
        }
        private void viewingRemains(Remains r)
        {
            viewing = true;
            remainsView = r;
        }

        private void deView()
        {
            viewing = false;
            creatureView = null;
            plantView = null;
            remainsView = null;
        }
    }
}
