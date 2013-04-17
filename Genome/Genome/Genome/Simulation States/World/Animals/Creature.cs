using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// A creature is a world object, it moves around the world, trying to maintain its energy level and not die
    /// </summary>
    class Creature : WorldObject
    {
        private Gene dna; //Creature's genes
        private WorldState world; //The world the creature lives in

        private bool inCombat; //Is the creature in combat?
        private bool stealth; //Is the creature trying to be stealthy?
        private Creature inCombatWith; //Is the creature in combat with any other creature
        Random random; //The random number generator used by the creature

        private Scenario scen; //The scenario the creature is using
        private Response resp; //The response to that scenario
        public Scenario CurrentScenario //The public accessor for the scen variable
        {
            get { return scen; }
        }
        public Response CurrentResponse //The public accessor for the resp var
        {
            get { return resp; }
        }

        #region stats
        //Dealing with the various parameters stored by this parameter based alife
        private int health; //The creature's health
        private int maxHealth; //The max health the creature can have
        private int energy; //The creature's energy
        private int initEnergy; //The energy the creature starts with (can be exceeded)

        private int strength; //The creature's strength, used in combat
        private int speed; //The creature's speed, used to determine move order, if the creature can escape combat successfully, and the number of move actions the creature makes when moving towards something
        private int awareness; //The creature's awareness, how far it can see, used to counter stealth and determine sight radius
        private int defence; //The creature's defence, used in combat and to reduce damage taken
        private double diet; //The creature's diet from 1 () to 0 //TODO: get correct vals for this
        private int stealthVal; //The stealth value of the creature, representing how good it is at avoiding detection, used in hiding

        private int stamina; //The creature's stamina, used to limit move speed
        private int maxStamina; //The creature's max stamina

        private Dictionary<Scenario, Response> behaviour; //The creatures behaviour, stored as a dictionary mapping the scenario to the response

        #endregion

        /// <summary>
        /// Constructor for creatures, sets up a random genome and then calls init
        /// </summary>
        public Creature()
        {
            dna = new Gene();
            init(WorldState.RandomNumberGenerator);
        }

        /// <summary>
        /// Overloaded constructor for creature, takes a Gene object and uses that as this creature's dna
        /// </summary>
        /// <param name="dna"></param>
        public Creature(Gene dna)
        {
            this.dna = dna;
            init(WorldState.RandomNumberGenerator);
        }

        /// <summary>
        /// Overloaded constructor for creature, takes a Random object and uses that as the random to create the
        /// Gene object with instead of the WorldState's static RandomNumberGenerator
        /// </summary>
        /// <param name="r">The Random object to be used by this creature and its genome</param>
        public Creature(Random r)
        {
            dna = new Gene(r);
            init(r);
        }


        /// <summary>
        /// Initialises the creature, performing all tasks common to all the constructors, used to avoid reusing code
        /// </summary>
        /// <param name="r">The random number generator to be used by this creature</param>
        private void init(Random r)
        {
            this.random = r;
            setLocation(-1, -1); //Sets location to -1 -1 initially

            inCombat = false;
            stealth = false;

            //strength, speed, awareness, defence, stealthval
            //DEFAULT VALUES INITIALISATION

            awareness = 10;
            initEnergy = 100;
            strength = 50;
            speed = 2;
            defence = 50;
            stealthVal = 50;
            maxHealth = 100;
            diet = 0.5;
            if (dna.getPosMods().Count > 0)
            {
                foreach (ParamToken p in dna.getPosMods())
                {
                    switch (p)
                    {
                        case ParamToken.AWARE: awareness++;
                            break;
                        case ParamToken.DEFENCE: defence++;
                            break;
                        case ParamToken.ENERGY: initEnergy++;
                            break;
                        case ParamToken.HEALTH: maxHealth++;
                            break;
                        case ParamToken.SPEED: speed++;
                            break;
                        case ParamToken.STEALTHVAL: stealthVal++;
                            break;
                        case ParamToken.STRENGTH: strength++;
                            break;
                    }
                }
            }

            if (dna.getNegMods().Count > 0)
            {
                foreach (ParamToken p in dna.getNegMods())
                {
                    switch (p)
                    {
                        case ParamToken.AWARE: awareness--;
                            break;
                        case ParamToken.DEFENCE: defence--;
                            break;
                        case ParamToken.ENERGY: initEnergy--;
                            break;
                        case ParamToken.HEALTH: maxHealth--;
                            break;
                        case ParamToken.SPEED: speed--;
                            break;
                        case ParamToken.STEALTHVAL: stealthVal--;
                            break;
                        case ParamToken.STRENGTH: strength--;
                            break;
                    }
                }
            }

            //multiply some values to make them higher, eg energy should be around 1000 at default.
            initEnergy *= 100;

            awareness -= awareness % 2;
            awareness /= 2;

            health = maxHealth;
            energy = initEnergy;
            maxStamina = energy / 10;
            stamina = maxStamina;

            //Speed should always be 0 or greater
            if (speed < 0)
            {
                speed = 0;
            }

            decideBehaviours();
        }

        /// <summary>
        /// Decide the behaviours associated with this creature, and associate them with the correct Scenarios
        /// </summary>
        private void decideBehaviours()
        {
            behaviour = new Dictionary<Scenario, Response>();
            behaviour.Add(Scenario.NOTHING, Response.RANDOM);
            Scenario[] scenarios = Scenario.NOTHING.AllScenarios();
            Response[] responses = new Response[scenarios.Length];
            int scenariosChecked = 0;
            int i;
            int j;
            //Poll the various areas of the dna to get the responses
            for (i = 1; i < dna.getSizeX() - 1; i += 2)
            {
                for (j = 1; j < dna.getSizeY() - 1; j += 2)
                {
                    if (scenariosChecked < responses.Length)
                    {
                        responses[scenariosChecked] = scenarios[scenariosChecked].PossibleResponses()[dna.poll(i, j)]; 
                        //.PossibleResponses is an extension method of the Scenarios enum that returns all the possible resps for that genom
                        scenariosChecked++;
                    }
                    else
                    {
                        i = dna.getSizeX();
                        j = dna.getSizeY();
                    }
                }
            }
            for (i = 0; i < responses.Length; i++)
            {
                behaviour.Add(scenarios[i], responses[i]);
            }
            //Deal with diet based on the count of individual colours
            int[] colourCount = dna.getColourCount();
            for (i = 0; i < 3; i++)
            {
                double dietMod = (double)colourCount[i] * 0.005; //0.005 since max of 1 colour is 100 and default diet == 0.5 so 100 * 0.005 + 0.5 = 1 AKA the max/min value of the diet
                diet += dietMod; 
            }
            for (i = 4; i < 7; i++)
            {
                double dietMod = (double)colourCount[i] * 0.005; //0.005 since max of 1 colour is 100 and default diet == 0.5 so 100 * 0.005 + 0.5 = 1 AKA the max/min value of the diet
                diet -= dietMod; 
            }
            if (Simulation.getNormaliseDiet())
            {
                i = 3;
                if (diet > 0.5)
                {
                    diet -= (int)(float)colourCount[i] * 0.005;
                    if (diet < 0.5)
                    {
                        diet = 0.5;
                    }
                }
                else if (diet < 0.5)
                {
                    diet += (int)(float)colourCount[i] * 0.005;
                    if (diet > 0.5)
                    {
                        diet = 0.5;
                    }
                }
            }
        }

        #region methods
        #region accessor methods
        /// <summary>
        /// Gets the dna associated with this creature
        /// </summary>
        /// <returns>The dna associated with this creature</returns>
        public Gene getDna()
        {
            return dna;
        }

        /// <summary>
        /// Gets the current stamina value of the creature
        /// </summary>
        /// <returns>The current stamina value of the creature</returns>
        public int getStamina()
        {
            return stamina;
        }

        /// <summary>
        /// Gets the maximum possible stamina the creature can have
        /// </summary>
        /// <returns>The maximum stamina the creature can have</returns>
        public int getMaxStamina()
        {
            return maxStamina;
        }
        
        /// <summary>
        /// Gets the defence value of this creature
        /// </summary>
        /// <returns>The defence value of the creature as an int</returns>
        public int getDefence()
        {
            return defence;
        }

        /// <summary>
        /// Gets the health of this creature
        /// </summary>
        /// <returns>The health of the creature as an int</returns>
        public int getHealth()
        {
            return health;
        }

        /// <summary>
        /// Gets the stealth value of this creature
        /// </summary>
        /// <returns>The stealth value of the creature as an int</returns>
        public int getStealthVal()
        {
            return stealthVal;
        }

        /// <summary>
        /// Gets the strength value of this creature
        /// </summary>
        /// <returns>The strength value of the creature as an int</returns>
        public int getStrength()
        {
            return strength;
        }

        /// <summary>
        /// Gets if this creature is being stealthy or not
        /// </summary>
        /// <returns>A bool representing if the creature is being stealthy</returns>
        public bool isStealthy()
        {
            return stealth;
        }

        /// <summary>
        /// Gets the speed of this creature
        /// </summary>
        /// <returns>The speed value of the creature as an int</returns>
        public int getSpeed()
        {
            return speed;
        }

        /// <summary>
        /// Gets the awareness of this creature
        /// </summary>
        /// <returns>The awareness value of the creature as an int</returns>
        public int getAwareness()
        {
            return awareness;
        }

        /// <summary>
        /// Gets the energy value of this creature
        /// </summary>
        /// <returns>The energy value of this creature</returns>
        public int getEnergy()
        {
            return energy;
        }

        /// <summary>
        /// Returns the diet value of the creature
        /// </summary>
        /// <returns>The diet value of the creature</returns>
        public double getDiet()
        {
            return diet;
        }

        /// <summary>
        /// Gets the stat value of this creature, the sum of all its various stats. This is used to break ties when sorting creatures for the judging state
        /// </summary>
        /// <returns></returns>
        public int getStatValue()
        {
            return speed + awareness + defence + maxHealth + initEnergy + stealthVal + strength;
        }

        /// <summary>
        /// Sets the world of the creature to a given world
        /// </summary>
        /// <param name="world">The world to add the creature to</param>
        public void setWorld(WorldState world)
        {
            this.world = world;
        }

        #endregion

        /// <summary>
        /// Takes the action for 1 tick, rejuvenates health and stamina, drains energy, looks around, decides on scenario and response. For more
        /// detailed description of the action see the report
        /// </summary>
        public void tick()
        {
            #region 1. Rejuvenate
            if (health < maxHealth - ((double)(maxHealth / 100) * Simulation.getHealthRegenSpeed()))
            {
                health += (int)(((double)maxHealth / 100.0) * (double)Simulation.getHealthRegenSpeed());
            }
            else if(health < maxHealth)
            {
                health = maxHealth;
            }

            if (stamina < maxStamina - ((double)(maxStamina / 100) * Simulation.getStaminaRegenSpeed()))
            {
                stamina += (int)(((double)maxStamina / 100) * (double)Simulation.getStaminaRegenSpeed());
            }
            else if (stamina < maxStamina)
            {
                stamina = maxStamina;
            }
            #endregion
            #region 2. Drain energy
            drainEnergy(Simulation.getEnergyDrainPerTick(), false);
            #endregion
            #region 3. Look Around
            ArrayList l = new ArrayList();
            l = world.scan(getLocationXY(), awareness);
            List<Creature> cl = (List<Creature>)l[0];
            cl.Remove(this);
            List<Plant> pl = (List<Plant>)l[1];
            List<Remains> rl = (List<Remains>)l[2];
            List<Obstacle> ol = (List<Obstacle>)l[3];
            List<FoodSource> fl = new List<FoodSource>();
            fl.AddRange(pl.ToArray<FoodSource>());
            fl.AddRange(rl.ToArray<FoodSource>());
            #region dealing with stealth
            List<Creature> removeList = new List<Creature>();
            foreach (Creature c in cl)
            {
                if (c.isStealthy() && !canSee(c))
                {
                    removeList.Add(c);
                }
                else
                {
                    c.spotted();
                }
            }
            foreach (Creature c in removeList)
            {
                cl.Remove(c);
            }
            #endregion

            #endregion
            #region 4. Decide what scenario applies
            scen = Scenario.NOTHING;

            int creatureCount = cl.Count;
            int plantCount = pl.Count;
            int remainsCount = rl.Count;
            int foodCount = plantCount + remainsCount;

            if (inCombat && inCombatWith.getHealth() <= 0)
            {
                outOfCombat();
            }
            if(inCombat)
            {
                #region combat
                if (creatureCount > 1)
                {
                    scen = Scenario.IN_COMBAT_CREATURE;
                }
                else if((double)maxHealth/(double)health < (double)Simulation.getWoundedHealthPercent())
                {
                    scen = Scenario.IN_COMBAT_WOUNDED;
                }
                else
                {
                    scen = Scenario.IN_COMBAT;
                }
                #endregion
            }
            else if (creatureCount == 1)
            {
                #region single creature
                if (foodCount > 0 && energy < Simulation.getStarvingEnergyLevel())
                {
                    scen = Scenario.STARVING_CREATURE_FOOD;
                }
                else if (foodCount == 1)
                {
                    scen = Scenario.CREATURE_FOOD;
                }
                else if (foodCount > 1)
                {
                    scen = Scenario.CREATURE_MULT_FOOD;
                }
                else
                {
                    scen = Scenario.CREATURE;
                }
                #endregion
            }
            else if (creatureCount > 1)
            {
                #region multi creature
                if (foodCount > 0 && energy < Simulation.getStarvingEnergyLevel())
                {
                    scen = Scenario.STARVING_MULT_CREATURE_FOOD;
                }
                else if (foodCount == 1)
                {
                    scen = Scenario.MULT_CREATURE_FOOD;
                }
                else if (foodCount > 1)
                {
                    scen = Scenario.MULT_CREATURE_MULT_FOOD;
                }
                else
                {
                    scen = Scenario.MULT_CREATURE;
                }
                #endregion
            }
            else if (foodCount == 1)
            {
                #region single food
                
                if (energy < Simulation.getStarvingEnergyLevel()) //check if the scenario is for starving or not starving
                {
                    scen = Scenario.STARVING_FOOD; //set scenario
                }
                else //if not starving
                {
                    scen = Scenario.FOOD;
                }
                #endregion
            }
            else if (foodCount > 0)
            {
                #region multi food
                scen = Scenario.MULT_FOOD;
                #endregion
            }
            else
            {
                scen = Scenario.NOTHING;
            }
            #endregion
            #region 5. Decide on response and take action
            resp = behaviour[scen];
            switch (resp)
            {
                    //creatures + food
                case Response.IGNORE_FOOD_NON_PREFERRED: ignoreNonPrefFood(pl, rl, cl); break;
                case Response.IGNORE_FOOD: ignoreFood(cl); break;
                case Response.IGNORE_CREATURE: ignoreCreature(pl, rl); break;
                    //multiple creatures
                case Response.FOCUS_WEAKEST: creatureAct(getWeakest(cl)); break;
                case Response.FOCUS_SLOWEST: creatureAct(getSlowest(cl)); break;
                case Response.FOCUS_WOUNDED: creatureAct(getWounded(cl)); break;
                case Response.FOCUS_LEAST_DEFENDED: creatureAct(getLeastDefended(cl)); break;
                case Response.FOCUS_CLOSEST: creatureAct((Creature)getClosest(cl.ToList<WorldObject>())); break;
                case Response.FOCUS_MOST_HUNGRY: creatureAct(getMostHungry(cl)); break;
                    //single food
                case Response.EAT:
                    if (plantCount > 0)
                    {
                        eat(pl[0]);
                    }
                    else
                    {
                        eat(rl[0]);
                    }
                    break;
                case Response.EAT_PREFERRED:
                    if (plantCount > 0)
                    {
                        eatPreferred(pl[0]);
                    }
                    else
                    {
                        eatPreferred(rl[0]);
                    }
                    break;
                    //multiple food
                case Response.EAT_MOST_NOURISHING: eat(getMostNourishing(fl)); break;
                case Response.EAT_MOST_REMAINING: eat(getMostRemaining(fl)); break;
                case Response.EAT_LEAST_DANGEROUS: eat(getLeastDangerous(fl, cl)); break;
                case Response.EAT_CLOSEST_PREFERRED: eat(getClosestPreferred(fl)); break;
                case Response.EAT_LEAST_REMAINING: eat(getLeastRemaining(fl)); break;
                case Response.EAT_MOST_EFFICIENT: eat(getMostEfficient(fl)); break;
                case Response.EAT_CLOSEST: eat((FoodSource)getClosest(fl.ToList<WorldObject>())); break;
                    //single creature
                case Response.ATTACK: 
                    if(inCombat)
                    {
                        attack(inCombatWith);
                    }
                    else
                    {
                        attack(cl[0]);   
                    }
                    break;
                case Response.IGNORE: randomAction(); break;
                case Response.HIDE: hide(); break;
                case Response.AMBUSH: ambush(cl[0]); break;
                case Response.DEFEND: break; //NOT DONE
                case Response.EVADE: evade(cl.ToList<WorldObject>()); break;
                case Response.STALK: stalk(cl[0]); break;
                    //no scenario
                case Response.RANDOM: randomAction(); break;
            }
            #endregion
        }

        
        

        /// <summary>
        /// A method used to check if you can detect a creature that is trying to be stealthy
        /// </summary>
        /// <param name="c">The creature in the scan radius that we are checking we can see</param>
        /// <returns>True if the creature is not hidden, false if the creature is hidden</returns>
        private bool canSee(Creature c)
        {
            bool b = true;
            double i = getEuclideanDistanceFrom(c);
            if (i > awareness)
            {
                b = false;
            }
            else if(random.Next(100) < c.getStealthVal() + (i - awareness))
            {
                b = false;
            }
            return b;
        }

        #region sorting methods
        /// <summary>
        /// Gets the closest WorldObject in a given list
        /// </summary>
        /// <param name="stuff">The list of WorldObjects to search through</param>
        /// <returns>The closest WorldObject in the list</returns>
        private WorldObject getClosest(List<WorldObject> stuff)
        {
            int dist = int.MaxValue;
            WorldObject closest = null;

            foreach(WorldObject w in stuff)
            {
                int d = getDistanceFrom(w);
                if(d < dist)
                {
                    dist = d;
                    closest = w;
                }
            }
            return closest;
        }

        /// <summary>
        /// Gets the furthest WorldObject in a given list
        /// </summary>
        /// <param name="stuff">The list of WorldObjects to search through</param>
        /// <returns>The furthest WorldObject in the list</returns>
        private WorldObject getFurthest(List<WorldObject> stuff)
        {
            int dist = 0;
            WorldObject furthest = null;

            foreach(WorldObject w in stuff)
            {
                int d = getDistanceFrom(w);
                if(d > dist)
                {
                    dist = d;
                    furthest = w;
                }
            }
            return furthest;
        }

        /// <summary>
        /// Gets the weakest (lowest strength) creature in a given list
        /// </summary>
        /// <param name="creatures">The list of Creatures to search through</param>
        /// <returns>The weakest Creature in the list</returns>
        private Creature getWeakest(List<Creature> creatures)
        {
            int lowestStr = int.MaxValue;
            Creature weakest = null;
            foreach (Creature c in creatures)
            {
                int str = c.getStrength();
                if (str < lowestStr)
                {
                    lowestStr = str;
                    weakest = c;
                }
            }
            return weakest;
        }

        /// <summary>
        /// Gets the slowest (lowest speed) creature in a given list
        /// </summary>
        /// <param name="creatures">The list of Creatures to search through</param>
        /// <returns>The slowest Creature in the list</returns>
        private Creature getSlowest(List<Creature> creatures)
        {
            int lowestSpeed = int.MaxValue;
            Creature slowest = null;
            foreach (Creature c in creatures)
            {
                int spd = c.getSpeed();
                if (spd < lowestSpeed)
                {
                    lowestSpeed = spd;
                    slowest = c;
                }
            }
            return slowest;
        }

        /// <summary>
        /// Gets the most wounded (lowest health) creature in a given list
        /// </summary>
        /// <param name="creatures">The list of Creatures to search through</param>
        /// <returns>The lowest health Creature in the list</returns>
        private Creature getWounded(List<Creature> creatures)
        {
            int lowestHealth = int.MaxValue;
            Creature wounded = null;
            foreach (Creature c in creatures)
            {
                int hth = c.getHealth();
                if (hth < lowestHealth)
                {
                    lowestHealth = hth;
                    wounded = c;
                }
            }
            return wounded;
        }

        /// <summary>
        /// Gets the least defended creature in a given list
        /// </summary>
        /// <param name="creatures">The list of Creatures to search through</param>
        /// <returns>The least defended Creature in the list</returns>
        private Creature getLeastDefended(List<Creature> creatures)
        {
            int lowestDef = int.MaxValue;
            Creature leastDefended = null;
            foreach (Creature c in creatures)
            {
                int def = c.getDefence();
                if (def < lowestDef)
                {
                    lowestDef = def;
                    leastDefended = c;
                }
            }
            return leastDefended;
        }

        /// <summary>
        /// Gets the hungriest (lowest energy) creature in a given list
        /// </summary>
        /// <param name="creatures">The list of Creatures to search through</param>
        /// <returns>The hungriest Creature in the list</returns>
        private Creature getMostHungry(List<Creature> creatures)
        {
            int lowestEnergy = int.MaxValue;
            Creature hungriest = null;
            foreach (Creature c in creatures)
            {
                int eng = c.getEnergy();
                if (eng < lowestEnergy)
                {
                    lowestEnergy = eng;
                    hungriest = c;
                }
            }
            return hungriest;
        }

        /// <summary>
        /// Gets the most nourishing piece of food in a given list
        /// </summary>
        /// <param name="food">The list of FoodSources to search through</param>
        /// <returns>The most nourishing FoodSource in the list</returns>
        private FoodSource getMostNourishing(List<FoodSource> food)
        {
            int mostNourishment = 0;
            FoodSource ret = null;
            foreach (FoodSource f in food)
            {
                int n = getNourishmentAmt(f);
                if (n > mostNourishment)
                {
                    mostNourishment = n;
                    ret = f;
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets how nourishing a given FoodSource is, by applying the multiplication that will
        /// be applied to the food based on diet when it is eaten
        /// </summary>
        /// <param name="f">The foodsource to look</param>
        /// <returns>The energy the creature would get if it ate the foodsource</returns>
        private int getNourishmentAmt(FoodSource f)
        {
            double ret = 0;
            if (f.isPlant())
            {
                double inverseDiet = 1 - diet;
                ret = f.getFoodValue() * inverseDiet;
            }
            else
            {
                ret = f.getFoodValue() * diet;
            }
            return (int)ret;
        }

#if DEBUG
        /// <summary>
        /// A public way to access getMostNourishing, used in debugging and testing
        /// </summary>
        public FoodSource pubGetMostNourishing(List<FoodSource> f)
        {
            return getMostNourishing(f);
        }

        /// <summary>
        /// A public way to access getNourishmentAmt, used in debugging and testing
        /// </summary>
        public int pubGetNourishmentAmt(FoodSource f)
        {
            return getNourishmentAmt(f);
        }
#endif

        /// <summary>
        /// Gets the FoodSource with the most remaining in a given list
        /// </summary>
        /// <param name="food">The list of FoodSources to search through</param>
        /// <returns>The FoodSource with the most remaining in the list</returns>
        private FoodSource getMostRemaining(List<FoodSource> food)
        {
            int mostRemaining = -1;
            FoodSource retFood = null;
            foreach (FoodSource f in food)
            {
                if (f.getFoodRemaining() > mostRemaining)
                {
                    mostRemaining = f.getFoodRemaining();
                    retFood = f;
                }
            }
            return retFood;
        }

        /// <summary>
        /// Gets the least dangerous FoodSource in a given list, based on a given list of creatures
        /// </summary>
        /// <param name="food">The list of FoodSources to search through</param>
        /// <param name="creatures">The list of creatures to use to determine danger</param>
        /// <returns>The least dangerous FoodSource, which is the one furthest from all the creatures in the given list of creatures</returns>
        private FoodSource getLeastDangerous(List<FoodSource> food, List<Creature> creatures)
        {
            int leastDanger = int.MaxValue;
            FoodSource retFood = null;
            foreach (FoodSource f in food)
            {
                int danger = getDanger(f, creatures.ToList<WorldObject>());
                if (danger < leastDanger)
                {
                    leastDanger = danger;
                    retFood = f;
                }
            }
            return retFood;
        }

        /// <summary>
        /// Gets the danger of a given WorldObject based on a list of other WorldObjects
        /// </summary>
        /// <param name="o">The WorldObject to get the danger for</param>
        /// <param name="things">The list of WorldObjects to use to find the danger of the other</param>
        /// <returns>An int value representing the danger of the given object, a lower value being more dangerous</returns>
        private int getDanger(WorldObject o, List<WorldObject> things)
        {
            int danger = 0;
            foreach (WorldObject b in things)
            {
                danger += o.getDistanceFrom(b);
            }
            return danger;
        }

        /// <summary>
        /// Gets the closest FoodSource that the creature prefers from a given list
        /// </summary>
        /// <param name="food">The list of FoodSources to look through</param>
        /// <returns>The closest preferred FoodSource</returns>
        private FoodSource getClosestPreferred(List<FoodSource> food)
        {
            int leastDist = int.MaxValue;
            FoodSource retFood = null;
            List<FoodSource> removeFood = new List<FoodSource>();
            foreach (FoodSource f in food)
            {
                if (foodIsPreferred(f))
                {
                    int d = getDistanceFrom(f);
                    if (d < leastDist)
                    {
                        leastDist = d;
                        retFood = f;
                    }
                }
            }
            return retFood;
        }

        /// <summary>
        /// Gets the FoodSource with the least remaining food units based on a given list
        /// </summary>
        /// <param name="food">The list of FoodSources to look through</param>
        /// <returns>The FoodSource with the least remaining food units in the list</returns>
        private FoodSource getLeastRemaining(List<FoodSource> food)
        {
            int leastRemaining = int.MaxValue;
            FoodSource retFood = null;
            foreach (FoodSource f in food)
            {
                if (f.getFoodRemaining() < leastRemaining)
                {
                    leastRemaining = f.getFoodRemaining();
                    retFood = f;
                }
            }
            return retFood;
        }

        /// <summary>
        /// Gets the most efficient FoodSource based on distance and how nourishing it will be for the creature
        /// </summary>
        /// <param name="food">The list of FoodSources to look through</param>
        /// <returns>The most efficient FoodSource in the list, the nourishment amount divided by the distance</returns>
        private FoodSource getMostEfficient(List<FoodSource> food)
        {
            double efficiency = 0.0;
            FoodSource retFood = null;
            foreach (FoodSource f in food)
            {
                double fEfficiency = getNourishmentAmt(f) / getDistanceFrom(f);
                if (fEfficiency > efficiency)
                {
                    efficiency = fEfficiency;
                    retFood = f;
                }
            }
            return retFood;

        }

        #endregion

        #region stat manipulating methods
        /// <summary>
        /// Damages the creature by a given value, killing it if its health drops below 0
        /// </summary>
        /// <param name="damage">The amount of damage to do to the creature</param>
        public void damage(int damage)
        {
            //don't reduce based on defence here, since this method is used for when the creature runs out of energy as well
            health -= damage;
            if (health <= 0)
            {
                this.die();
            }
        }

        /// <summary>
        /// Deals with the necessary steps to kill the creature
        /// </summary>
        public void die()
        {
            world.killCreature(this);
        }

        /// <summary>
        /// Checks if the creature can act based on energy cost
        /// </summary>
        /// <param name="energyCost">The energy cost to check the action energy cost against</param>
        /// <returns>A bool representing if the creature can act or not</returns>
        public bool canAct(int energyCost)
        {
            return energyCost <= stamina && energyCost <= energy;
        }

        /// <summary>
        /// Drains energy and stamina by a given value
        /// </summary>
        /// <param name="energyAmt">The amount to drain energy and stamina by</param>
        public void drainEnergy(int energyAmt)
        {
            drainEnergy(energyAmt, true);
        }
        
        /// <summary>
        /// Drains energy by a given amount and drains stamina or not based on a bool parameter
        /// </summary>
        /// <param name="energyAmt">The energy amount to drain the energy and stamina by</param>
        /// <param name="drainStam">Whether or not to drain stamina as well as energy, drain the stamina if true, not if false</param>
        public void drainEnergy(int energyAmt, bool drainStam)
        {
            if (energy >= energyAmt) //if have more energy than will be drained
            {
                energy -= energyAmt;
            }
            else //otherwise
            {
                energyAmt -= energy;
                energy = 0;
                damage(energyAmt * 2);
            }
            if (drainStam)
            {
                if (stamina > energyAmt)
                {
                    stamina -= energyAmt;
                }
                else
                {
                    stamina = 0;
                }
            }
        }

        #endregion

        #region action methods
        /// <summary>
        /// Checks if a piece of food is of the type the creature prefers
        /// </summary>
        /// <param name="f">The piece of food to check</param>
        /// <returns>If the piece of food is of the type the creature prefer</returns>
        private bool foodIsPreferred(FoodSource f)
        {
            bool b = false;
            if (f.isPlant() && diet * 2 <= 1)
            {
                b = true;
            }
            else if (!f.isPlant() && diet * 2 >= 1) //Both use >= / <= since omnivores shouldn't really have any preference
            {
                b = true;
            }
            else
            {
                b = false;
            }
            return b;
        }

        /// <summary>
        /// Tells the creature it is being attacked and updates the variables related to that
        /// </summary>
        /// <param name="c"></param>
        public void attacked(Creature c)
        {
            inCombat = true;
            inCombatWith = c;
        }

        /// <summary>
        /// Updates the flags to tell the creature it is not in combat
        /// </summary>
        public void outOfCombat()
        {
            inCombat = false;
            inCombatWith = null;
        }

        /// <summary>
        /// Eats a piece of food and adjusts stamina, energy and the food itself as needed
        /// </summary>
        /// <param name="f">The foodsource to eat from</param>
        public void eat(FoodSource f)
        {
            if (f == null)
            {
                randomAction();
            }
            else
            {
                if (isAdjacent(f))
                {
                    if (stamina == maxStamina)
                    {
                        f.beEaten();
                        energy += getNourishmentAmt(f);
                        float ts = (float)stamina; //convert to float here to avoid minimise rounding errors
                        ts /= (100 / 50); //div 50 could be settable by Simulation (100 / x) where x == %age of stamina to drain
                        stamina = (int)ts;
                    }
                    else
                    {
                        //wait for stamina to increase
                    }
                }
                else
                {
                    moveTowards(f);
                }
            }
        }

        /// <summary>
        /// Eats a piece of food only if it is of the type the creature prefers
        /// </summary>
        /// <param name="f"></param>
        private void eatPreferred(FoodSource f)
        {
            if (foodIsPreferred(f))
            {
                eat(f);
            }
            else
            {
                randomAction();
            }
        }

        /// <summary>
        /// Attacks another creature, comparing strength to defence and moving towards the other creature if it is not adjacent
        /// </summary>
        /// <param name="otherCreature">The creature to attack</param>
        public void attack(Creature otherCreature)
        {
            if (isAdjacent(otherCreature))
            {
                int winnerEnergy = 10;
                int loserEnergy = 20;

                otherCreature.attacked(this);
                inCombat = true;
                inCombatWith = otherCreature;
                int attackVal = this.getStrength() + rand(10);
                int defVal = otherCreature.getDefence() + rand(10);

                if (attackVal > defVal)
                {
                    otherCreature.damage(attackVal - defVal);
                    this.drainEnergy(winnerEnergy);
                    otherCreature.drainEnergy(loserEnergy);
                }
                else if (attackVal < defVal)
                {
                    this.damage((defVal - attackVal) / 2);
                    this.drainEnergy(loserEnergy);
                    otherCreature.drainEnergy(winnerEnergy);
                }
                else
                {
                    this.drainEnergy(loserEnergy);
                    otherCreature.drainEnergy(loserEnergy);
                }
            }
            else
            {
                moveTowards(otherCreature);
            }
        }

        /// <summary>
        /// Generates an 'exploding' random number with x as the high value for each roll
        /// </summary>
        /// <param name="x">The max value of each random roll and the value that must be gotten to reroll</param>
        /// <returns>A value which consists of a number of randomly generated numbers where each x triggers another number to be generated and added to the total</returns>
        private int rand(int x)
        {
            int returnVal = 0;
            int randVal;
            do
            {
                randVal = random.Next(x) + 1;
                returnVal += randVal;
            }
            while (randVal == x);
            return returnVal;
        }

        /// <summary>
        /// If a creature hides it is considered stealthy until it is spotted
        /// </summary>
        public void hide()
        {
            stealth = true;
        }

        /// <summary>
        /// If a creature is spotted it is no longer considered hidden from any other creature
        /// </summary>
        public void spotted()
        {
            stealth = false;
        }

        /// <summary>
        /// Controls ambush behaviour, where a creature tries to sneak up next to another to get a high damage special attack
        /// </summary>
        /// <param name="c"></param>
        public void ambush(Creature c)
        {
            if (isStealthy())
            {
                if (!c.canSee(this))
                {
                    if (isAdjacent(c))
                    {
                        c.damage((getStrength() / 2) + getStealthVal());
                        spotted();
                    }
                    else
                    {
                        moveTowards(c);
                    }
                }
                else
                {
                    spotted();
                }
            }
            else
            {
                hide();
            }
        }

        /// <summary>
        /// Moves the creature in a given direction if that is possible and drains energy as needed
        /// </summary>
        /// <param name="dir">The direction to move in</param>
        private void move(Direction dir)
        {
            int energyDiv = speed / 2;
            if (energyDiv < 1)
            {
                energyDiv = 1;
            }
            int energyCost = 20 / energyDiv; //speed + 1 is used to avoid divide by 0 errors.
            if (isStealthy())
            {
                energyCost *= 2;
            }
            int[] newLoc = getLocationFromDirection(dir);
            if (world.tileIsClear(newLoc[1], newLoc[0]))
            {
                if(canAct(energyCost)) //if it can afford the energy cost
                {
                    world.clearTile(getLocationXY()[0], getLocationXY()[1]);
                    this.setLocation(newLoc[0], newLoc[1]); //update location
                    world.addCreature(newLoc[0], newLoc[1], this);
                    this.drainEnergy(energyCost); //drain energy 
                }
            }
            else if (world.creatureAt(getLocationXY()[1], getLocationXY()[0]))
            {
                world.getCreatureAt(getLocationXY()[1], getLocationXY()[0]).spotted();
            }
            else
            {
                if (!surrounded())
                {
                    if (random.Next(2) > 0)
                    {
                        move(dir.right());
                    }
                    else
                    {
                        move(dir.left());
                    }
                }
            }
        }

        /// <summary>
        /// Moves towards a given world object
        /// </summary>
        /// <param name="thing">The thing to move towards</param>
        public void moveTowards(WorldObject thing)
        {
            if (inCombat)
            {
                if (getSpeed() + rand(10) < inCombatWith.getSpeed() + rand(10))
                {
                    damage(inCombatWith.getStrength() - getDefence());
                }
                outOfCombat();
            }
            int i = 0;
            int maxMoves = speed;
            while(i < maxMoves && !isAdjacent(thing))
            {
                Direction d = getDirectionTo(thing);
                move(d);
                i++;
            }
        }

        /// <summary>
        /// Moves away from a given world object
        /// </summary>
        /// <param name="thing">The world object to avoid</param>
        public void moveAwayFrom(WorldObject thing)
        {
            if (inCombat)
            {
                if (getSpeed() + rand(10) < inCombatWith.getSpeed() + rand(10))
                {
                    damage(inCombatWith.getStrength() - getDefence());
                }
                outOfCombat();
            } 
            int i = 0;
            int maxMoves = speed;
            while (i < maxMoves && !isAdjacent(thing))
            {
                Direction d = getDirectionTo(thing);
                d = d.opposite();
                move(d);
                i++;
            }
        }

        /// <summary>
        /// Evades a list of WorldObjects, trying to move in a direction that places it as far away from all of them as possible
        /// </summary>
        /// <param name="things">The list of WorldObjects to try and avoid</param>
        public void evade(List<WorldObject> things)
        {
            int[] realLoc = getLocationXY();
            Direction[] d = new Direction[] { Direction.NORTH, Direction.NORTHEAST, Direction.NORTHWEST, Direction.SOUTH, Direction.SOUTHEAST, Direction.SOUTHWEST, Direction.WEST, Direction.EAST };
            int bestSoFar = int.MaxValue;
            Direction bestDirSoFar = Direction.NORTH;
            foreach (Direction dir in d)
            {
                int[] newLoc = getLocationFromDirection(dir);
                if (world.tileIsClear(newLoc[1], newLoc[0]))
                {
                    this.setLocation(newLoc[0], newLoc[1]);
                    int danger = getDanger(this, things);
                    if (danger < bestSoFar)
                    {
                        bestSoFar = danger;
                        bestDirSoFar = dir;
                    }
                }
            }
            setLocation(realLoc[0], realLoc[1]);
            move(bestDirSoFar);
        }
        
        /// <summary>
        /// Moves in a random direction
        /// </summary>
        private void randomAction()
        {
            Direction dir = Direction.NORTH;
            dir = dir.randomDirection();
            move(dir);
        }

        /// <summary>
        /// A routing method to decide on the creature's action, using a provided creature
        /// </summary>
        /// <param name="c">The creature to act upon</param>
        private void creatureAct(Creature c)
        {
            switch (behaviour[Scenario.CREATURE])
            {
                case Response.ATTACK: attack(c); break;
                case Response.EVADE: moveAwayFrom(c); break;
                case Response.IGNORE: randomAction(); break;
                case Response.HIDE: hide(); break;
                case Response.DEFEND: break;
                case Response.STALK: stalk(c); break;
                case Response.AMBUSH: ambush(c); break;
            }
        }

        /// <summary>
        /// Routes to the correct method call if the creature decides it needs to ignore any food in sight
        /// </summary>
        /// <param name="cl">The list of creatures gotten from scanning the surroundings</param>
        private void ignoreFood(List<Creature> cl)
        {
            if (cl.Count > 1)
            {
                switch (behaviour[Scenario.MULT_CREATURE])
                {
                    case Response.FOCUS_WEAKEST: creatureAct(getWeakest(cl)); break;
                    case Response.FOCUS_SLOWEST: creatureAct(getSlowest(cl)); break;
                    case Response.FOCUS_WOUNDED: creatureAct(getWounded(cl)); break;
                    case Response.FOCUS_LEAST_DEFENDED: creatureAct(getLeastDefended(cl)); break;
                    case Response.FOCUS_CLOSEST: creatureAct((Creature)getClosest(cl.ToList<WorldObject>())); break;
                    case Response.FOCUS_MOST_HUNGRY: creatureAct(getMostHungry(cl)); break;
                }
            }
            else
            {
                creatureAct(cl[0]);
            }
        }

        /// <summary>
        /// Gets a location in the world based on a given direction
        /// </summary>
        /// <param name="d">The direction to look in</param>
        /// <returns>The location in the world 1 tile away from the location of this creature in a given direction</returns>
        private int[] getLocationFromDirection(Direction d)
        {
            int[] newLoc = null;
            switch (d)
            {
                case Direction.SOUTH: newLoc = new int[] { getLocationXY()[0], getLocationXY()[1] + 1 };
                    break;
                case Direction.SOUTHEAST: newLoc = new int[] { getLocationXY()[0] + 1, getLocationXY()[1] + 1 };
                    break;
                case Direction.EAST: newLoc = new int[] { getLocationXY()[0] + 1, getLocationXY()[1] };
                    break;
                case Direction.NORTHEAST: newLoc = new int[] { getLocationXY()[0] + 1, getLocationXY()[1] - 1 };
                    break;
                case Direction.NORTH: newLoc = new int[] { getLocationXY()[0], getLocationXY()[1] - 1 };
                    break;
                case Direction.NORTHWEST: newLoc = new int[] { getLocationXY()[0] - 1, getLocationXY()[1] - 1 };
                    break;
                case Direction.WEST: newLoc = new int[] { getLocationXY()[0], getLocationXY()[1] - 1 };
                    break;
                case Direction.SOUTHWEST: newLoc = new int[] { getLocationXY()[0] - 1, getLocationXY()[1] + 1 };
                    break;
            }
            return newLoc;
        }

        /// <summary>
        /// Stalks another creature, trying to stay outside of its view radius, with the ideal position being just outside the view radius
        /// </summary>
        /// <param name="creature">The creature to stalk</param>
        private void stalk(Creature creature)
        {
            if (creature.getAwareness() < this.awareness)
            {
                if (getDistanceFrom(creature) > creature.getAwareness() + 1)
                {
                    moveTowards(creature);
                }
                else if (getDistanceFrom(creature) == creature.getAwareness() + 1)
                {
                    //stay still
                }
                else
                {
                    moveAwayFrom(creature);
                }
            }
            else
            {
                ambush(creature);
            }
        }

        /// <summary>
        /// Called for the Response that ignores creatures
        /// </summary>
        /// <param name="pl">The list of plants to look at</param>
        /// <param name="rl">The list of remains to look at</param>
        private void ignoreCreature(List<Plant> pl, List<Remains> rl)
        {
            Scenario s = Scenario.MULT_FOOD;
            if (energy < Simulation.getStarvingEnergyLevel())
            {
                s = Scenario.STARVING_FOOD;
            }
            else if (pl.Count + rl.Count > 1)
            {
                s = Scenario.MULT_FOOD;
            }
            else
            {
                s = Scenario.FOOD;
            }
            List<FoodSource> fl = pl.ToList<FoodSource>();
            fl.AddRange(rl.ToList<FoodSource>());

            switch (behaviour[s])
            {
                case Response.EAT: eat((FoodSource)getClosest(fl.ToList<WorldObject>())); break;
                case Response.EAT_PREFERRED: eatPreferred((FoodSource)getClosest(fl.ToList<WorldObject>())); break;
                case Response.EAT_CLOSEST: eat((FoodSource)getClosest(fl.ToList<WorldObject>())); break;
                case Response.EAT_CLOSEST_PREFERRED: eat(getClosestPreferred(fl)); break;
                case Response.EAT_LEAST_DANGEROUS: eat(getLeastDangerous(fl, new List<Creature>())); break;
                case Response.EAT_LEAST_REMAINING: eat(getLeastRemaining(fl)); break;
                case Response.EAT_MOST_EFFICIENT: eat(getMostEfficient(fl)); break;
                case Response.EAT_MOST_NOURISHING: eat(getMostNourishing(fl)); break;
                case Response.EAT_MOST_REMAINING: eat(getMostRemaining(fl)); break;
            }
        }

        /// <summary>
        /// Routing method for handling ignoring non preferred food, if no preferred food exists, treats it as an ignore food response, if preferred food exists treats it as an
        /// ignore creature response without the ignored food being passed on for consideration
        /// </summary>
        /// <param name="pl">The list of visible plants</param>
        /// <param name="rl">The list of visible remains</param>
        /// <param name="cl">The list of visible creatures</param>
        private void ignoreNonPrefFood(List<Plant> pl, List<Remains> rl, List<Creature> cl)
        {
            if (diet <= 0.5)
            {
                if (pl.Count == 0)
                {
                    ignoreFood(cl);
                }
                else if (pl.Count == 1)
                {
                    eat(pl[0]);
                }
                else
                {
                    ignoreCreature(pl, new List<Remains>());
                }
            }
            else if (diet > 0.5)
            {
                if (rl.Count == 0)
                {
                    ignoreFood(cl);
                }
                else if (rl.Count == 1)
                {
                    eat(rl[0]);
                }
                else
                {
                    ignoreCreature(new List<Plant>(), rl);
                }
            }
        }

        /// <summary>
        /// Used in move() to avoid infinite loops, the idea being that the creature tries other moves until it finds one it can make if it is blocked, the problem being that if it is blocked
        /// on all sides it will trap the program
        /// </summary>
        /// <returns>A bool representing if the creature is surrounded on all sides by impassible objects</returns>
        private bool surrounded()
        {
            bool isSurrounded = true;
            Direction[] d = new Direction[]{Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST, Direction.NORTHEAST, Direction.NORTHWEST, Direction.SOUTHEAST, Direction.SOUTHWEST};
            int i = 0;
            while (i < d.Length && isSurrounded)
            {
                int[] loc = getLocationFromDirection(d[i]);
                if(world.tileIsClear(loc[1], loc[0]))
                {
                    isSurrounded = false;
                }
                i++;
            }
            return isSurrounded;
        }

        #region user manual controls

        /// <summary>
        /// Clones this creature and adds it at a random location in the world
        /// </summary>
        /// <returns>The creature that has just been created</returns>
        public Creature userCloneCreature()
        {
            Creature c = new Creature(this.getDna());
            c.setWorld(this.world);
            world.addCreature(c);
            return c;
        }

        /// <summary>
        /// Kills this creature
        /// </summary>
        public void userKillCreature()
        {
            damage(this.getHealth());
        }

        #endregion
        #endregion
        #endregion

#if DEBUG
        public Dictionary<Scenario, Response> getBehaviour()
        {
            return behaviour;
        }
#endif
    }
}
