//#define CELLTEST
//#define PATTERNTEST
//#define POLLTEST
//#define DIETTEST
//#define BREEDTEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    /// The Simulation class stores all the game rules related variables, and is the main class, responsible for loading content, calling update and
    /// keeping track of what state the program is currently in. It provides methods to move between states
    /// </summary>
    class Simulation : Game
    {
        private static Simulation currentSimulation;

        private static List<Shape> recognisedShapes = new List<Shape>();

        private static int remainsFoodValue = 2500; //flat
        private static int plantFoodValue = 100; //flat
        private static int remainsFoodValueVariation = 2; //  +- (1/x)
        private static int plantFoodValueVariation = 2; // +- 1/x
        private static int numTicksPlantRegrow = 300; //flat
        private static int numTicksRemainsDecay = 300; //flat
        private static int numFoodUnitsPlant = 75; //flat
        private static int numFoodUnitsRemains = 3; //flat

        private static int population = 500; //flat
        private static int plantPop = 1000; //flat
        private static int obstacleNumber = 125000; //flat

        //judging
        private static int energyWeight = 2; //energy * x
        private static int healthWeight = 3;//health * x
        private static int topPercentage = 25; //top x% are the best performers, their children make up 50% of the next gen
        private static int elimPercentage = 25; //bottom x% eliminated

        //creatures
        private static int starvingEnergyLevel = 5000; //flat
        private static int woundedHealthPercent = 15; //%age

        private static int energyDrainPerTick = 5; //flat
        private static int staminaRejuvenationPercent = 1; //%age
        private static int healthRejuvenationPercent = 1; //%age

        private static int mutationChance = 100; // 1:x chance
        private static bool normaliseDiet = false; //bool

        //Round rules
        private static int roundLengths = 10000; //flat
        private static int currentRoundTime; //counter

        private static int numGenerations = -1; //targer generation, we will normally reach here and then stop
        private static int currentGeneration = 1; //the current gen

        private static bool goToJudgeOnDeath = true; //boolean representing if we immediately judge a generation once its last member dies out
        private static bool followOnClick = true; //boolean representing if we follow the selected creature/plant/remains when clicked on

        private static Color[] colourMap = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
        private static GraphicsDeviceManager graphics;
        private static SpriteBatch spriteBatch;
        private static SpriteFont spriteFont;

        //Simulation class inner workings
        private static SimulationState state; //the current sim state, all update and draw methods point to this
        private static WorldState theWorld; //the world, stored since it needs to be kept consistent

        public Simulation()
        {
            currentSimulation = this;
            parseShapes("Content/Shapes.txt");
            currentRoundTime = 0;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        #region Methods

        #region Game rules related

        /// <summary>
        /// Gets if the creature should normalise its diet, that is, if 3s will move the diet value back towards 0.5, if false 3s have no effect on diet
        /// </summary>
        /// <returns>A bool representing if the diet of creatures should be normalised by 3s</returns>
        public static bool getNormaliseDiet()
        {
            return normaliseDiet;
        }

        /// <summary>
        /// Sets if the creature should normalise its diet
        /// </summary>
        /// <param name="val">A bool to set the normalisation of the diet to</param>
        public static void setNormaliseDiet(bool val)
        {
            normaliseDiet = val;
        }

        /// <summary>
        /// Gets the number of ticks that have been performed this generation so far
        /// </summary>
        /// <returns>The number of ticks that have been performed this generation</returns>
        public static int getNumTicks()
        {
            return currentRoundTime;
        }

        /// <summary>
        /// Gets how many ticks each generation should last for
        /// </summary>
        /// <returns>The number of ticks each generation should last for</returns>
        public static int getRoundLength()
        {
            return roundLengths;
        }

        /// <summary>
        /// Sets how many ticks each generation should last for
        /// </summary>
        /// <param name="val">The number of ticks to set the generation length to</param>
        public static void setRoundLength(int val)
        {
            roundLengths = val;
        }

        /// <summary>
        /// Gets the number of the current generation, generations start at generation 1 and are incremented each time the judging state is reached
        /// </summary>
        /// <returns>The number the current generation</returns>
        public static int getGeneration()
        {
            return currentGeneration;
        }

        /// <summary>
        /// Gets the target generation of the Simulation, the generation to stop ticking the world at
        /// </summary>
        /// <returns>The target generation to stop ticking the world at</returns>
        public static int getTargetGeneration()
        {
            return numGenerations;
        }

        /// <summary>
        /// Sets the target generation for the program to a specified value
        /// </summary>
        /// <param name="val">The value to set the target generation to</param>
        public static void setTargetGeneration(int val)
        {
            numGenerations = val;
        }

        /// <summary> //TODO: move to Display
        /// Gets an array of Colors that are mapped to different colours in the genome
        /// </summary>
        /// <returns>The array of colours that the Simulation stores</returns>
        public static Color[] getColours()
        {
            return colourMap;
        }

        /// <summary>
        /// Gets the graphics device manager associated with this game
        /// </summary>
        /// <returns>The graphics device manager used by the game</returns>
        public static GraphicsDeviceManager getGraphicsDeviceManager()
        {
            return graphics;
        }

        /// <summary>
        /// Gets the list of recognised shapes that have been parsed from shapes.txt
        /// </summary>
        /// <returns>The list of shapes that can be searched for in the gene</returns>
        public static List<Shape> getShapes()
        {
            return recognisedShapes;
        }

        /// <summary>
        /// Gets the health regeneration speed as a percentage (1 to 100)
        /// </summary>
        /// <returns>The percentage of health for the creature to regenerate if it has less than maximum health</returns>
        public static int getHealthRegenSpeed()
        {
            return healthRejuvenationPercent;
        }

        /// <summary>
        /// Sets the percentage of health for a creature to regenerate when it has less than maximum health 
        /// </summary>
        /// <param name="healthSpeed">The value to set the health value to</param>
        public static void setHealthRegenSpeed(int healthSpeed)
        {
            healthRejuvenationPercent = healthSpeed;
        }

        /// <summary>
        /// Gets the stamina regeneration speed as a percentage (1 to 100)
        /// </summary>
        /// <returns>The percentage of stamina for the creature to regenerate if it has less than maximum stamina</returns>
        public static int getStaminaRegenSpeed()
        {
            return staminaRejuvenationPercent;
        }

        /// <summary>
        /// Sets the percentage of stamina for a creature to regenerate when it has less than maximum stamina 
        /// </summary>
        /// <param name="healthSpeed">The value to set the stamina value to</param>
        public static void setStaminaRegenSpeed(int stamSpeed)
        {
            staminaRejuvenationPercent = stamSpeed;
        }

        /// <summary>
        /// Gets the energy drain a creature takes per tick from simply continuing to exist, a flat value
        /// </summary>
        /// <returns>The energy drain per tick as an int</returns>
        public static int getEnergyDrainPerTick()
        {
            return energyDrainPerTick;
        }

        /// <summary>
        /// Sets the energy drain a creature takes per tick from simply continuing to exist, a flat value
        /// </summary>
        /// <param name="newDrainSpeed">The value to set the energy drain per tick to</param>
        public static void setEnergyDrainPerTick(int newDrainSpeed)
        {
            energyDrainPerTick = newDrainSpeed;
        }
        
        /// <summary>
        /// Gets the starting population of creatures in the world
        /// </summary>
        /// <returns>The number of creatures that are placed in the world when it is first created</returns>
        public static int getPopulation()
        {
            return population;
        }

        /// <summary>
        /// Sets the population of creatures in the world
        /// </summary>
        /// <param name="val">The value to set the population of creatures in the world to</param>
        public static void setPopulation(int val)
        {
            population = val;
        }

        /// <summary>
        /// Gets the percentage of creatures to eliminate during the judging that occurs at the end of each generation
        /// </summary>
        /// <returns>The percentage of creatures to eliminate during the judging as an int from 1 to 100</returns>
        public static int getElimPercentage()
        {
            return elimPercentage;
        }

        /// <summary>
        /// Sets the percentage of creatures to eliminate during the judging that occurs at the end of each generation
        /// </summary>
        /// <param name="val">The percentage of creature to eliminate as an int from 1 to 100</param>
        public static void setElimPercentage(int val)
        {
            elimPercentage = val;
        }

        /// <summary>
        /// Gets the percentage of creatures to breed to make up 50% of the next generation when judging
        /// </summary>
        /// <returns>The percentage of creatures to breed together to make up 50% of the next generation</returns>
        public static int getTopPercentage()
        {
            return topPercentage;
        }

        /// <summary>
        /// Sets the percentage of creatures to make up 50% of the next generation when judging
        /// </summary>
        /// <param name="val">The value to set the top percentage to</param>
        public static void setTopPercentage(int val)
        {
            topPercentage = val;
        }

        /// <summary>
        /// Gets the current world associated with the simulation
        /// </summary>
        /// <returns>The world that that is currently being used by the simulation</returns>
        public static WorldState getCurrentWorld()
        {
            return theWorld;
        }

        /// <summary>
        /// Gets the weight that energy is given by the judging algorithm
        /// </summary>
        /// <returns>The value that the energy of each creature will be multiplied by when judging fitness</returns>
        public static int getEnergyWeight()
        {
            return energyWeight;
        }

        /// <summary>
        /// Sets the weight that energy is given by the judging algorithm
        /// </summary>
        /// <returns>The value to set the multiplier applied to energy by the judging state to</returns>
        public static void setEnergyWeight(int val)
        {
            energyWeight = val;
        }

        /// <summary>
        /// Gets the chance of a cell mutating during the breeding process, P(mutation) = 1/value
        /// </summary>
        /// <returns>The likelihood of a cell mutating when 2 genes are bred together</returns>
        public static int getMutationChance()
        {
            return mutationChance;
        }

        /// <summary>
        /// Sets the chance of a cell mutating during the breeding process
        /// </summary>
        /// <returns>The value to set the likelihood of mutation to</returns>
        public static void setMutationChance(int val)
        {
            mutationChance = val;
        }

        /// <summary>
        /// Gets the weight that health is given by the judging algorithm
        /// </summary>
        /// <returns>The value that the health of each creature will be multiplied by when judging fitness</returns>
        public static int getHealthWeight()
        {
            return healthWeight;
        }

        /// <summary>
        /// Sets the weight that health is given by the judging algorithm
        /// </summary>
        /// <returns>The value to set the multiplier applied to health by the judging state to</returns>
        public static void setHealthWeight(int val)
        {
            healthWeight = val;
        }

        /// <summary>
        /// Gets how many obstacles should be placed in the world when it is generated
        /// </summary>
        /// <returns>The number of obstacles to place in the world during generation</returns>
        public static int getNumObstacles()
        {
            return obstacleNumber;
        }

        /// <summary>
        /// Sets how many obstacles are placed in the world when it is generated
        /// </summary>
        /// <param name="val">The value to set the population of obstacles to</param>
        public static void setNumObstacles(int val)
        {
            obstacleNumber = val;
        }

        /// <summary>
        /// Gets the number of ticks it takes before a remains object decays
        /// </summary>
        /// <returns>The number of ticks before a remains object decays as an int</returns>
        public static int getNumTicksToDecayRemains()
        {
            return numTicksRemainsDecay;
        }

        /// <summary>
        /// Sets the number of ticks it takes for a remains to decay a single food unit
        /// </summary>
        /// <param name="val">The number of ticks to set the act timer in remains to</param>
        public static void setNumTicksToDecayRemains(int val)
        {
            numTicksRemainsDecay = val;
        }

        /// <summary>
        /// Gets the number of ticks it takes befor a plant object regrows
        /// </summary>
        /// <returns>The number of ticks before a plant regrows as an int</returns>
        public static int getNumTicksToRegrowPlant()
        {
            return numTicksPlantRegrow;
        }

        /// <summary>
        /// Sets the number of ticks a plant must by undisturbed before it regrows a single food unit
        /// </summary>
        /// <param name="val">The value to set the act timer of the plant objects to</param>
        public static void setNumTicksToRegrowPlant(int val)
        {
            numTicksPlantRegrow = val;
        }

        /// <summary>
        /// Gets the maximum number of food units a plant can have in it
        /// </summary>
        /// <returns>An int representing the number of food units a plant can contain</returns>
        public static int getPlantFoodMax()
        {
            return numFoodUnitsPlant;
        }

        /// <summary>
        /// Sets the maximum number of food units a plant can have in it to a specified value
        /// </summary>
        /// <param name="val">The value to set the maximum number of food units for a plant to</param>
        public static void setPlantFoodMax(int val)
        {
            numFoodUnitsPlant = val;
        }

        /// <summary>
        /// Gets the nutritional value of plants, that is, the amount of energy a plant would give a perfect herbivore
        /// </summary>
        /// <returns>The amount of energy a plant would give a perfect herbivore</returns>
        public static int getPlantFoodValue()
        {
            return plantFoodValue;
        }

        /// <summary>
        /// Sets the nutritional value of plants
        /// </summary>
        /// <param name="val">An int to set the food value of the plants to</param>
        public static void setPlantFoodValue(int val)
        {
            plantFoodValue = val;
        }

        /// <summary>
        /// Gets the amount of variation between plants, plants can have a nutritional value equal to 1 +/- 1/x, with a 0 value variation is removed
        /// </summary>
        /// <returns>A value representing the variation between plant foor values</returns>
        public static int getPlantFoodValueVariation()
        {
            return plantFoodValueVariation;
        }

        /// <summary>
        /// Sets the amount of variation in food value between plants to a specified value
        /// </summary>
        /// <param name="val">The value to set the variation between plant food values to, with a 0 value there is no variation</param>
        public static void setPlantFoodValueVariation(int val)
        {
            plantFoodValueVariation = val;
        }

        /// <summary>
        /// Gets the number of plants the generated world will have in it
        /// </summary>
        /// <returns>The plant population of the world</returns>
        public static int getPlantPopulation()
        {
            return plantPop;
        }
        
        /// <summary>
        /// Sets the number of plants the generated world will have in it to a specified value
        /// </summary>
        /// <param name="val">The value to set the plant population to</param>
        public static void setPlantPopulation(int val)
        {
            plantPop = val;
        }

        /// <summary>
        /// Gets the amount of food units a remains start as having
        /// </summary>
        /// <returns>The number of food units a remains object starts off containing</returns>
        public static int getRemainsFoodAmount()
        {
            return numFoodUnitsRemains;
        }

        /// <summary>
        /// Sets the number of food units a remains starts with to a specified value
        /// </summary>
        /// <param name="val">An int to set the starting number of food units in a remains object to</param>
        public static void setRemainsFoodAmount(int val)
        {
            numFoodUnitsRemains = val;
        }

        /// <summary>
        /// Gets the base amount of food a food unit from a remains object is worth
        /// </summary>
        /// <returns>The amount of energy a perfect carnivoew will get from a remains food unit when variation is 0</returns>
        public static int getRemainsFoodValue()
        {
            return remainsFoodValue;
        }

        /// <summary>
        /// Sets the amount a food unit from a remains item is worth to a perfect carnivore to a specified value
        /// </summary>
        /// <param name="val">An int to set the base nutritional value of remains to</param>
        public static void setRemainsFoodValue(int val)
        {
            remainsFoodValue = val;
        }

        /// <summary>
        /// Gets the variation that remains food values can have, which is 1 +/- (1/x)
        /// </summary>
        /// <returns>The amount of variation the nutritional value of remains can have, with smaller numbers being greater variation</returns>
        public static int getRemainsFoodValueVariation()
        {
            return remainsFoodValueVariation;
        }

        /// <summary>
        /// Sets the amount of variation the remains can have to a specified value, with lower having greater variation
        /// </summary>
        /// <param name="val">X in the formula 1 +/- (1/X) which is used to decide remains values</param>
        public static void setRemainsFoodValueVariation(int val)
        {
            remainsFoodValueVariation = val;
        }

        /// <summary>
        /// Gets the energy level below which creatures recognise themselves as starving, a flat value
        /// </summary>
        /// <returns>The flat energy level below which creatures are starving</returns>
        public static int getStarvingEnergyLevel()
        {
            return starvingEnergyLevel;
        }

        /// <summary>
        /// Set the energy level below which creatures are starving to a specified int value
        /// </summary>
        /// <param name="val">The value to set the starving energy level to</param>
        public static void setStarvingEnergyLevel(int val)
        {
            starvingEnergyLevel = val;
        }

        /// <summary>
        /// Gets the health level below which creatures recognise themselves as wounded, a percentage
        /// </summary>
        /// <returns>The percentage of health below which creatures are wounded</returns>
        public static int getWoundedHealthPercent()
        {
            return woundedHealthPercent;
        }

        /// <summary>
        /// Sets the percentage of health below which a creature sees itself as wounded to specified int value
        /// </summary>
        /// <param name="val">The value to set the wounded health percentage to</param>
        public static void setWoundedHealthPercent(int val)
        {
            woundedHealthPercent = val;
        }

        /// <summary>
        /// Gets a bool representing if the viewer should follow the clicked on object
        /// </summary>
        /// <returns>Returns the stored followOnClick value, if true the viewer should follow any selected object around the world</returns>
        public static bool getFollowOnClick()
        {
            return followOnClick;
        }

        /// <summary>
        /// Sets if the viewer should follow a selected object
        /// </summary>
        /// <param name="val">The value to set the bool representing if the viewer should follow the selected object to</param>
        public static void setFollowOnClick(bool val)
        {
            followOnClick = val;
        }

        #endregion

        #region game running logic

        /// <summary>
        /// Deals with what the Simulation should do when the world ticks, increments the number of ticks elapsed in the current round and
        /// checks if the Simulation should transition to the judging state
        /// </summary>
        public static void tick()
        {
            currentRoundTime++;
            if (currentRoundTime >= roundLengths)
            {
                judge();
            }
        }

        /// <summary>
        /// Gets if the simulation should go to the judging state if all the creatures are dead
        /// </summary>
        /// <returns>a bool representing if the simulation should go to the judging state if all the creatures are dead</returns>
        public static bool judgeIfAllDead()
        {
            return goToJudgeOnDeath;
        }

        /// <summary>
        /// Sets the judging flag to a specified value
        /// </summary>
        /// <param name="val">The value to set the flag for judging to</param>
        public static void setJudgeIfAllDead(bool val)
        {
            goToJudgeOnDeath = val;
        }

        /// <summary>
        /// Transitions to the judging state, getting the necessary input from the world and initialising the judging state
        /// </summary>
        public static void judge()
        {
            WorldState w = theWorld;
            state = new JudgingState(w.getLiveCreatures(), w.getDeadCreatures());
        }

        /// <summary>
        /// Calls when the judging state finishes judging, and adds the next set of creatures to the world after it is reset
        /// </summary>
        /// <param name="creatureList">The list of creatures the judging state has created</param>
        public static void judgingDone(List<Creature> creatureList)
        {
            theWorld.reset(creatureList);
            state = theWorld;
            currentRoundTime = 0;
            currentGeneration++;
        }

        /// <summary>
        /// Transitions to the main menu state
        /// </summary>
        public static void goToMenu()
        {
            state = new MainMenuState();
        }

        /// <summary>
        /// Transitions to the options menu state
        /// </summary>
        public static void options()
        {
            state = new OptionsState();
        }

        /// <summary>
        /// Exits the simulation, calling any methods needed to unload content
        /// </summary>
        public static void quit()
        {
            currentSimulation.Exit();
        }

        /// <summary>
        /// Restarts and regenerates the world and creatures by returning to the initialisation state
        /// </summary>
        public static void restart()
        {
            state = new InitialisationState();
            currentGeneration = 1;
            currentRoundTime = 0;
        }

        /// <summary>
        /// Resumes the world updating, and returns to the world state from the menu
        /// </summary>
        public static void resume()
        {
            state = theWorld;
        }

        /// <summary>
        /// Begins the world state simulation, generating a new world and setting it to the current state
        /// </summary>
        /// <param name="creatureList">The list of creatures to add to the new world</param>
        /// <param name="seed">The seed of the random number generator to add to the world</param>
        public static void begin(List<Creature> creatureList, int seed)
        {
            theWorld = new WorldState(seed, creatureList);
            state = theWorld;
        }

        #endregion

        #region overriding methods to make the game work

        /// <summary>
        /// Initialises the necessary parts of the Simulation
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;
            theWorld = null;
            state = new InitialisationState();
        }

        /// <summary>
        /// Loads all content required by the entire Simulation
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont");
            //using a dictionary to set up the textures to allow us to store metadata about them in the form of an enum listing their names
            Dictionary<TextureNames, string> texList = new Dictionary<TextureNames, string>();
            texList.Add(TextureNames.EMPTY, "BlankTile");
            texList.Add(TextureNames.CREATURE, "Creature");
            texList.Add(TextureNames.OBSTACLE, "Obstacle");
            texList.Add(TextureNames.PLANT, "Plant");
            texList.Add(TextureNames.REMAINS, "Remains");
            texList.Add(TextureNames.SPEEDUP, "plus");
            texList.Add(TextureNames.SLOWDOWN, "minus");
            texList.Add(TextureNames.MENU, "MenuButton");
            texList.Add(TextureNames.TOP, "TopMenu");
            texList.Add(TextureNames.VIEWBACK, "ViewingBack");
            texList.Add(TextureNames.PLANT_DEPLETED, "PlantDep");
            texList.Add(TextureNames.MOREINFO, "MoreInfoBtn");
            texList.Add(TextureNames.RESUME, "Resume");
            texList.Add(TextureNames.RESTART, "Restart");
            texList.Add(TextureNames.OPTIONSMENUBTN, "Options");
            texList.Add(TextureNames.QUIT, "Quit");
            texList.Add(TextureNames.EMPTYBTN, "BlankButton");
            texList.Add(TextureNames.BACK, "BackButton");
            texList.Add(TextureNames.KILL, "Kill");
            texList.Add(TextureNames.CLONE, "Clone");
            TextureNames[] texNames = texList.Keys.ToArray<TextureNames>();

            Dictionary<TextureNames, Texture2D>  textures = new Dictionary<TextureNames, Texture2D>();

            foreach (TextureNames s in texNames)
            {
                textures.Add(s, Content.Load<Texture2D>(texList[s]));
            }

            Display.setTextures(textures);
            Display.setFont(spriteFont);
            Display.setSpriteBatch(spriteBatch);

            base.LoadContent();
        }

        /// <summary>
        /// The update method of the Simulation, calls the update method of the current state of the simulation
        /// </summary>
        /// <param name="gameTime">The time since update was last called</param>
        protected override void Update(GameTime gameTime)
        {
            #region testing
#if CELLTEST
            int[][] colours = new int[6][];
            colours[0] = new int[] { 0, 0 }; //Check the same
            colours[1] = new int[] { 7, 7 }; //Check the same
            colours[2] = new int[] { 1, 3 }; //Check different
            colours[3] = new int[] { 3, 1 }; //Check different but the other way around
            colours[4] = new int[] { 7, 1 }; //Check different and wrapping 
            colours[5] = new int[] { 1, 7 }; //Check different and wrapping but the other way around
            foreach(int[] i in colours)
            {
                Cell c = new Cell(i[0], i[1]);
                Console.WriteLine("C1: " + i[0] + " C2: " + i[1] + " DOM: " + c.getDomColour());
            }
            Environment.Exit(0);
#endif
#if PATTERNTEST
            //Code for checking various things
            Cell[][] c = new Cell[3][];
            c[0] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(0, 0) };
            c[1] = new Cell[] { new Cell(0, 0), new Cell(1, 1), new Cell(0, 0) };
            c[2] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(1, 1) };
            geneTest(new Gene(c));

            c[0] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(0, 0) };
            c[1] = new Cell[] { new Cell(0, 0), new Cell(1, 1), new Cell(0, 0) };
            c[2] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(0, 0) };
            geneTest(new Gene(c));

            c[0] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(1, 1) };
            c[1] = new Cell[] { new Cell(0, 0), new Cell(1, 1), new Cell(0, 0) };
            c[2] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(0, 0) };
            geneTest(new Gene(c));

            c[0] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(1, 1) };
            c[1] = new Cell[] { new Cell(0, 0), new Cell(1, 1), new Cell(0, 0) };
            c[2] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(1, 1) };
            geneTest(new Gene(c));

            c[0] = new Cell[] { new Cell(0, 0), new Cell(6, 6), new Cell(1, 1), new Cell(1, 1) };
            c[1] = new Cell[] { new Cell(1, 1), new Cell(0, 0), new Cell(6, 6), new Cell(1, 1) };
            c[2] = new Cell[] { new Cell(1, 1), new Cell(1, 1), new Cell(0, 0), new Cell(6, 6) };
            geneTest(new Gene(c));

            c[0] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(0, 0) };
            c[1] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(0, 0) };
            c[2] = new Cell[] { new Cell(0, 0), new Cell(0, 0), new Cell(1, 1) };

            Cell[][] rCells = new Cell[1][];
            rCells[0] = new Cell[] { new Cell(1, 1) };
            List<ParamToken> rMods = new List<ParamToken>();
            rMods.Add(ParamToken.STRENGTH);
            recognisedShapes = new List<Shape>();
            recognisedShapes.Add(new Shape(rCells, rMods, new List<ParamToken>()));

            geneTest(new Gene(c));

            Environment.Exit(0);
#endif
#if POLLTEST
            Cell[][] newGene = new Cell[10][];
            for (int colour = 0; colour < 7; colour++)
            {
                for (int i = 0; i < newGene.Length; i++)
                {
                    newGene[i] = new Cell[10];
                    for (int j = 0; j < newGene[i].Length; j++)
                    {
                        newGene[i][j] = new Cell(colour, colour);
                    }
                }
                creatureTest(newGene);
            }


            for (int i = 0; i < newGene.Length; i++)
            {
                newGene[i] = new Cell[10];
                for (int j = 0; j < newGene[i].Length; j++)
                {
                    newGene[i][j] = new Cell(0, 0);
                }
            }
            newGene[0][0] = new Cell(0, 0); newGene[0][1] = new Cell(0, 0); newGene[0][2] = new Cell(0, 0);
            newGene[1][0] = new Cell(0, 0); newGene[1][1] = new Cell(0, 0); newGene[1][2] = new Cell(0, 0);
            newGene[2][0] = new Cell(0, 0); newGene[2][1] = new Cell(0, 0); newGene[2][2] = new Cell(1, 1);
            creatureTest(newGene);

            newGene[0][0] = new Cell(0, 0); newGene[0][1] = new Cell(0, 0); newGene[0][2] = new Cell(0, 0);
            newGene[1][0] = new Cell(0, 0); newGene[1][1] = new Cell(2, 2); newGene[1][2] = new Cell(1, 1);
            newGene[2][0] = new Cell(1, 1); newGene[2][1] = new Cell(1, 1); newGene[2][2] = new Cell(1, 1);
            creatureTest(newGene);

            newGene[0][0] = new Cell(1, 1); newGene[0][1] = new Cell(0, 0); newGene[0][2] = new Cell(0, 0);
            newGene[1][0] = new Cell(0, 0); newGene[1][1] = new Cell(2, 2); newGene[1][2] = new Cell(1, 1);
            newGene[2][0] = new Cell(1, 1); newGene[2][1] = new Cell(1, 1); newGene[2][2] = new Cell(0, 0);
            creatureTest(newGene);

            Environment.Exit(0);
#endif
#if DIETTEST
            Cell[][] cc = new Cell[10][];
            Cell[][] ch = new Cell[10][];
            for (int i = 0; i < 10; i++)
            {
                cc[i] = new Cell[10];
                ch[i] = new Cell[10];
                for (int j = 0; j < 10; j++)
                {
                    cc[i][j] = new Cell(0, 0);
                    ch[i][j] = new Cell(6, 6);
                }
            }
            Creature carn = new Creature(new Gene(cc));
            Creature herb = new Creature(new Gene(ch));
            List<FoodSource> foodToSort = new List<FoodSource>();
            plantFoodValue = 1000;
            remainsFoodValue = 1000;
            foodToSort.Add(new Plant(new Random()));
            foodToSort.Add(new Remains(new Random()));

            FoodSource carnChoice = carn.pubGetMostNourishing(foodToSort);
            FoodSource herbChoice = herb.pubGetMostNourishing(foodToSort);

            Console.WriteLine("Diet: Carnivore : " + carn.getDiet() + " Herbivore: " + herb.getDiet());
            Console.WriteLine("Actual values: Plant: " + foodToSort[0].getFoodValue() + " Remains: " + foodToSort[1].getFoodValue());
            Console.WriteLine("Carnivore sees plant as: " + carn.pubGetNourishmentAmt(foodToSort[0]) + " and remains as " + carn.pubGetNourishmentAmt(foodToSort[1]));
            Console.WriteLine("Herbivore sees plant as: " + herb.pubGetNourishmentAmt(foodToSort[0]) + " and remains as " + herb.pubGetNourishmentAmt(foodToSort[1]));
            Console.WriteLine("Carnivore chooses meat: " + !carnChoice.isPlant());
            Console.WriteLine("Herbivore chooses plant: " + herbChoice.isPlant());
            Environment.Exit(0);
#endif
#if BREEDTEST
            Cell[][] newGene = new Cell[10][];
            for (int i = 0; i < newGene.Length; i++)
            {
                newGene[i] = new Cell[10];
                for (int j = 0; j < newGene[i].Length; j++)
                {
                    newGene[i][j] = new Cell(0, 0);
                }
            }
            Gene g = new Gene(newGene, new Random());
            for (int i = 0; i < 100; i++)
            {
                g.breedWith(g);
            }
            Environment.Exit(0);
#endif
            #endregion
            base.Update(gameTime);
            state.update(gameTime);
        }

        /// <summary>
        /// The draw method of the Simulation, calls the draw method of the current state of the simulation, after performing some
        /// common procedures
        /// </summary>
        /// <param name="gameTime">The time since update was last called</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.White);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            state.draw();
        }

        /// <summary>
        /// Unloads all content that needs to be uploaded by the Simulation
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

        #region Parsers

        /// <summary>
        /// Parses all the shapes from the specified files into a List of Shape objects, that can be used by the Genes to set up their parameters
        /// </summary>
        /// <param name="fileLocation">The location of the file to parse the shapes from</param>
        private void parseShapes(String fileLocation)
        {
            try
            {
                StreamReader sr = new StreamReader(fileLocation);
                recognisedShapes = new List<Shape>();
                string s = sr.ReadLine();
                while (!s.Equals("### BEGIN ###")) //parser starts at ### BEGIN ###
                {
                    s = sr.ReadLine();
                }
                s = sr.ReadLine();
                while (s != null)
                {
                    string[] dims = s.Split('x'); //each shape begins with a description of its dimensions in the form AxB;
                    int dimX = int.Parse(dims[0]);
                    int dimY = int.Parse(dims[1]);
                    s = sr.ReadLine();
                    int[][] cells = new int[dimX][];
                    List<ParamToken> pMods = new List<ParamToken>();
                    List<ParamToken> nMods = new List<ParamToken>();
                    for(int i = 0; i < dimY; i++)
                    {
                        cells[i] = new int[dimY];
                        char[] c = s.ToCharArray();
                        for(int j = 0; j < cells[i].Length; j++)
                        {
                            if(c[j].Equals('A')) //A stands for any value, represented by -1 in the shape text
                            {
                                cells[i][j] = -1;
                            }
                            else
                            {
                                cells[i][j] = (int)char.GetNumericValue(c[j]);
                                cells[i][j]--;
                            }
                        }
                        s = sr.ReadLine();
                    }
                    s = sr.ReadLine();
                    while(!s.Equals("###")) //param input ends at ###
                    {
                        char[] split = s.ToCharArray();
                        char[] t = new char[] { split[0], split[1], split[2] };
                        string ident = new string(t);
                        
                        ParamToken p = ParamToken.STRENGTH;
                        if(ident.Equals("STR")) //first work out what parameter we're dealing with
                        {
                            p = ParamToken.STRENGTH;
                        }
                        else if (ident.Equals("SPD"))
                        {
                            p = ParamToken.SPEED;
                        }
                        else if (ident.Equals("AWA"))
                        {
                            p = ParamToken.AWARE;
                        }
                        else if (ident.Equals("DEF"))
                        {
                            p = ParamToken.DEFENCE;
                        }
                        else if (ident.Equals("ENG"))
                        {
                            p = ParamToken.ENERGY;
                        }
                        else if (ident.Equals("HTH"))
                        {
                            p = ParamToken.HEALTH;
                        }
                        else if(ident.Equals("STL"))
                        {
                            p = ParamToken.STEALTHVAL;
                        }
                        else
                        {
                            throw new Exception("Unrecognised parameter ident in shapes text");
                        }
                        for (int c = 3; c < split.Length; c++)
                        {
                            if(split[c].Equals('+'))
                            {
                                pMods.Add(p);
                            }
                            else if (split[c].Equals('-'))
                            {
                                nMods.Add(p);
                            }
                            else
                            {
                                throw new Exception("Non +/- character used as incrementor/decrementor in shapes text");
                            }
                        }
                        s = sr.ReadLine();
                    }
                    Shape shape = new Shape(cells, pMods, nMods);
                    recognisedShapes.Add(shape);
                    s = sr.ReadLine();
                }
            }
            catch(Exception e)
            {
                Console.Beep();
                Console.WriteLine(e.Message);
            }
        }

        #endregion

#if PATTERNTEST
        private void geneTest(Gene g)
        {
            List<ParamToken> pm = g.getPosMods();
            List<ParamToken> nm = g.getNegMods();
            Console.WriteLine();
            Console.WriteLine("POSITIVES");
            foreach (ParamToken pos in pm)
            {
                Console.WriteLine(pos);
            }
            Console.WriteLine();
            Console.WriteLine("NEGATIVES");
            foreach (ParamToken neg in nm)
            {
                Console.WriteLine(neg);
            }
        }
#endif
#if POLLTEST
        private void creatureTest(Cell[][] newGene)
        {
            Creature cret = new Creature(new Gene(newGene));
            Dictionary<Scenario, Response> beh = cret.getBehaviour();
            foreach (Scenario s in beh.Keys)
            {
                Console.WriteLine("Scen: " + s + " Resp: " + beh[s]);
            }
        }
#endif

        #endregion

    }
}
