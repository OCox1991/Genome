using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The textbutton is an extension of the button class that allows the button to have any arbitrary text in it, used mainly for the option
    /// buttons, since it doesn't look as good as the pre drawn buttons.
    /// </summary>
    abstract class TextButton : Button
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Sets up the button with all the required variables needed for a regular button, as well as a string for the text of the button
        /// </summary>
        /// <param name="text">The text of the button</param>
        /// <param name="topLeft">The location of the button as a Vector2</param>
        /// <param name="size">The size of the button as a Vector2</param>
        /// <param name="texString">The texture associated with the button (in the case of this button this texture will serve as the background and have the text displayed over it)</param>
        public TextButton(string text, Vector2 topLeft, Vector2 size, TextureNames texString)
            : base(topLeft, size, texString)
        {
            this.text = text;
        }
    }
}
