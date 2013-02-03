using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Remains : FoodSource
    {
        public Remains(Random r)
        {
            foodValue = Simulation.getRemainsFoodValue();
            if (!Simulation.getRemainsFoodValueVariation() == 0)
            {
                int flip = r.Next(2);
                if (flip == 1)
                {
                    foodValue += foodValue * (1 / Simulation.getRemainsFoodValueVariation());
                }
                else
                {
                    foodValue -= foodValue * (1 / Simulation.getRemainsFoodValueVariation());
                }
            }
            foodRemaining = Simulation.getRemainsFoodAmount();
            actTimer = Simulation.getNumTicksToDecayRemains();
        }

        public override void act()
        {
            foodRemaining--;
        }

        public bool fullyDecayed()
        {
            return foodRemaining == 0;
        }
    }
}
