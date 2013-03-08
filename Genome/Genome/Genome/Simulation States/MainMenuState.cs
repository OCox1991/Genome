using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class MainMenuState : Menu
    {
        private MainMenuDrawer drawer;

        public MainMenuState()
        {
            drawer = new MainMenuDrawer(this);
        }

        public override void  draw()
        {
            drawer.draw();
        }

        public override void  update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
