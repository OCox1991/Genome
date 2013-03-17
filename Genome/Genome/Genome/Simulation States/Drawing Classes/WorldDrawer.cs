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
    class WorldDrawer
    {
        private WorldInputHandler inputHandler;
        private SpriteBatch spriteBatch;
        private Texture2D creatureTile;
        private Texture2D plantTile;
        private Texture2D remainsTile;
        private Texture2D obstacleTile;
        private Texture2D emptyTile;
        private Texture2D topBar;
        private Texture2D viewBack;
        private Texture2D depletedPlantTile;
        private const string speed = "Speed: ";
        private const string tileLocText = "Tile ";
        private SpriteFont spriteFont;

        public WorldDrawer(WorldInputHandler inputHandler)
        {
            this.inputHandler = inputHandler;


            creatureTile = Display.getTexture(TextureNames.CREATURE);
            plantTile = Display.getTexture(TextureNames.PLANT);
            remainsTile = Display.getTexture(TextureNames.REMAINS);
            obstacleTile = Display.getTexture(TextureNames.OBSTACLE);
            emptyTile = Display.getTexture(TextureNames.EMPTY);
            topBar = Display.getTexture(TextureNames.TOP);
            viewBack = Display.getTexture(TextureNames.VIEWBACK);
            depletedPlantTile = Display.getTexture(TextureNames.PLANT_DEPLETED);
            spriteBatch = Display.getSpriteBatch();
            spriteFont = Display.getFont();
        }

        public void draw()
        {
            drawWorld();
            drawTop();
            if(inputHandler.viewingSomething())
            {
                if (inputHandler.viewingACreature())
                {
                    drawCreatureView(inputHandler.getCreature());
                }
                else if (inputHandler.viewingAPlant())
                {
                   drawPlantView(inputHandler.getPlant());
                }
                else if (inputHandler.viewingSomeRemains())
                {
                   drawRemainsView(inputHandler.getRemains());
                }
            }
            foreach (Button b in inputHandler.getButtons())
            {
                Display.drawButton(b);
            }
        }

        private void drawWorld()
        {
            Vector2 loc = inputHandler.getLocation();
            Tile[][] drawTiles = inputHandler.getTilesVisible();
            Vector2 drawLoc = new Vector2(0 , 0);
            float tileSize = Display.getTileSize();
            if (Math.Abs(loc.X) % tileSize != 0 || Math.Abs(loc.Y) % tileSize != 0) //if loc is not exactly on a tile
            {
                drawLoc = new Vector2(-(Math.Abs(loc.X) % tileSize), -(Math.Abs(loc.Y) % tileSize));
            }

            for (int x = 0; x < drawTiles.Length; x++)
            {
                for (int y = 0; y < drawTiles[x].Length; y++)
                {
                    int ts = (int)Display.getTileSize();
                    Rectangle r = new Rectangle((int)((x * ts) + inputHandler.getTopLeft().X + drawLoc.X), (int)((y * ts) + inputHandler.getTopLeft().Y + drawLoc.Y), ts, ts);
                    drawTile(r, drawTiles[x][y]);
                }
            }
        }

        private void drawTile(Rectangle r, Tile t)
        {
            Simulation.getGraphicsDeviceManager().GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            spriteBatch.Begin(0, null, SamplerState.PointWrap, null, null);
            if (t.plantPresent())
            {
                if (t.getPlant().isDepeleted())
                {
                    spriteBatch.Draw(depletedPlantTile, r, Color.White);
                }
                else
                {
                    spriteBatch.Draw(plantTile, r, Color.White);
                }
            }
            else if (t.remainsPresent())
            {
                spriteBatch.Draw(remainsTile, r, Color.White);
            }
            else if (t.obstaclePresent())
            {
                spriteBatch.Draw(obstacleTile, r, Color.White);
            }
            else if (t.creaturePresent())
            {
                if (Display.getDrawCreaturesAsGenes())
                {
                    Gene g = t.getCreature().getDna();
                    spriteBatch.Draw(g.getTexture(), r, Color.White);
                }
                else
                {
                    spriteBatch.Draw(creatureTile, r, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(emptyTile, r, Color.White);
            }
            spriteBatch.End();
        }

        private void drawTop()
        {
            int[] loc = inputHandler.getTileLoc();
            string locationData = String.Concat(new String[] { tileLocText, " ( ", loc[0].ToString(), ",", loc[1].ToString(), " ) " });

            spriteBatch.Begin();
            spriteBatch.Draw(topBar, new Rectangle(0, 0, 1024, 150), Color.White);
            spriteBatch.DrawString(spriteFont, speed, new Vector2(20, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, inputHandler.getSpeed().ToString(), new Vector2(20 + spriteFont.MeasureString(speed).X + 35 + 30, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, locationData, new Vector2(20, (60 + spriteFont.MeasureString(speed).Y + 20)), Color.Black);
            spriteBatch.DrawString(spriteFont, roundInfo(), new Vector2(1019 - spriteFont.MeasureString(roundInfo()).X, 10), Color.Black);
            spriteBatch.DrawString(spriteFont, generationInfo(), new Vector2(1019 - spriteFont.MeasureString(generationInfo()).X, 15 + spriteFont.MeasureString(roundInfo()).Y), Color.Black);
            spriteBatch.DrawString(spriteFont, creatureInfo(), new Vector2(1019 - spriteFont.MeasureString(creatureInfo()).X, 20 + spriteFont.MeasureString(roundInfo()).Y + spriteFont.MeasureString(generationInfo()).Y), Color.Black);
            string seed = "Seed: " + inputHandler.getSeed();
            spriteBatch.DrawString(spriteFont, seed, new Vector2(1019 - spriteFont.MeasureString(seed).X, 25 + spriteFont.MeasureString(roundInfo()).Y + spriteFont.MeasureString(generationInfo()).Y + spriteFont.MeasureString(creatureInfo()).Y), Color.Black);
            spriteBatch.End();
        }

        private string roundInfo()
        {
            string r = "Current Round: " + Simulation.getNumTicks() + "/" + Simulation.getRoundLength();
            return r;
        }

        private string generationInfo()
        {
            string r = "Generation: " + Simulation.getGeneration();
            if (Simulation.getTargetGeneration() != -1)
            {
                r += " (Stopping at: " + Simulation.getTargetGeneration() + ")";
            }
            return r;
        }

        private string creatureInfo()
        {
            string r = "Creatures alive: " + inputHandler.getCreaturesAlive() + "/" + Simulation.getPopulation();
            return r; 
        }

        private void drawCreatureView(Creature c)
        {
            drawBack();
            Simulation.getGraphicsDeviceManager().GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            spriteBatch.Begin(0, null, SamplerState.PointWrap, null, null);
            Texture2D tex = c.getDna().getTexture();
            Rectangle r = new Rectangle((Display.getWindowWidth() - 800) / 2 + 10, Display.getWindowHeight() - 130, 120, 120);
            spriteBatch.Draw(tex, r, Color.White);
            String title = "Creature";
            if (c.isStealthy())
            {
                title += " (hiding)";
            }
            string[] status = inputHandler.getCreatureInfo(c);
            Vector2 topLeft = new Vector2((Display.getWindowWidth() - 800) / 2 + 10 + 120 + 10, Display.getWindowHeight() - 130);
            spriteBatch.End();
            drawStrings(topLeft, title, status, 4);
        }

        private void drawPlantView(Plant p)
        {
            drawBack();
            Simulation.getGraphicsDeviceManager().GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            spriteBatch.Begin(0, null, SamplerState.PointWrap, null, null);
            Rectangle r = new Rectangle((Display.getWindowWidth() - 800) / 2 + 10, Display.getWindowHeight() - 130, 120, 120);
            Texture2D tex = null;
            if(p.isDepeleted())
            {
                tex = depletedPlantTile;
            }
            else
            {
                tex = plantTile;
            }
            spriteBatch.Draw(tex, r, Color.White);
            spriteBatch.End();
            string[] status = inputHandler.getPlantInfo(p);
            Vector2 topLeft = new Vector2((Display.getWindowWidth() - 800) / 2 + 10 + 120 + 10, Display.getWindowHeight() - 130);
            string title = "Plant";
            if(p.isDepeleted())
            {
                title += " (depleted)";
            }
            drawStrings(topLeft, title, status, 4);
        }

        private void drawRemainsView(Remains r)
        {
            drawBack();
            Simulation.getGraphicsDeviceManager().GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            spriteBatch.Begin(0, null, SamplerState.PointWrap, null, null);
            Rectangle rect = new Rectangle((Display.getWindowWidth() - 800) / 2 + 10, Display.getWindowHeight() - 130, 120, 120);
            Texture2D tex = remainsTile;
            spriteBatch.Draw(tex, rect, Color.White);
            spriteBatch.End();
            string[] status = inputHandler.getRemainsInfo(r);
            Vector2 topLeft = new Vector2((Display.getWindowWidth() - 800) / 2 + 10 + 120 + 10, Display.getWindowHeight() - 130);
            string title = "Remains";
            drawStrings(topLeft, title, status, 4);
        }

        private void drawBack()
        {
            spriteBatch.Begin();
            int sizeX = 800;
            int sizeY = 140;
            spriteBatch.Draw(viewBack, new Rectangle((Display.getWindowWidth() - sizeX) / 2, Display.getWindowHeight() - sizeY, sizeX, sizeY), Color.White);
            spriteBatch.End();
        }

        private void drawStrings(Vector2 topLeft, string title, string[] strings, int maxRows)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, title, new Vector2(topLeft.X, topLeft.Y), Color.Black);
            int cRows = 0;
            float currentX = topLeft.X;
            float currentY = topLeft.Y + spriteFont.MeasureString(title).Y + 10;
            foreach (string s in strings)
            {
                if (cRows >= maxRows)
                {
                    cRows = 0;
                    float cMax = 0;
                    int i = 0;
                    while (s != strings[i] && i < strings.Length)
                    {
                        if(cMax < spriteFont.MeasureString(strings[i]).X)
                        {
                            cMax = spriteFont.MeasureString(strings[i]).X;
                        }
                        i++;
                    }
                    currentX += cMax + 10;
                    currentY = topLeft.Y + spriteFont.MeasureString(title).Y + 10;
                }
                spriteBatch.DrawString(spriteFont, s, new Vector2(currentX, currentY), Color.Black);
                currentY += spriteFont.MeasureString(s).Y + 5;
                cRows++;
            }
            spriteBatch.End();
        }
    }
}
