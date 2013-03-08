using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Genome
{
    class Creature : WorldObject
    {
        private Gene dna;
        private WorldState world;

        private bool inCombat;
        private bool stealth;
        private Creature inCombatWith;
        Random random;


        #region stats
        private int health;
        private int maxHealth;
        private int energy;
        private int initEnergy;

        private int strength;
        private int speed;
        private int awareness;
        private int defence;
        private double diet;
        private int[] colour;
        private int stealthVal;

        private int stamina;
        private int maxStamina;

        private Dictionary<Scenario, Response> behaviour;

        #endregion

        public Creature()
        {
            dna = new Gene();
            init(WorldState.RandomNumberGenerator);
        }

        public Creature(Gene dna)
        {
            this.dna = dna;
            init(WorldState.RandomNumberGenerator);
        }

        private void init(Random r)
        {
            this.random = r;
            colour = new int[3];
            setLocation(-1, -1);

            inCombat = false;
            stealth = false;

            //strength, speed, awareness, defence, stealthval
            //DEFAULT VALUES INITIALISATION

            awareness = 25; //TODO: Make these modifiable (if time permits)
            initEnergy = 100;
            strength = 50;
            speed = 50;
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
            awareness /= 5;

            health = maxHealth;
            energy = initEnergy;
            maxStamina = energy / 10;
            stamina = maxStamina;

            decideBehaviours();
        }

        private void decideBehaviours()
        {
            behaviour = new Dictionary<Scenario, Response>();
            behaviour.Add(Scenario.NOTHING, Response.RANDOM);
            Scenario[] scenarios = Scenario.NOTHING.AllScenarios();
            Response[] responses = new Response[scenarios.Length];
            int scenariosChecked = 0;
            int i;
            int j;
            for (i = 1; i < dna.getSizeX() - 1; i += 2)
            {
                for (j = 1; j < dna.getSizeY() - 1; j += 2)
                {
                    if (scenariosChecked < responses.Length)
                    {
                        responses[scenariosChecked] = scenarios[scenariosChecked].PossibleResponses()[dna.poll(i, j)];
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

        }

        #region methods
        #region accessor methods
        public Gene getDna()
        {
            return dna;
        }

        public int getDefence()
        {
            return defence;
        }

        public int getHealth()
        {
            return health;
        }

        public int getStealthVal()
        {
            return stealthVal;
        }

        public int getStrength()
        {
            return strength;
        }

        public bool isStealthy()
        {
            return stealth;
        }

        public int getSpeed()
        {
            return speed;
        }

        public int getAwareness()
        {
            return awareness;
        }

        public int getEnergy()
        {
            return energy;
        }

        public int getStatValue()
        {
            return speed + awareness + defence + maxHealth + initEnergy + stealthVal + strength;
        }

        public void setWorld(WorldState world)
        {
            this.world = world;
        }

        #endregion

        public void tick()
        {
            #region 1. Rejuvenate
            if (health < maxHealth - ((maxHealth / 100) * Simulation.getHealthRegenSpeed()))
            {
                health += (int)(((double)maxHealth / 100.0) * (double)Simulation.getHealthRegenSpeed());
            }
            else if(health < maxHealth)
            {
                health = maxHealth;
            }

            if (stamina < maxStamina - ((maxStamina / 100) * Simulation.getStaminaRegenSpeed()))
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
            List<Plant> pl = (List<Plant>)l[1];
            List<Remains> rl = (List<Remains>)l[2];
            List<Obstacle> ol = (List<Obstacle>)l[3];
            List<FoodSource> fl = new List<FoodSource>();
            fl.AddRange(pl.ToArray<FoodSource>());
            fl.AddRange(rl.ToArray<FoodSource>());
            #region dealing with stealth
            foreach (Creature c in cl)
            {
                if (c.isStealthy() && !canSee(c))
                {
                    cl.Remove(c);
                }
                else
                {
                    c.spotted();
                }
            }
            #endregion

            #endregion
            #region 4. Decide what scenario applies
            Scenario s = Scenario.NOTHING;

            int creatureCount = cl.Count;
            int plantCount = pl.Count;
            int remainsCount = rl.Count;
            int foodCount = plantCount + remainsCount;

            if(inCombat)
            {
                #region combat
                if (creatureCount > 1)
                {
                    s = Scenario.IN_COMBAT_CREATURE;
                }
                else if((double)maxHealth/(double)health < (double)Simulation.getWoundedHealthPercent())
                {
                    s = Scenario.IN_COMBAT_WOUNDED;
                }
                else
                {
                    s = Scenario.IN_COMBAT;
                }
                #endregion
            }
            else if (creatureCount == 1)
            {
                #region single creature
                if (foodCount > 0 && energy < Simulation.getStarvingEnergyLevel())
                {
                    s = Scenario.STARVING_CREATURE_FOOD;
                }
                else if (foodCount == 1)
                {
                    s = Scenario.CREATURE_FOOD;
                }
                else if (foodCount > 1)
                {
                    s = Scenario.CREATURE_MULT_FOOD;
                }
                else
                {
                    s = Scenario.CREATURE;
                }
                #endregion
            }
            else if (creatureCount > 1)
            {
                #region multi creature
                if (foodCount > 0 && energy < Simulation.getStarvingEnergyLevel())
                {
                    s = Scenario.STARVING_MULT_CREATURE_FOOD;
                }
                else if (foodCount == 1)
                {
                    s = Scenario.MULT_CREATURE_FOOD;
                }
                else if (foodCount > 1)
                {
                    s = Scenario.MULT_CREATURE_MULT_FOOD;
                }
                else
                {
                    s = Scenario.MULT_CREATURE;
                }
                #endregion
            }
            else if (foodCount == 1)
            {
                #region single food
                
                if (energy < Simulation.getStarvingEnergyLevel()) //check if the scenario is for starving or not starving
                {
                    s = Scenario.STARVING_FOOD; //set scenario
                }
                else //if not starving
                {
                    s = Scenario.FOOD;
                }
                #endregion
            }
            else if (foodCount > 0)
            {
                #region multi food
                s = Scenario.MULT_FOOD;
                #endregion
            }
            else
            {
                s = Scenario.NOTHING;
            }
            #endregion
            #region 5. Decide on response and take action
            Response resp = behaviour[s];
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
#if DEBUG
            Console.WriteLine("Creature At: (" + getLocationXY()[0] + " , " + getLocationXY()[1] + ")");
#endif
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

        private Creature getSlowest(List<Creature> creatures)
        {
            int lowestSpeed = int.MaxValue;
            Creature slowest = null;
            foreach (Creature c in creatures)
            {
                int spd = c.getStrength();
                if (spd < lowestSpeed)
                {
                    lowestSpeed = spd;
                    slowest = c;
                }
            }
            return slowest;
        }

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

        private Creature getMostHungry(List<Creature> creatures)
        {
            int lowestEnergy = int.MaxValue;
            Creature hungriest = null;
            foreach (Creature c in creatures)
            {
                int eng = c.getStrength();
                if (eng < lowestEnergy)
                {
                    lowestEnergy = eng;
                    hungriest = c;
                }
            }
            return hungriest;
        }

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

        private int getNourishmentAmt(FoodSource f)
        {
            int ret = 0;
            if (f.isPlant())
            {
                ret = (int)(f.getFoodValue() * (1 - diet));
            }
            else
            {
                ret = (int)(f.getFoodValue() * diet);
            }
            return ret;
        }

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

        private int getDanger(WorldObject o, List<WorldObject> things)
        {
            int danger = 0;
            foreach (WorldObject b in things)
            {
                danger += o.getDistanceFrom(b);
            }
            return danger;
        }

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

        public void damage(int damage)
        {
            //don't reduce based on defence here, since this method is used for when the creature runs out of energy as well
            health -= damage;
            if (health <= 0)
            {
                this.die();
            }
        }

        public void die()
        {
            world.killCreature(this);
        }

        public bool canAct(int energyCost)
        {
            return energyCost <= stamina;
        }

        public void drainEnergy(int energyAmt)
        {
            drainEnergy(energyAmt, true);
        }

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
            else if (!f.isPlant() && diet * 2 > 1)
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
                        stamina /= (100 / 50); //div 50 could be settable by Simulation 
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
                int winnerEnergy = 10; //TODO: make these editable
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
            int energyCost = 40;
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
                    move(dir.right());
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
            Direction d = getDirectionTo(thing);
            move(d);
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
            Direction d = getDirectionTo(thing);
            move(d.opposite());
        }

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
        
        private void randomAction()
        {
            Direction dir = Direction.NORTH;
            dir = dir.randomDirection();
            move(dir);
        }

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

        public Creature userCloneCreature()
        {
            Creature c = new Creature(this.getDna());
            world.addCreature(c);
            return c;
        }

        public void userKillCreature()
        {
            this.die();
        }

        #endregion

        #endregion
        #endregion


    }
}
