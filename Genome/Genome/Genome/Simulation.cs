﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class Simulation : Microsoft.Xna.Framework.Game
    {
        private static ArrayList recognisedShapes = new ArrayList();
        private static int energyDrainPerTick = 5;
        private static int staminaRejuvenationPercent = 5;
        private static int healthRejuvenationPercent = 5;

        private static int roundLengths = 10000;
        private static int currentRoundTime;

        private static int numGenerations = -1;
        private static int currentGeneration = 1;

        private static SimulationState state;

        private static Color[] colourMap = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet }; //Remember this is from 0 to 6 so when drawing take 1 to get the desired colour.
        private WorldState theWorld;
        private Creature selectedCreature;

        public Simulation()
        {
            parseShapes("shapes.txt");
            state = new WorldState();
            currentRoundTime = 0;
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

        public static void tick()
        {
            currentRoundTime++;
            if (currentRoundTime >= roundLengths)
            {
                WorldState w = (WorldState)state;
                state = new JudgingState(w.getLiveCreatures(), w.getDeadCreatures);
            }
        }

        #endregion

        #region overriding methods to make the game work

        protected override void Update(GameTime gameTime)
        {
            state.update(gameTime);
        }

        protected override void Draw()
        {
            state.draw();
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
