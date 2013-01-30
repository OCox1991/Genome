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

        private WorldState world;
        

        #region scenarios

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
            colour = new int[3];
            setLocation(-1, -1);

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
                    responses[scenariosChecked] = scenarios[scenariosChecked].PossibleResponses()[dna.poll(i, j)];
                }
            }
            for (i = 0; i < responses.Length; i++)
            {
                behaviour.Add(scenarios[i], responses[i]);
            }

        }

        #region methods
        #region accessor
        public Gene getDna()
        {
            return dna;
        }

        public int getSpeed()
        {
            return speed;
        }
        #endregion

        public void tick()
        {
            //1. Rejuvenate
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

            //2. Drain energy
            energy -= Simulation.getEnergyDrainPerTick();

            //3. Decide on action, look around
            ArrayList l = world.scan(getLocationXY(), awareness);
            List<Creature> cl = (List<Creature>)l[0];
            List<Plant> pl = (List<Plant>)l[1];
            List<Remains> rl = (List<Remains>)l[2];
            List<Obstacle> ol = (List<Obstacle>)l[3];

            Scenario s = Scenario.NOTHING;

            int creatureCount = cl.Count;
            int plantCount = pl.Count;
            int remainsCount = rl.Count;
            int foodCount = plantCount + remainsCount;
            WorldObject trackingObject = null;

            if(inCombat)
            {
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
            }
            else if (creatureCount == 1)
            {
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
            }
            else if (creatureCount > 1)
            {
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
            }
            else if (foodCount == 1)
            {
                if (energy < Simulation.getStarvingEnergyLevel())
                {
                    s = Scenario.STARVING_FOOD;
                    if (behaviour[s] == Response.EAT_PREFERRED)
                    {
                        //TODO: check if the food is the preferred type, then transition to either NOTHING or EAT
                    }
                }
                else
                {
                    s = Scenario.FOOD;
                    if (behaviour[s] == Response.EAT_PREFERRED)
                    {
                        //see above
                    }
                }
            }
            else if (foodCount > 0)
            {
                s = Scenario.MULT_FOOD;
            }
            else
            {
                s = Scenario.NOTHING;
            }

            //get response based on scenario. Have a loop to keep trying to get an action until one is taken

            //4. Take action, stamina & energy penalties

        }

        public void eat(FoodSource f)
        {
            f.beEaten();
            if (f.isPlant())
            {
                energy += (int)(f.getFoodValue() * (1 - diet));
            }
            else
            {
                energy += (int)(f.getFoodValue() * diet);
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

        public void attack(Creature otherCreature)
        {

        }

        public void move(Direction dir)
        {
            world.clearTile(getLocationXY()[0], getLocationXY()[1]);
            switch (dir)
            {
                case Direction.SOUTH: setLocation(getLocationXY()[0], getLocationXY()[1] + 1); 
                    break;
                case Direction.SOUTHEAST: setLocation(getLocationXY()[0] + 1, getLocationXY()[1] + 1);
                    break;
                case Direction.EAST: setLocation(getLocationXY()[0] + 1, getLocationXY()[1]);
                    break;
                case Direction.NORTHEAST: setLocation(getLocationXY()[0] + 1, getLocationXY()[1] - 1);
                    break;
                case Direction.NORTH: setLocation(getLocationXY()[0], getLocationXY()[1] -1);
                    break;
                case Direction.NORTHWEST: setLocation(getLocationXY()[0] - 1, getLocationXY()[1] - 1);
                    break;
                case Direction.WEST: setLocation(getLocationXY()[0], getLocationXY()[1] - 1);
                    break;
                case Direction.SOUTHWEST: setLocation(getLocationXY()[0] - 1, getLocationXY()[1] + 1);
                    break;
            }
            world.addCreature(getLocationXY()[0], getLocationXY()[1], this);
        }

        public void moveTowards(WorldObject thing)
        {
            Direction d = getDirectionTo(thing);
            move(d);
        }

        public void evade(List<WorldObject> things)
        {

        }
        #endregion

       
    }
}
