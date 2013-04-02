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
    /// <summary>
    /// The SpeedUpButton speeds up the simulation when clicked on
    /// </summary>
    class SpeedUpButton : Button
    {
        private WorldInputHandler inputHandler;

        /// <summary>
        /// Sets up the button with all the required variables
        /// </summary>
        /// <param name="topLeft">The location of the top left of the button as a Vector2</param>
        /// <param name="inputHandler">The inputHandler to associate with this object</param>
        public SpeedUpButton(Vector2 topLeft, WorldInputHandler inputHandler)
            : base(topLeft, new Vector2(35, 35), TextureNames.SPEEDUP)
        {
            this.inputHandler = inputHandler;
        }

        /// <summary>
        /// Calls the speedUp method of the inputHandler when clicked on
        /// </summary>
        public override void clicked()
        {
            inputHandler.speedUp();
        }
    }
}
