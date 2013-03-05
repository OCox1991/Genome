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
            if (!(Simulation.getPlantFoodValueVariation() == 0))
            {
                int flip = r.Next(3); //either add, subtract or leave the food value the same
                if (flip == 1)
                {
                    foodValue += foodValue * (1 / Simulation.getPlantFoodValueVariation());
                }
                else if (flip == 2)
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

        public void reset()
        {
            foodRemaining = foodMax;
            actTimer = Simulation.getNumTicksToRegrowPlant();
            timeTillActing = 0;
        }
    }
}
