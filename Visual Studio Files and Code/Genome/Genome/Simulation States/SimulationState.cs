using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Genome
{
    /// <summary>
    /// The SimulationState class is an abstract superclass for all the possible states the simulation can be in, and is
    /// a crucial part of the State design pattern.
    /// </summary>
    abstract class SimulationState
    {
        /// <summary>
        /// The update method that the SimulationState needs
        /// </summary>
        /// <param name="gameTime">The time since update was last called</param>
        public abstract void update(GameTime gameTime);

        /// <summary>
        /// The draw method that the SimulationState needs
        /// </summary>
        public abstract void draw();
    }
}
