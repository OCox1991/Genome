using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Genome
{
    class Creature
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

        private int xLocation;
        private int yLocation;

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
            xLocation = -1;
            yLocation = -1;

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

        public int[] getLocation()
        {
            return new int[] { xLocation, yLocation };
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
            ArrayList l = world.scan(getLocation(), awareness);
            List<Creature> cl = (List<Creature>)l[0];
            List<Plant> pl = (List<Plant>)l[1];
            List<Remains> rl = (List<Remains>)l[2];
            List<Obstacle> ol = (List<Obstacle>)l[3];

            //decide what scenario applies

            //get the response to the scenario from the behaviours dictionary

            //repeat until some action is taken (since we might get an ignore responses)

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

        public void setLocation(int x, int y)
        {
            xLocation = x;
            yLocation = y;
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

        }

        public void moveTowards(int[] locationXY)
        {

        }
        #endregion

       
    }
}
