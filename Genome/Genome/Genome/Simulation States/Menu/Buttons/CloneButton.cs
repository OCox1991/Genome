using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class CloneButton : Button
    {
        private WorldInputHandler input;
        public CloneButton(WorldInputHandler input, Vector2 topLeft) : base(topLeft, new Microsoft.Xna.Framework.Vector2(30, 30), TextureNames.CLONE)
        {
            this.input = input;
        }

        public override void clicked()
        {
            input.viewCreature(input.getCreature().userCloneCreature());
        }
    }
}
