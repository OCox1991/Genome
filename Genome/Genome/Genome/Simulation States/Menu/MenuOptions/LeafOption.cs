using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    abstract class LeafOption : MenuOption
    {
        protected LeafOption(string buttonText, string text, string description, Menu menu)
            : base(buttonText, text, description, menu)
        {
            //empty
        }

        public override void draw()
        {
            //do nothing, should never be called
            throw new NotImplementedException("Draw for leaf nodes in the menu should never be called");
        }
    }
}
