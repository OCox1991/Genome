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
    /// The MenuButton causes the Simulation to go to the main menu
    /// </summary>
    class MenuButton : Button
    {
        /// <summary>
        /// Sets up the main menu, providing the predecided sizes and texture
        /// </summary>
        /// <param name="topLeft">The location to place the button at</param>
        public MenuButton(Vector2 topLeft) : base(topLeft, new Vector2(150, 50), TextureNames.MENU)
        {
        }

        /// <summary>
        /// Calls the goToMenu method of the Simulation when clicked on
        /// </summary>
        public override void clicked()
        {
            Simulation.goToMenu();
        }
    }
}
