using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The KillButton class represents the button that can be clicked to kill the creature currently being viewed
    /// </summary>
    class KillButton : Button
    {
        private WorldInputHandler input;

        /// <summary>
        /// Sets up the kill button, sets its size and texture to those associated with the kill button, takes the location and the inputhandler to
        /// associate the button with
        /// </summary>
        /// <param name="input">The WorldInputHandler to associate this button with</param>
        /// <param name="topLeft">The location to place this button as a Vector2</param>
        public KillButton(WorldInputHandler input, Vector2 topLeft) : base(topLeft, new Vector2(30, 30), TextureNames.KILL)
        {
            this.input = input;
        }

        /// <summary>
        /// Kills the creature currently being viewed in the world
        /// </summary>
        public override void clicked()
        {
            input.getCreature().userKillCreature();
        }
    }
}
