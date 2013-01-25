﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    abstract class SimulationState
    {
        protected Simulation simulation;

        public abstract void update(GameTime gameTime);

        public abstract void draw();
    }
}