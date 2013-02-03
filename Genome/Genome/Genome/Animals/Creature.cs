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


        #region stats
        private int health;
        private int maxHealth;
        private int energy;
        private int initEnergy;

        private int strength;
        private int speed;
        private int awareness;
        private int defence;
        private Decimal diet;
        private int[] colour;
        private int stealthVal;

        private int stamina;
        private int maxStamina;

        private Dictionary<Scenario, Response> behaviour;

        #endregion

        public Creature()
        {
            dna = new Gene();
            init();
        }

        public Creature(Gene dna)
        {
            this.dna = dna;
            init();
        }

        private void init()
        {
            world = Simulation.getCurrentWorld();
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
            Response[] responses = new Response[15];
            int scenariosChecked = 0;
            int i;
            int j;
            for (i = 1; i < dna.getSizeX(); i += 2)
            {
                for (j = 1; j < dna.getSizeY(); j += 2)
                {
                    responses[scenariosChecked] = scenarios[scenariosChecked].PossibleResponses()[dna.poll(i, j) - 1] ; //- 1 here since colours are 1 to 7 and possible responses are 0 to 6
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
        #endregion

        public void tick()
        {
            #region 1. Rejuvenate
            if (health < maxHealth - ((maxHealth / 100) * Simulation.getHealthRegenSpeed()))
            {
                health += ((maxHealth / 100) * Simulation.getHealthRegenSpeed());
            }
            else if(health < maxHealth)
            {
                health = maxHealth;
            }

            if (stamina < maxStamina - ((maxStamina / 100) * Simulation.getStaminaRegenSpeed()))
            {
                stamina += ((maxStamina / 100) * Simulation.getStaminaRegenSpeed());
            }
            else if (stamina < maxStamina)
            {
                stamina = maxStamina;
            }
            #endregion
            #region 2. Drain energy
            energy -= Simulation.getEnergyDrainPerTick();
            #endregion
            #region 3. Look Around
            ArrayList l = world.scan(getLocationXY(), awareness);
            List<Creature> cl = (List<Creature>)l[0];
            List<Plant> pl = (List<Plant>)l[1];
            List<Remains> rl = (List<Remains>)l[2];
            List<Obstacle> ol = (List<Obstacle>)l[3];
            #endregion
            #region 4. Do Something
            Scenario s = Scenario.NOTHING;

            int creatureCount = cl.Count;
            int plantCount = pl.Count;
            int remainsCount = rl.Count;
            int foodCount = plantCount + remainsCount;
            WorldObject trackingObject = null;
            List<WorldObject> trackingObjects = null;
            Response resp = Response.RANDOM;

            foreach (Creature c in cl)
            {
                if (c.isStealthy())
                {
                    if (!canSee(c))
                    {
                        cl.Remove(c);
                    }
                    else
                    {
                        c.spotted();
                    }
                }
            }

            if(inCombat)
            {
                #region combat
                if (creatureCount > 1)
                {
                    s = Scenario.IN_COMBAT_CREATURE;
                }
                else if(maxHealth/health < Simulation.getWoundedHealthPercent())
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
                resp = behaviour[s];
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
            else if (foodCount == 1) //1 food visible
            {
                #region single food
                //set up tracking object
                if (pl.Count == 0) //if not plant
                {
                    trackingObject = rl[0];
                }
                else //if plant
                {
                    trackingObject = pl[0];
                }
                
                if (energy < Simulation.getStarvingEnergyLevel()) //check if the scenario is for starving or not starving
                {
                    s = Scenario.STARVING_FOOD; //set scenario
                    resp = behaviour[s];

                    if (resp == Response.EAT_PREFERRED) //deal with conditional responses
                    {
                        eatPreferred((FoodSource)trackingObject);
                    }
                    else
                    {
                        eat((FoodSource)trackingObject);
                    }
                }
                else //if not starving
                {
                    s = Scenario.FOOD;
                    resp = behaviour[s];

                    if (resp == Response.EAT_PREFERRED) //deal with conditional responses
                    {
                        eatPreferred((FoodSource)trackingObject);
                    }
                    else
                    {
                        eat((FoodSource)trackingObject);
                    }
                }
                #endregion
                //DONE
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
        }

        /// <summary>
        /// A method used to check if you can detect a creature that is trying to be stealthy
        /// </summary>
        /// <param name="c">The creature in the scan radius that we are checking we can see</param>
        /// <returns>True if the creature is not hidden, false if the creature is hidden</returns>
        private bool canSee(Creature c)
        {
            Random r = new Random();
            bool b = true;
            if(r.Next(100) < c.getStealthVal() + (getEuclideanDistanceFrom(c) - awareness))
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
                int danger = getDanger(f, creatures);
                if (danger < leastDanger)
                {
                    leastDanger = danger;
                    retFood = f;
                }
            }
            return retFood;
        }

        private int getDanger(FoodSource f, List<Creature> creatures)
        {
            int danger = 0;
            foreach (Creature c in creatures)
            {
                danger += f.getDistanceFrom(c);
            }
            return danger;
        }

        private FoodSource getClosestPreferred(List<FoodSource> food)
        {
            int leastDist = int.MaxValue;
            FoodSource retFood = null;
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
                else
                {
                    food.Remove(f);
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

        #region action methods
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

        public void attacked(Creature c)
        {
            inCombat = true;
            inCombatWith = c;
        }

        public void outOfCombat()
        {
            inCombat = false;
            inCombatWith = null;
        }

        public void eat(FoodSource f)
        {
            if (isAdjacent(f))
            {
                f.beEaten();
                energy += getNourishmentAmt(f);
            }
            else
            {
                moveTowards(f);
            }
        }

        public void damage(int damage)
        {
            //some reduction based on defence
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

        private bool eatPreferred(FoodSource f)
        {
            bool b = foodIsPreferred(f);
            if (b)
            {
                eat(f);
            }
            return b;
        }

        public void attack(Creature otherCreature)
        {
            if (isAdjacent(otherCreature))
            {
                otherCreature.attacked(this);
                inCombat = true;
                inCombatWith = otherCreature;
            }
            else
            {
                moveTowards(otherCreature);
            }
        }

        public void hide()
        {
            stealth = true;
        }

        public void spotted()
        {
            stealth = false;
        }

        public void move(Direction dir)
        {
            int[] newLoc = null;
            //add combat checking
            switch (dir)
            {
                case Direction.SOUTH: newLoc = new int[]{getLocationXY()[0], getLocationXY()[1] + 1}; 
                    break;
                case Direction.SOUTHEAST: newLoc = new int[]{getLocationXY()[0] + 1, getLocationXY()[1] + 1};
                    break;
                case Direction.EAST: newLoc = new int[]{getLocationXY()[0] + 1, getLocationXY()[1]};
                    break;
                case Direction.NORTHEAST: newLoc = new int[]{getLocationXY()[0] + 1, getLocationXY()[1] - 1};
                    break;
                case Direction.NORTH: newLoc = new int[]{getLocationXY()[0], getLocationXY()[1] -1};
                    break;
                case Direction.NORTHWEST: newLoc = new int[]{getLocationXY()[0] - 1, getLocationXY()[1] - 1};
                    break;
                case Direction.WEST: newLoc = new int[]{getLocationXY()[0], getLocationXY()[1] - 1};
                    break;
                case Direction.SOUTHWEST: newLoc = new int[]{getLocationXY()[0] - 1, getLocationXY()[1] + 1};
                    break;
            }
            if (world.tileIsClear(newLoc[1], newLoc[0]))
            {
                world.clearTile(getLocationXY()[1], getLocationXY()[0]);
                world.addCreature(getLocationXY()[0], getLocationXY()[1], this);
            }
            else if (world.creatureAt(getLocationXY()[1], getLocationXY()[0]))
            {
                world.getCreatureAt(getLocationXY()[1], getLocationXY()[0]).spotted();
            }
        }

        public void moveTowards(WorldObject thing) //TODO: Add speed stuff for no. of moves
        {
            Direction d = getDirectionTo(thing);
            move(d);
        }

        public void evade(List<WorldObject> things)
        {

        }
        #endregion
        #endregion


    }
}
