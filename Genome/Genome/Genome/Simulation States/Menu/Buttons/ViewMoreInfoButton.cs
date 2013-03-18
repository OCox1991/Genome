using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class ViewMoreInfoButton : Button
    {
        WorldInputHandler inputHandler;

        public ViewMoreInfoButton(Vector2 topLeft, WorldInputHandler input) : base(topLeft, new Vector2(100 , 30), TextureNames.MOREINFO)
        {
            this.inputHandler = input;
        }

        public override void clicked()
        {
            inputHandler.nextInfo();
        }
    }
}
