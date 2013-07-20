using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// A Plant is a FoodSource that is more nutritious for herbivores and regrows if unmolested for long enough
    /// </summary>
    class Plant : FoodSource
    {
        int foodMax; //The max amount of food the plant can have one it
        /// <summary>
        /// The Constructor for the plants, takes the random object that is used by the world to ensure that when many of the plants are generated in quick succession they
        /// will not end up with the same values from the generator, an error caused when many Randoms are initialised without seeds in quick sucession.
        /// </summary>
        /// <param name="r">A random number generator for the Plant to use</param>
        public Plant(Random r)
        {
            foodValue = Simulation.getPlantFoodValue();
            if (!(Simulation.getPlantFoodValueVariation() == 0))
            {
                int flip = r.Next(3); //either add, subtract or leave the food value the same
                if (flip == 1)
                {
                    foodValue += r.Next(foodValue * (1 / Simulation.getPlantFoodValueVariation()));
                }
                else if (flip == 2)
                {
                    foodValue -= r.Next(foodValue * (1 / Simulation.getPlantFoodValueVariation()));
                }
            }
            foodMax = Simulation.getPlantFoodMax();
            foodRemaining = foodMax;
            actTimer = Simulation.getNumTicksToRegrowPlant();
        }

        /// <summary>
        /// The overridden act method regrows a food unit if the plant is not already at the maximum
        /// </summary>
        public override void act()
        {
            if (foodRemaining < foodMax)
            {
                foodRemaining++;
            }
        }

        /// <summary>
        /// Overrides the FoodSource isPlant method to say that this is a Plant. Used to simplify type identification when polymorphism makes the Plant a FoodSource object
        /// </summary>
        /// <returns>True, the plant class will always be a Plant Object</returns>
        public override bool isPlant()
        {
            return true;
        }

        /// <summary>
        /// Checks if the Plant is depleted, this is, if it has no food units remaining
        /// </summary>
        /// <returns>If the plant has 0 food units remaining</returns>
        public bool isDepeleted()
        {
            return foodRemaining == 0;
        }

        /// <summary>
        /// Resets the Plant, used when the Simulation resets the world, resets the number of food units, checks the actTimer is correct and resets the time till acting
        /// </summary>
        public void reset()
        {
            foodRemaining = foodMax;
            actTimer = Simulation.getNumTicksToRegrowPlant();
            timeTillActing = 0;
        }
    }
}
