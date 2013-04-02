using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// FoodSource represents anything in the world that can be eaten, Plants & Remains
    /// </summary>
    abstract class FoodSource : WorldObject
    {
        protected int foodRemaining; //these 3 will be set by each subclass since they need to be different for each
        protected int foodValue;
        protected int actTimer;
        protected int timeTillActing;

        /// <summary>
        /// Sets up the time till acting timer and nothing else.
        /// </summary>
        public FoodSource()
        {
            timeTillActing = 0;
        }

        /// <summary>
        /// Deals with what happens when the foodsource is being eaten from
        /// </summary>
        public void beEaten()
        {
            timeTillActing = 0;
            foodRemaining --;
        }

        /// <summary>
        /// Allows us to distinguish between the different types of food source, this will be overridden in the Plant subclass
        /// to return true
        /// </summary>
        /// <returns>False except in the Plant subclass</returns>
        public abstract bool isPlant();

        /// <summary>
        /// Gets the value of the food, this will be different for each subclass
        /// </summary>
        /// <returns>The food value of the food</returns>
        public int getFoodValue()
        {
            return foodValue;
        }

        /// <summary>
        /// Gets the amount of food remaining
        /// </summary>
        /// <returns>The amount of food remaining on the object</returns>
        public int getFoodRemaining()
        {
            return foodRemaining;
        }

        /// <summary>
        /// Called each tick, and does some action after a certain number of ticks.
        /// </summary>
        public void tick()
        {
            if (timeTillActing >= actTimer)
            {
                act();
                timeTillActing = 0;
            }
            else
            {
                timeTillActing++;
            }
        }

        /// <summary>
        /// Returns the number of ticks before the act method is called
        /// </summary>
        /// <returns>The number of ticks until the act method is called as an int</returns>
        public int getTicksRemainingBeforeAction()
        {
            return actTimer - timeTillActing;
        }

        /// <summary>
        /// Returns if the FoodSource can be eaten, that is, if it has any food units still remaining on it
        /// </summary>
        /// <returns>If there is more than 0 food units remaining in the foodsource</returns>
        public bool canBeEaten()
        {
            return foodRemaining > 0;
        }

        /// <summary>
        /// An abstract method that must be overridden, handles what will happen each set of ticks.
        /// </summary>
        public abstract void act();
    }
}
