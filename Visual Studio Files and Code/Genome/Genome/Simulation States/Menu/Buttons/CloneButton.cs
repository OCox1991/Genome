using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The CloneButton is the button that can be clicked to clone the creature currently being viewed
    /// </summary>
    class CloneButton : Button
    {
        private WorldInputHandler input;

        /// <summary>
        /// Sets up the clone button, sets the size to the size of the texture for the clone button, and the texture to the correct one for the clone button
        /// requires the location and the WorldInputHandler to be associated with
        /// </summary>
        /// <param name="input">The WorldInputHandler to be associated with this button</param>
        /// <param name="topLeft">The location of this button as a Vector2 representing where the top left of the button is</param>
        public CloneButton(WorldInputHandler input, Vector2 topLeft) : base(topLeft, new Microsoft.Xna.Framework.Vector2(30, 30), TextureNames.CLONE)
        {
            this.input = input;
        }

        /// <summary>
        /// Sets the WorldInputHandler to be viewing a clone of the currently selected creature
        /// </summary>
        public override void clicked()
        {
            input.viewCreature(input.getCreature().userCloneCreature());
        }
    }
}
