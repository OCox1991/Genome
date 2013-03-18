using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Genome
{
    class SlowDownButton : Button
    {
        WorldInputHandler inputHandler;

        public SlowDownButton(Vector2 topLeft, WorldInputHandler inputHandler) 
            : base(topLeft, new Vector2(35,35), TextureNames.SLOWDOWN)
        {
            this.inputHandler = inputHandler;
        }

        public override void clicked()
        {
            inputHandler.slowDown();
        }
    }
}
