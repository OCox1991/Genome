using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// The LeafOption represents a leaf in the tree structure of the menu, it is a superclass for the various leaf options, but at the moment has
    /// no behaviour associated with it
    /// </summary>
    abstract class LeafOption : MenuOption
    {
        /// <summary>
        /// Initialises the menu option
        /// </summary>
        /// <param name="buttonText">The button text for the menu option</param>
        /// <param name="text">The text for the menu option, should be a sentence long description. This is what will be displayed in the branch list</param>
        /// <param name="description">The detailed description of the menu option</param>
        /// <param name="menu">The menu this object is associated with</param>
        protected LeafOption(string buttonText, string text, string description, Menu menu)
            : base(buttonText, text, description, menu)
        {
            //empty
        }

        /// <summary>
        /// The draw method override for the Leaf Options, should never be called
        /// </summary>
        public override void draw()
        {
            throw new NotImplementedException("Leaf options should never be drawn");
        }
    }
}
