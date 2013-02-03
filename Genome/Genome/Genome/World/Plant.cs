using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Plant : FoodSource
    {
        int foodMax;
        public Plant(Random r)
        {
            foodValue = Simulation.getPlantFoodValue();
            if (!Simulation.getPlantFoodValueVariation() == 0)
            {
                int flip = r.Next(2);
                if (flip == 1)
                {
                    foodValue += foodValue * (1 / Simulation.getPlantFoodValueVariation());
                }
                else
                {
                    foodValue -= foodValue * (1 / Simulation.getPlantFoodValueVariation());
                }
            }
            foodMax = Simulation.getPlantFoodMax();
            foodRemaining = foodMax;
            actTimer = Simulation.getNumTicksToRegrowPlant();
        }

        public override void act()
        {
            if (foodRemaining < foodMax)
            {
                foodRemaining++;
            }
        }

        public bool isDepeleted()
        {
            return foodRemaining == 0;
        }
    }
}
