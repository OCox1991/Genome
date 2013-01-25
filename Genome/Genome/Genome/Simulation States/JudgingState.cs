using System;
using System.Collections;
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
        bool isJudged;
        bool isEliminated;
        bool isBred;
        int numBred;
        string status;

        ArrayList aliveCreatures;
        Stack deadCreatures;
        ArrayList creatureList;

        public JudgingState(ArrayList aliveCreatures, Stack deadCreatures)
        {
            this.aliveCreatures = aliveCreatures;
            this.deadCreatures = deadCreatures;
            isJudged = false;
            isEliminated = false;
            isBred = false;
            numBred = 0;
            status = "Judging Creatures...";
        }

        public void update(GameTime t)
        {
            if (!isJudged)
            {
                aliveCreatures.Sort(); //TODO: fix this
                isJudged = true;
                status = "Eliminating creatures...";
            }
            else if (!isEliminated)
            {
                status = "Breeding next generation: " + numBred + "/" + Simulation.getPopulation();
            }
            else if (!isBred)
            {
                if (numBred > Simulation.getPopulation())
                {
                    status = "Done!";
                    isBred = true;
                }
                else
                {
                    status = "Breeding next generation: " + numBred + "/" + Simulation.getPopulation();
                    numBred++;
                }
            }
            else
            {
                Simulation.judgingDone(creatureList);
            }

        }

        public void draw()
        {

        }
    }
}
