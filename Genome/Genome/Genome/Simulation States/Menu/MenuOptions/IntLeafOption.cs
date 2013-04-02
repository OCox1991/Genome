using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The IntLeafOption represents a leaf option that is associated with an int in the Simulation
    /// </summary>
    class IntLeafOption : LeafOption
    {
        private Func<int> getMethod;
        private Action<int> setMethod;
        private int maxValue;
        private int minValue;

        /// <summary>
        /// Initialises the leaf option
        /// </summary>
        /// <param name="text">The text title of the leaf option, what is shown when it is in the list</param>
        /// <param name="description">The detailed description of the node, this is not displayed at the moment, but could be in a future version</param>
        /// <param name="menu">The menu the option is a part of</param>
        /// <param name="getMethod">The get method for the variable this option represents</param>
        /// <param name="setMethod">The set method for the variable this option represents</param>
        /// <param name="maxValue">The maximum value for this variable</param>
        /// <param name="minValue">The minimum value for this variable</param>
        public IntLeafOption(string text, string description, Menu menu, Func<int> getMethod, Action<int> setMethod, int maxValue, int minValue)
            : base(getMethod().ToString(), text, description, menu)
        {
            init(getMethod, setMethod, maxValue, minValue);
        }

        /// <summary>
        /// Initialises the leaf option, maximum value is set to unbounded and minimum value is set to 0
        /// </summary>
        /// <param name="text">The text title of the leaf option, what is shown when it is in the list</param>
        /// <param name="description">The detailed description of the node, this is not displayed at the moment, but could be in a future version</param>
        /// <param name="menu">The menu the option is a part of</param>
        /// <param name="getMethod">The get method for the variable this option represents</param>
        /// <param name="setMethod">The set method for the variable this option represents</param>
        public IntLeafOption(string text, string description, Menu menu, Func<int> getMethod, Action<int> setMethod)
            : base(getMethod().ToString(), text, description, menu)
        {
            init(getMethod, setMethod, int.MaxValue, 0);
        }

        /// <summary>
        /// Sets up the variables common to the 2 constructors
        /// </summary>
        /// <param name="getMethod">The get method for the variable this option represents</param>
        /// <param name="setMethod">The set method for the variable this option represents</param>
        /// <param name="maxValue">The maximum value for this variable</param>
        /// <param name="minValue">The minimum value for this variable</param>
        private void init(Func<int> getMethod, Action<int> setMethod, int maxValue, int minValue)
        {
            this.getMethod = getMethod;
            this.setMethod = setMethod;
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        /// <summary>
        /// Deals with if the button associated with this leaf is clicked. Gets input by switching the menu temporarily to an
        /// InputGetterState
        /// </summary>
        public override void clicked()
        {
            menu.getInput(this);
        }

        /// <summary>
        /// Deals with being given input
        /// </summary>
        /// <param name="input">The input that the InputGetter had got in the form of a string</param>
        public void takeInput(string input)
        {
            try
            {
                int val = int.Parse(input);
                if (val > maxValue && maxValue != -1)
                {
                    val = maxValue;
                }
                else if (val < minValue && minValue != -1)
                {
                    val = maxValue;
                }
                else
                {
                    setMethod(val);
                }
            }
            catch
            {
                //do nothing if a non well formed value has been entered
            }
        }

        /// <summary>
        /// The update method for this menu option, updates the button text and the button
        /// </summary>
        /// <param name="gameTime">The time since the game was last updated, not used by this method</param>
        public override void update(GameTime gameTime)
        {
            button.Text = getMethod().ToString();
            button.update(gameTime);
        }
    }
}
