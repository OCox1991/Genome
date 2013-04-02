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
    class Simulation : Game
    {
        private static Simulation currentSimulation;

        private static List<Shape> recognisedShapes = new List<Shape>();

        private static int remainsFoodValue = 1500; //flat
        private static int plantFoodValue = 750; //flat
        private static int remainsFoodValueVariation = 2; //  +- (1/x)
        private static int plantFoodValueVariation = 2; // +- 1/x
        private static int numTicksPlantRegrow = 750; //flat
        private static int numTicksRemainsDecay = 500; //flat
        private static int numFoodUnitsPlant = 3; //flat
        private static int numFoodUnitsRemains = 4; //flat

        private static int population = 100; //flat
        private static int plantPop = 10000; //flat
        private static int obstacleNumber = 25000; //flat

        //judging
        private static int energyWeight = 2; //energy * x
        private static int healthWeight = 3;//health * x
        private static int topPercentage = 25; //top x% are the best performers, their children make up 50% of the next gen
        private static int elimPercentage = 25; //bottom x% eliminated

        //creatures
        private static int starvingEnergyLevel = 250; //flat
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

        //TODO: display stuff: needs to be moved to Display
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
            //TODO: move to Display class
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        #region Methods

        #region Game rules related

        public static bool getNormaliseDiet()
        {
            return normaliseDiet;
        }

        public static void setNormaliseDiet(bool val)
        {
            normaliseDiet = val;
        }

        public static int getNumTicks()
        {
            return currentRoundTime;
        }

        public static int getRoundLength()
        {
            return roundLengths;
        }

        public static void setRoundLength(int val)
        {
            roundLengths = val;
        }

        public static int getGeneration()
        {
            return currentGeneration;
        }

        public static int getTargetGeneration()
        {
            return numGenerations;
        }

        public static void setTargetGeneration(int val)
        {
            numGenerations = val;
        }

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
            return healthRejuvenationPercent;
        }

        public static void setHealthRegenSpeed(int healthSpeed)
        {
            healthRejuvenationPercent = healthSpeed;
        }

        public static int getStaminaRegenSpeed()
        {
            return staminaRejuvenationPercent;
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

        public static int getMutationChance()
        {
            return mutationChance;
        }

        public static void setMutationChance(int val)
        {
            mutationChance = val;
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

        public static bool getFollowOnClick()
        {
            return followOnClick;
        }

        public static void setFollowOnClick(bool val)
        {
            followOnClick = val;
        }

        #endregion

        #region game running logic

        public static void tick()
        {
            currentRoundTime++;
            if (currentRoundTime >= roundLengths)
            {
                judge();
            }
        }

        public static bool judgeIfAllDead()
        {
            return goToJudgeOnDeath;
        }

        public static void setJudgeIfAllDead(bool val)
        {
            goToJudgeOnDeath = val;
        }

        public static void judge()
        {
            WorldState w = theWorld;
            state = new JudgingState(w.getLiveCreatures(), w.getDeadCreatures());
        }

        public static void judgingDone(List<Creature> creatureList)
        {
            theWorld.reset(creatureList);
            state = theWorld;
            currentRoundTime = 0;
            currentGeneration++;
        }

        public static void goToMenu()
        {
            state = new MainMenuState();
        }

        public static void options()
        {
            state = new OptionsState();
        }

        public static void quit()
        {
            currentSimulation.Exit();
        }

        public static void restart()
        {
            state = new InitialisationState();
            currentGeneration = 1;
            currentRoundTime = 0;
        }

        public static void resume()
        {
            state = theWorld;
        }

        public static void begin(List<Creature> creatureList, Random r, int seed)
        {
            theWorld = new WorldState(r, seed, creatureList);
            state = theWorld;
        }

        #endregion

        #region overriding methods to make the game work

        protected override void Initialize()
        {
            base.Initialize();
            //TODO: initialise Display here
            this.IsMouseVisible = true;
            theWorld = null;
            state = new InitialisationState();
        }

        protected override void LoadContent()
        {
            //TODO: move all this stuff to Display
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
            if (numGenerations > currentGeneration || numGenerations == -1) 
            {
                state.update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //TODO: call Display.draw to do the next 5 lines then call state.draw()
            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.White);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
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
