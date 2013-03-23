using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class KillButton : Button
    {
        private WorldInputHandler input;
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
