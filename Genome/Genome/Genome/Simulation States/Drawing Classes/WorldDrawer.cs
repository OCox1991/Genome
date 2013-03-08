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
                    //drawCreatureView(inputHandler.getCreature());
                }
                else if (inputHandler.viewingAPlant())
                {
                   // drawPlantView(inputHandler.getPlant());
                }
                else if (inputHandler.viewingSomeRemains())
                {
                   // drawRemainsView(inputHandler.getRemains());
                }
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
            spriteBatch.Begin();
            if (t.plantPresent())
            {
                spriteBatch.Draw(plantTile, r, Color.White);
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
                spriteBatch.Draw(creatureTile, r, Color.White);
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
            spriteBatch.End();

            foreach (Button b in inputHandler.getButtons())
            {
                Display.drawButton(b);
            }
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
    }
}
