using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class OptionsMenuState : Menu
    {
        private OMenuDrawer drawer;

        public OptionsMenuState() : base()
        {
            drawer = new OMenuDrawer(this);
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void draw()
        {
            drawer.draw();
        }
    }
}
