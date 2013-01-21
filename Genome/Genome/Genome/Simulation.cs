using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class Simulation
    {
        private static ArrayList recognisedShapes = new ArrayList();
        private static int energyDrainPerTick = 5;
        private static int staminaRejuvenationPercent = 5;
        private static int healthRejuvenationPercent = 5;

        private static SimulationState state;

        private static Color[] colourMap = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet }; //Remember this is from 0 to 6 so when drawing take 1 to get the desired colour.

        public Simulation()
        {
            parseShapes("shapes.txt");
            state = new WorldState();
        }

        #region Methods

        #region Game rules related
        public static ArrayList getShapes()
        {
            return recognisedShapes;
        }

        public static int getHealthRegenSpeed()
        {
            return staminaRejuvenationPercent;
        }

        public void setHealthRegenSpeed(int healthSpeed)
        {
            healthRejuvenationPercent = healthSpeed;
        }

        public static int getStaminaRegenSpeed()
        {
            return healthRejuvenationPercent;
        }

        public void setStaminaRegenSpeed(int stamSpeed)
        {
            staminaRejuvenationPercent = stamSpeed;
        }

        public static int getEnergyDrainPerTick()
        {
            return energyDrainPerTick;
        }

        public void setEnergyDrainPerTick(int newDrainSpeed)
        {
            energyDrainPerTick = newDrainSpeed;
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
