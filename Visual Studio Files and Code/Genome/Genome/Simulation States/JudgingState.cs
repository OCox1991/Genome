﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// In charge of judging the creatures and breeding the new ones needed for the next round
    /// </summary>
    class JudgingState : SimulationState
    {
        private SingleStringDrawer drawer;

        private bool isJudged;
        private bool isEliminated;
        private bool isBred;
        private int numBred;
        private string status;

        private List<Creature> aliveCreatures;
        private Stack<Creature> deadCreatures;
        private List<Creature> creatureList;
        private List<Creature> childList;
        private List<Creature> top;
        private List<Creature> bottom;

        private int lowerFix;
        private int upperFix;
        private int total;

        /// <summary>
        /// Constructor for judging state, initialises all lists and checks to the correct values
        /// </summary>
        /// <param name="aliveCreatures">The list of alive creatures for rating</param>
        /// <param name="deadCreatures">The list of dead creatures for rating in the form of a stack</param>
        public JudgingState(List<Creature> aliveCreatures, Stack<Creature> deadCreatures)
        {
            drawer = new SingleStringDrawer(this);
            this.aliveCreatures = aliveCreatures;
            this.deadCreatures = deadCreatures;
            isJudged = false;
            isEliminated = false;
            isBred = false;
            numBred = 0;
            top = new List<Creature>();
            bottom = new List<Creature>();
            childList = new List<Creature>();
            status = "Judging Creatures...";
        }

        /// <summary>
        /// Updates the judge, moving one step foward through the judginf process
        /// </summary>
        /// <param name="t">The time since the update method was last called, not used in this method</param>
        public override void update(GameTime t) 
        {
            if (!isJudged) //should run through once
            {
                aliveCreatures.Sort(judgeCreatures);
                isJudged = true;
                status = "Eliminating creatures...";
            }
            else if (!isEliminated) //should run through once
            {
                creatureList = new List<Creature>();
                if (aliveCreatures.Count > 0)
                {
                    creatureList.AddRange(aliveCreatures.ToArray<Creature>());
                }
                //enclose this in an IF statement if you want to prevent dead creatures from being included
                while(deadCreatures.Count > 0)
                {
                    Creature c = deadCreatures.Pop();
                    creatureList.Add(c);
                }
                //upon loop exit creatureList should be a list of creatures sorted by success in the world
                total = creatureList.Count;
                //find indexes for certain percentages of the list
                lowerFix = total / 100 * (100 - Simulation.getElimPercentage()); //index to eliminate all after
                upperFix = total / 100 * Simulation.getTopPercentage(); //index to treat all before as top
                creatureList.RemoveRange(lowerFix, total - lowerFix);

                for (int i = 0; i < upperFix; i++)
                {
                    top.Add(creatureList[i]);
                }

                for (int i = upperFix; i < creatureList.Count; i++)
                {
                    bottom.Add(creatureList[i]);
                }

                creatureList.Clear();
                
                status = "Breeding next generation: " + numBred + "/" + Simulation.getPopulation();
                isEliminated = true;
            }
            else if (!isBred) //should run through x times where x = number of creatures we need
            {
                if (numBred > Simulation.getPopulation())
                {
                    status = "Done!";
                    isBred = true;
                }
                else
                {
                    Random rand = WorldState.RandomNumberGenerator;
                    if (childList.Count < (Simulation.getPopulation() / 100) * Simulation.getTopPercentage())
                    {
                        int c1 = rand.Next(top.Count);
                        int c2 = rand.Next(top.Count);
                        int attempts = 0;
                        while (c1 == c2 && attempts < 10)
                        {
                            c2 = rand.Next(upperFix);
                            attempts++;
                        }
                        Creature creature1 = top[c1];
                        Creature creature2 = top[c2];
                        Creature child = new Creature(creature1.getDna().breedWith(creature2.getDna()));
                        childList.Add(child);

                    }
                    else if (childList.Count < Simulation.getPopulation())
                    {
                        int c1 = rand.Next(bottom.Count);
                        int c2 = rand.Next(bottom.Count);
                        int attempts = 0;
                        while (c1 == c2 && attempts < 10)
                        {
                            c2 = rand.Next(upperFix);
                            attempts++;
                        }
                        Creature creature1 = bottom[c1];
                        Creature creature2 = bottom[c2];
                        Creature child = new Creature(creature1.getDna().breedWith(creature2.getDna()));
                        childList.Add(child);
                    }
                    status = "Breeding next generation: " + numBred + "/" + Simulation.getPopulation();
                    numBred++;
                }
            }
            else
            {
                Simulation.judgingDone(childList);
            }

        }

        /// <summary>
        /// Returns an int based on a comparison between creatures
        /// </summary>
        /// <param name="c1">The first creature to compare</param>
        /// <param name="c2">The second creature to compare</param>
        /// <returns>An int, 1 if the first creature is better and -1 if the second is better</returns>
        public static int judgeCreatures(Creature c1, Creature c2)
        {
            int ret;
            int c1Health = (int)c1.getHealth() * Simulation.getHealthWeight();
            int c1Energy = (int)c1.getEnergy() * Simulation.getEnergyWeight();
            int c2Health = (int)c2.getHealth() * Simulation.getHealthWeight();
            int c2Energy = (int)c2.getEnergy() * Simulation.getEnergyWeight(); ;
            if (c1Health + c1Energy > c2Health + c2Energy)
            {
                ret = -1;
            }
            else if (c1Health + c1Energy == c2Health + c2Energy)
            {
                int c1Stats = c1.getStatValue();
                int c2Stats = c2.getStatValue();
                if (c1Stats > c2Stats)
                {
                    ret = -1;
                }
                else if (c2Stats < c1Stats)
                {
                    ret = 1;
                }
                else
                {
                    ret = 0; //in this case they are equal
                }
            }
            else //if c1stuff < c2stuff
            {
                ret = -1;
            }
            return ret;
        }

        /// <summary>
        /// Returns a string representing the current status of the JudgingState
        /// </summary>
        /// <returns>A string representing the status of the JusgingState</returns>
        public override string ToString()
        {
            return status;
        }

        /// <summary>
        /// Calls the drawer to draw the state
        /// </summary>
        public override void draw()
        {
            drawer.draw();
        }
    }
}
