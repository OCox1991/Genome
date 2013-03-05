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
    class MenuButton : Button
    {
        public MenuButton(Vector2 topLeft) : base(topLeft, new Vector2(50, 10), "Menubutton")
        {
        }

        protected override void clicked()
        {
            Simulation.goToMenu();
        }
    }
}
