using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// A branch option represents a branch in a menu that is viewed as a tree, the root node will usually be one of these
    /// </summary>
    class BranchOption : MenuOption
    {
        private List<MenuOption> options; //the list of options associated with this branch
        private int listTop; //the option currently at the top of the list that will be displayed
        private int listDispLength; //the display length of the list
        private MenuBranchDrawer drawer; //The drawer class for this option state

        /// <summary>
        /// Sets up the branch options, sets the button text to "select"
        /// </summary>
        /// <param name="text">The text to display this option as if it is in a list of menu options</param>
        /// <param name="description">The description of this option that is displayed at the top of the screen under the title if it is the selected option</param>
        /// <param name="menu">The menu object associated with this menu option</param>
        public BranchOption(string text, string description, Menu menu) : base("select", text, description, menu)
        {
            options = new List<MenuOption>();
            listTop = 0;
            listDispLength = 5;
            drawer = new MenuBranchDrawer(this); //TODO: this
            button.Text = "Select";
        }

        /// <summary>
        /// Adds an option to the list this object represents
        /// </summary>
        /// <param name="m">The option to add to the list</param>
        public void addOption(MenuOption m)
        {
            options.Add(m);
        }

        public List<MenuOption> getVisOptions()
        {
            List<MenuOption> returning;
            if (listDispLength < options.Count)
            {
                returning = options.GetRange(listTop, listDispLength);
            }
            else
            {
                returning = options;
            }
            return returning;
        }

        /// <summary>
        /// Tells the option what to do if the button associated with it is clicked
        /// </summary>
        public override void clicked()
        {
            menu.select(this);
        }

        /// <summary>
        /// Calls update, updates all the options in the list and the button associated with this object
        /// </summary>
        /// <param name="gameTime">The time since the last update call in the form of a TimeSpan</param>
        public override void update(GameTime gameTime)
        {
            foreach (MenuOption o in options)
            {
                o.update(gameTime);
            }
            button.update(gameTime);
        }

        /// <summary>
        /// Scrolls the list of options down if the bottom hasn't been reached yet
        /// </summary>
        public void scrollDown()
        {
            if (listTop < options.Count - listDispLength)
            {

            }
            else
            {
                listTop++;
            }
        }

        /// <summary>
        /// Scrolls the list of options up if we aren't already at the top
        /// </summary>
        public void scrollUp()
        {
            if (listTop - 1 < 0)
            {

            }
            else
            {
                listTop--;
            }
        }

        /// <summary>
        /// Draws the list of options
        /// </summary>
        public override void draw()
        {
            drawer.draw();
        }
    }
}
