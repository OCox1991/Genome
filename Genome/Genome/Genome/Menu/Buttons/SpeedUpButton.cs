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
    class SpeedUpButton : Button
    {
        private WorldInputHandler inputHandler;

        public SpeedUpButton(Vector2 topLeft, WorldInputHandler inputHandler)
            : base(topLeft, new Vector2(35, 35), TextureNames.SPEEDUP)
        {
            this.inputHandler = inputHandler;
        }

        protected override void clicked()
        {
            inputHandler.speedUp();
        }
    }
}
