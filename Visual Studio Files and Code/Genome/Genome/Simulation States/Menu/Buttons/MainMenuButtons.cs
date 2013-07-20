using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    // This section contains class definitions for all the buttons used by the MainMenu

    /// <summary>
    /// The ResumeButton resumes the simulation and returns to the WorldState
    /// </summary>
    class ResumeButton : Button
    {
        /// <summary>
        /// Sets up the button with the correct information provided to the base class
        /// </summary>
        /// <param name="loc">The location to place the button</param>
        /// <param name="size">The size of the button</param>
        /// <param name="tex">The texture to give to the button</param>
        public ResumeButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        /// <summary>
        /// When clicked calls the resume method of the Simulation
        /// </summary>
        public override void clicked()
        {
            Simulation.resume();
        }
    }

    /// <summary>
    /// The RestartButton restarts the Simulation, regenerating the Creatures and World with any new parameters desired
    /// </summary>
    class RestartButton : Button
    {
        /// <summary>
        /// Sets up the button with the correct information provided to the base class
        /// </summary>
        /// <param name="loc">The location to place the button</param>
        /// <param name="size">The size of the button</param>
        /// <param name="tex">The texture to give to the button</param>
        public RestartButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        /// <summary>
        /// When clicked calls the restart method of the Simulation
        /// </summary>
        public override void clicked()
        {
            Simulation.restart();
        }
    }

    /// <summary>
    /// The OptionsButton causes the Simulation to change to the options menu state
    /// </summary>
    class OptionsButton : Button
    {
        /// <summary>
        /// Sets up the button with the correct information provided to the base class
        /// </summary>
        /// <param name="loc">The location to place the button</param>
        /// <param name="size">The size of the button</param>
        /// <param name="tex">The texture to give to the button</param>
        public OptionsButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        /// <summary>
        /// When clicked calls the options method of the Simulation
        /// </summary>
        public override void clicked()
        {
            Simulation.options();
        }
    }

    /// <summary>
    /// The QuitButton causes the Simulation to exit
    /// </summary>
    class QuitButton : Button
    {
        /// <summary>
        /// Sets up the button with the correct information provided to the base class
        /// </summary>
        /// <param name="loc">The location to place the button</param>
        /// <param name="size">The size of the button</param>
        /// <param name="tex">The texture to give to the button</param>
        public QuitButton(Vector2 loc, Vector2 size, TextureNames tex)
            : base(loc, size, tex)
        {

        }

        /// <summary>
        /// When clicked calls the quit method of the Simulation
        /// </summary>
        public override void clicked()
        {
            Simulation.quit();
        }
    }
}
