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
    class Simulation : Microsoft.Xna.Framework.Game
    {
        private static List<Shape> recognisedShapes = new List<Shape>();
        private static int energyDrainPerTick = 5;
        private static int staminaRejuvenationPercent = 5;
        private static int healthRejuvenationPercent = 5;

        private static int remainsFoodValue = 600;
        private static int plantFoodValue = 300;
        private static int remainsFoodValueVariation = 2; // (1/x)
        private static int plantFoodValueVariation = 2;
        private static int numTicksPlantRegrow = 5000;
        private static int numTicksRemainsDecay = 7000;
        private static int numFoodUnitsPlant = 30;
        private static int numFoodUnitsRemains = 20;

        private static int population = 1000;
        private static int plantPop = 250;
        private static int obstacleNumber = 500;

        private static int energyWeight = 2;
        private static int healthWeight = 3;

        private static int starvingEnergyLevel = 250;
        private static int woundedHealthPercent = 15;
        private static int topPercentage = 25; //top 25%
        private static int elimPercentage = 25; //bottom 25%

        private static int roundLengths = 10000;
        private static int currentRoundTime;

        private static int numGenerations = -1;
        private static int currentGeneration = 1;

        private static SimulationState prevState;
        private static SimulationState state;

        
        private static Color[] colourMap = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet }; //Remember this is from 0 to 6 so when drawing take 1 to get the desired colour.
        private static WorldState theWorld;
        private static Creature selectedCreature;

        private static Dictionary<string, Texture2D> textures;

        private static GraphicsDeviceManager graphics;
        private static SpriteBatch spriteBatch;
        private static SpriteFont spriteFont;

        public Simulation()
        {
            parseShapes("shapes.txt");
            state = new WorldState();
            currentRoundTime = 0;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 480;
        }

        #region Methods

        #region Game rules related

        public static Color[] getColours()
        {
            return colourMap;
        }

        public static GraphicsDeviceManager getGraphicsDeviceManager()
        {
            return graphics;
        }

        public static List<Shape> getShapes()
        {
            return recognisedShapes;
        }

        public static int getHealthRegenSpeed()
        {
            return staminaRejuvenationPercent;
        }

        public static void setHealthRegenSpeed(int healthSpeed)
        {
            healthRejuvenationPercent = healthSpeed;
        }

        public static int getStaminaRegenSpeed()
        {
            return healthRejuvenationPercent;
        }

        public static void setStaminaRegenSpeed(int stamSpeed)
        {
            staminaRejuvenationPercent = stamSpeed;
        }

        public static int getEnergyDrainPerTick()
        {
            return energyDrainPerTick;
        }

        public static void setEnergyDrainPerTick(int newDrainSpeed)
        {
            energyDrainPerTick = newDrainSpeed;
        }
        
        public static int getPopulation()
        {
            return population;
        }

        public static void setPopulation(int val)
        {
            population = val;
        }

        public static int getElimPercentage()
        {
            return elimPercentage;
        }

        public static void setElimPercentage(int val)
        {
            elimPercentage = val;
        }

        public static int getTopPercentage()
        {
            return topPercentage;
        }

        public static void setTopPercentage(int val)
        {
            topPercentage = val;
        }

        public static WorldState getCurrentWorld()
        {
            return theWorld;
        }

        public static int getEnergyWeight()
        {
            return energyWeight;
        }

        public static void setEnergyWeight(int val)
        {
            energyWeight = val;
        }

        public static int getHealthWeight()
        {
            return healthWeight;
        }

        public static void setHealthWeight(int val)
        {
            healthWeight = val;
        }

        public static int getNumObstacles()
        {
            return obstacleNumber;
        }

        public static void setNumObstacles(int val)
        {
            obstacleNumber = val;
        }

        public static int getNumTicksToDecayRemains()
        {
            return numTicksRemainsDecay;
        }

        public static void setNumTicksToDecayRemains(int val)
        {
            numTicksRemainsDecay = val;
        }

        public static int getNumTicksToRegrowPlant()
        {
            return numTicksPlantRegrow;
        }

        public static void setNumTicksToRegrowPlant(int val)
        {
            numTicksPlantRegrow = val;
        }

        public static int getPlantFoodMax()
        {
            return numFoodUnitsPlant;
        }

        public static void setPlantFoodMax(int val)
        {
            numFoodUnitsPlant = val;
        }

        public static int getPlantFoodValue()
        {
            return plantFoodValue;
        }

        public static void setPlantFoodValue(int val)
        {
            plantFoodValue = val;
        }

        public static int getPlantFoodValueVariation()
        {
            return plantFoodValueVariation;
        }

        public static void setPlantFoodValueVariation(int val)
        {
            plantFoodValueVariation = val;
        }

        public static int getPlantPopulation()
        {
            return plantPop;
        }

        public static void setPlantPopulation(int val)
        {
            plantPop = val;
        }

        public static int getRemainsFoodAmount()
        {
            return numFoodUnitsRemains;
        }

        public static void setRemainsFoodAmount(int val)
        {
            numFoodUnitsRemains = val;
        }

        public static int getRemainsFoodValue()
        {
            return remainsFoodValue;
        }

        public static void setRemainsFoodValue(int val)
        {
            remainsFoodValue = val;
        }

        public static int getRemainsFoodValueVariation()
        {
            return remainsFoodValueVariation;
        }

        public static void setRemainsFoodValueVariation(int val)
        {
            remainsFoodValueVariation = val;
        }

        public static int getStarvingEnergyLevel()
        {
            return starvingEnergyLevel;
        }

        public static void setStarvingEnergyLevel(int val)
        {
            starvingEnergyLevel = val;
        }

        public static int getWoundedHealthPercent()
        {
            return woundedHealthPercent;
        }

        public static void setWoundedHealthPercent(int val)
        {
            woundedHealthPercent = val;
        }

        #endregion

        #region game running logic

        public static void tick()
        {
            currentRoundTime++;
            if (currentRoundTime >= roundLengths)
            {
                WorldState w = theWorld;
                state = new JudgingState(w.getLiveCreatures(), w.getDeadCreatures());
            }
        }

        public static void judgingDone(List<Creature> creatureList)
        {
            theWorld.reset();
            theWorld.addCreatures(creatureList);
            state = theWorld;
            currentGeneration++;
        }

        public static void goToMenu()
        {
            prevState = state;
            state = new MainMenuState();
        }

        public static void exitMenu()
        {
            state = prevState;
            prevState = null;
        }

        #endregion

        #region overriding methods to make the game work

        protected override void Initialize()
        {
            //TODO: add code for initialising the world
            theWorld = new WorldState();
            state = theWorld;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont");
            //using a dictionary to set up the textures to allow us to store metadata about them in the form of strings (as keys)
            Dictionary<string, string> texList = new Dictionary<string, string>();
            texList.Add("Empty Tile", "BlankTile");
            texList.Add("Creature Tile", "Creature");
            texList.Add("Obstacle Tile", "Obstacle");
            texList.Add("Plant Tile", "Plant");
            texList.Add("Remains Tile", "Remains");

            string[] texNames = texList.Keys.ToArray<string>();

            foreach (string s in texNames)
            {
                textures.Add(s, Content.Load<Texture2D>(texList[s]));
            }

            base.LoadContent();
        }

        protected Texture2D getTexture(string textureDescription)
        {
            return textures[textureDescription];
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            state.update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //TODO: Add code for drawing the top menu, which will be present on all screens
            state.draw();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

        #region Parsers

        private void parseShapes(String fileLocation)
        {

        }

        #endregion

        #endregion

    }
}
