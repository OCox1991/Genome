using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genome
{
    /// <summary>
    /// The Menu is a SimulationState that contains a tree structure and a pointer to the currently selected node. It also contains a stack of the previously selected options.
    /// </summary>
    class Menu : SimulationState
    {
        protected MenuOption selectedOption;
        protected Stack<MenuOption> prevOptions;
        protected MenuButton menuButton;
        protected BackButton backButton;
        protected KeyboardState prevState;

        /// <summary>
        /// Initialises this menu, sets the selected and previous options to null, sets up the menu and back buttons and sets the back button to non visible. Sets up the previous keyboard state.
        /// Once initialised with this constructor you need to select an option with the select method before the menu will work properly
        /// </summary>
        public Menu()
        {
            this.selectedOption = null;
            this.prevOptions = new Stack<MenuOption>();
            menuButton = new MenuButton(new Vector2(0, 0));
            backButton = new BackButton(new Vector2(menuButton.getWidth() + 1, 0), this);
            backButton.setVisible(false);
            prevState = new KeyboardState();
        }

        /// <summary>
        /// Sets the specified option as the current option and pushes the previous current option onto the prevoptions Stack
        /// </summary>
        /// <param name="o">The option to select</param>
        public void select(MenuOption o)
        {
            if (selectedOption != null)
            {
                prevOptions.Push(selectedOption);
            }
            selectedOption = o;
        }

        /// <summary>
        /// Sets the current element to be the first element popped out of the previous options stack
        /// </summary>
        public void back()
        {
            selectedOption = prevOptions.Pop();
        }

        /// <summary>
        /// Passes input gotten from an InputGetter to a specified IntLeafOption, the method in the IntLeaf is not called directly since the
        /// selected option must also be changed
        /// </summary>
        /// <param name="input">The input to pass in the form of a string</param>
        /// <param name="option">The option to pass the input to</param>
        public void passInput(string input, IntLeafOption option)
        {
            back(); //Go back since the current state will be an InputGetter and we want to go back to what it was before
            option.takeInput(input);
        }

        /// <summary>
        /// Called when an IntLeafOption is clicked, creates an InputGetterOption associated with the selected IntLeafOption and sets that to the current node
        /// </summary>
        /// <param name="option">The IntLeafOption that was clicked on</param>
        public void getInput(IntLeafOption option)
        {
            select(new InputGetterOption(this, option));
        }

        /// <summary>
        /// The update method for the menu, updates the selected option as well as updating the keyboard state and checking if esc was pressed to go back to the menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            KeyboardState cState = Keyboard.GetState();
            if (prevState.IsKeyDown(Keys.Escape) && cState.IsKeyUp(Keys.Escape))
            {
                menuButton.clicked();
            }
            prevState = cState;
            if (prevOptions.Count == 0)
            {
              backButton.setVisible(false);
            }
            else
            {
                backButton.setVisible(true);
            }
            selectedOption.update(gameTime);
            menuButton.update(gameTime);
            backButton.update(gameTime);
        }

        /// <summary>
        /// The draw method for the menu, draws the back and menu button and then calls the draw method of the selected node
        /// </summary>
        public override void draw()
        {
            Display.drawButton(menuButton);
            Display.drawButton(backButton);
            selectedOption.draw();
        }
    }
}
