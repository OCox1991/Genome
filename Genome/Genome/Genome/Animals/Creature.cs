using System;
using System.Collections.Generic;
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
        }

        #region methods
        #region accessor
        public Gene getDna()
        {
            return dna;
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

        public int[] getLocation()
        {
            return new int[]{xLocation, yLocation};
        }
        #endregion

       
    }
}
