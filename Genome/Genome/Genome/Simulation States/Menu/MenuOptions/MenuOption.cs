using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// A menu option represents a node in the tree structure of the menu
    /// </summary>
    abstract class MenuOption
    {
        protected OptionButton button;
        protected string buttonText;
        protected string text;
        protected string description;
        protected Vector2 location;
        protected Menu menu;

        /// <summary>
        /// Initialises the menu option
        /// </summary>
        /// <param name="buttonText">The button text for the menu option. Should be a single word or value that is not too long</param>
        /// <param name="text">The text of the menu option, a sentence description of the option that will be used when it is displayed in a list and the title when it is selected</param>
        /// <param name="description">The description of the menu option, a more in depth description of the purpose of the menu option</param>
        /// <param name="menu">The menu that this option is a part of, used to update the current node of the menu etc.</param>
        public MenuOption(string buttonText, string text, string description, Menu menu)
        {
            this.description = description;
            this.buttonText = buttonText;
            this.location = new Vector2(0,0);
            this.text = text;
            this.menu = menu;
            button = new OptionButton(new Vector2(0,0), new Vector2(100, 35), TextureNames.EMPTYBTN, this);
            button.Text = buttonText;
            Vector2 bLoc = new Vector2(location.X + Display.measureString(text).X + 5, location.Y);
            button.setLocation(bLoc, new Vector2(button.getWidth(), button.getHeight()));
        }

        /// <summary>
        /// Gets the option button associated with this MenuOption
        /// </summary>
        /// <returns>The button associated with this option</returns>
        public OptionButton getButton()
        {
            return button;
        }

        /// <summary>
        /// Gets the text of this option
        /// </summary>
        /// <returns>The text of this option, the text is a sentence description or title of the option</returns>
        public string getText()
        {
            return text;
        }

        /// <summary>
        /// The description of this option
        /// </summary>
        /// <returns>A string description of this option, a more in depth description of the options purpose etc.</returns>
        public string getDescription()
        {
            return description;
        }

        /// <summary>
        /// Sets the location to display the option
        /// </summary>
        /// <param name="loc">The location to display the option as a Vector2</param>
        public void setLocation(Vector2 loc)
        {
            location = loc;
            Vector2 bLoc = new Vector2(location.X + Display.measureString(text).X + 5, location.Y - (button.getHeight() / 2 - Display.measureString(text).Y / 2));
            button.setLocation(bLoc, new Vector2(button.getWidth(), button.getHeight()));
        }

        /// <summary>
        /// An abstract update method
        /// </summary>
        /// <param name="gameTime">The time since the game was last updated</param>
        public abstract void update(GameTime gameTime);

        /// <summary>
        /// An abstract draw method
        /// </summary>
        public abstract void draw();

        /// <summary>
        /// An abstract method for the menuoption to call when it is clicked
        /// </summary>
        public abstract void clicked();
    }
}
