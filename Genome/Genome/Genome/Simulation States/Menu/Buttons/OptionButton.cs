using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// An OptionButton is associated with a MenuButton, when clicked it calls a method in the MenuOption that does something different depending on the type of MenuOption
    /// </summary>
    class OptionButton : TextButton
    {
        private MenuOption associatedOption;

        /// <summary>
        /// Initialses the OptionButton, taking the location, size, texture and option to be associated with and setting the button text to be the default "unset".
        /// </summary>
        /// <param name="topLeft">The location of the top left of the button as a Vector2</param>
        /// <param name="size">The size of the button as a Vector2</param>
        /// <param name="texture">The TextureNames associated with the texture for this button</param>
        /// <param name="option">The option associated with this button</param>
        public OptionButton(Vector2 topLeft, Vector2 size, TextureNames texture, MenuOption option) : base("unset", topLeft, size, texture)
        {
            associatedOption = option;
        }

        /// <summary>
        /// When clicked informs the associated option it is clicked, letting the different MenuOption types handle being clicked in different ways
        /// </summary>
        public override void clicked()
        {
            associatedOption.clicked();
        }
    }
}
