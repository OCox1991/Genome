using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The ViewMoreInfoButton is used when creatures plants and remains are viewed in the world, the button cycles through different pieces of information about the viewed object
    /// </summary>
    class ViewMoreInfoButton : Button
    {
        WorldInputHandler inputHandler;

        /// <summary>
        /// Sets up the button with all the required variables
        /// </summary>
        /// <param name="topLeft">The location of the button as a Vector2</param>
        /// <param name="input">The WorldInputHandler associated with this button</param>
        public ViewMoreInfoButton(Vector2 topLeft, WorldInputHandler input) : base(topLeft, new Vector2(100 , 30), TextureNames.MOREINFO)
        {
            this.inputHandler = input;
        }

        /// <summary>
        /// Calls the nextInfo method of the inputHandler, which cycles through all the different pieces of info available about the selected object
        /// </summary>
        public override void clicked()
        {
            inputHandler.nextInfo();
        }
    }
}
