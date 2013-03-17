using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// A BackButton is a button that transits to the previous state of a menu when clicked
    /// </summary>
    class BackButton : Button
    {
        private Menu menu;

        /// <summary>
        /// Constructor for BackButton objects
        /// </summary>
        /// <param name="topLeft">The location of the top leftmost point of the button as a Vector2</param>
        /// <param name="size">The size of the button as a Vector2</param>
        /// <param name="texString">The TextureNames enum that corresponds to the texture to use for this button</param>
        /// <param name="menu">The menu this button is a part of</param>
        public BackButton(Vector2 topLeft,Menu menu)
            : base(topLeft, new Vector2(150, 50), TextureNames.BACK)
        {
            this.menu = menu;
        }

        /// <summary>
        /// Tells the button what to do when clicked, goes back one screen on the menu
        /// </summary>
        protected override void clicked()
        {
            menu.back();
        }
    }
}
