using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// A BoolLeafOption represents a leaf of a menu viewed as a tree structure, the Bool part means the leaf is associated with a bool
    /// variable
    /// </summary>
    class BoolLeafOption : LeafOption
    {
        private Func<bool> getMethod; //A method to get the associated variable's value
        private Action<bool> setMethod; //A method to set the associated variable's value

        /// <summary>
        /// Sets up the button
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="description">The description of this leaf</param>
        /// <param name="menu">The menu associated with this leaf</param>
        /// <param name="getMethod">The get method for the variable this leaf affects</param>
        /// <param name="setMethod">The set method for the variable this leaf affects</param>
        public BoolLeafOption(string text, string description, Menu menu, Func<bool> getMethod, Action<bool> setMethod)
            : base(getMethod().ToString(), text, description, menu)
        {
            this.getMethod = getMethod;
            this.setMethod = setMethod;
        }

        /// <summary>
        /// Tells the button what to do when clicked on. This button toggles the status of the bool variable it has been given the methods
        /// to look up and to change
        /// </summary>
        public override void clicked()
        {
            if (getMethod())
            {
                setMethod(false);
            }
            else
            {
                setMethod(true);
            }
        }

        /// <summary>
        /// The method called to update the menu state, updates the button and sets what its text is
        /// </summary>
        /// <param name="gameTime">The time since the last update call</param>
        public override void update(GameTime gameTime)
        {
            button.Text = getMethod().ToString();
            button.update(gameTime);
        }
    }
}
