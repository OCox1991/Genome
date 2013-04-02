using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// Objects of class Remains are FoodSources that are better for carnivores and decay when they act
    /// </summary>
    class Remains : FoodSource
    {
        /// <summary>
        /// Sets up the Remains with a random object to prevent the issues with using Randoms that are generated with seeds based on the system clock
        /// when they are generated quickly
        /// </summary>
        /// <param name="r">The Random object this Remains object will use</param>
        public Remains(Random r)
        {
            foodValue = Simulation.getRemainsFoodValue();
            if (!(Simulation.getRemainsFoodValueVariation() == 0))
            {
                int flip = r.Next(3); //either add, subtract or leave the same
                if (flip == 1)
                {
                    foodValue += r.Next(foodValue * (1 / Simulation.getRemainsFoodValueVariation()));
                }
                else if(flip == 2)
                {
                    foodValue -= r.Next(foodValue * (1 / Simulation.getRemainsFoodValueVariation()));
                }
            }
            foodRemaining = Simulation.getRemainsFoodAmount();
            actTimer = Simulation.getNumTicksToDecayRemains();
        }

        /// <summary>
        /// Returns if this object is a plant, used when polymorphism ends up with this object being of type FoodSource
        /// </summary>
        /// <returns>false, Remains objects will never be plants</returns>
        public override bool isPlant()
        {
            return false;
        }

        /// <summary>
        /// When it acts the remains decays and loses one food unit
        /// </summary>
        public override void act()
        {
            foodRemaining--;
        }

        /// <summary>
        /// Checks if the remains are fully decayed, that is if they have 0 or less food units less, typically called after all the remains are updated
        /// </summary>
        /// <returns>True if 0 or less food remains, false otherwise</returns>
        public bool fullyDecayed()
        {
            return foodRemaining <= 0;
        }
    }
}
