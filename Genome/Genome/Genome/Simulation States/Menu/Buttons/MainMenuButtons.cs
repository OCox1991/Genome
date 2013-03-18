using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class ResumeButton : Button
    {
        public ResumeButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        public override void clicked()
        {
            Simulation.resume();
        }
    }

    class RestartButton : Button
    {
        public RestartButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        public override void clicked()
        {
            Simulation.restart();
        }
    }

    class OptionsButton : Button
    {
        public OptionsButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        public override void clicked()
        {
            Simulation.options();
        }
    }

    class QuitButton : Button
    {
        public QuitButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        public override void clicked()
        {
            Simulation.quit();
        }
    }
}
