using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genome
{
    /// <summary>
    /// The InputGetterOption gets input from the keyboard for the int options. Since only numbers are needed the InputGetter only recognises the numbers 1 to 0 and the minus symbol
    /// </summary>
    class InputGetterOption : MenuOption
    {
        private KeyboardState currentState;
        private KeyboardState prevState;
        private IntLeafOption returnOption;
        private string input;
        //Public accessors for private input variable
        public string Input
        {
            get { return input; }
        }
        private InputGetterDrawer drawer;

        /// <summary>
        /// Sets up the option, associates it with a menu and the IntLeafOption that needed it to get the input
        /// </summary>
        /// <param name="menu">The menu that this option is a part of</param>
        /// <param name="option">The option that needed the InputGetter to get some input for it</param>
        public InputGetterOption(Menu menu, IntLeafOption option)
            : base("OK", "Enter Value:", option.getDescription(), menu)
        {
            returnOption = option;
            input = "";
            drawer = new InputGetterDrawer(this);
        }

        /// <summary>
        /// When the button associated with an InputGetter is selected it informs the IntLeafOption that created it that it has new input for it.
        /// </summary>
        public override void clicked()
        {
            menu.passInput(input, returnOption);
        }

        public override void update(GameTime gameTime)
        {
            button.update(gameTime);
            UpdateInput();
        }

        /// <summary>
        /// Based on a method found at http://www.gamedev.net/topic/457783-xna-getting-text-from-keyboard/ (14/03/2013)
        /// Author: keshtath. 
        /// 
        /// While this is not an in depth solution to keyboard input it should be good enough for our purposes
        /// </summary>
        private void UpdateInput()
        {
            prevState = currentState;
            currentState = Keyboard.GetState();
            Keys[] numberKeys = new Keys[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.Subtract, Keys.OemMinus };

            foreach (Keys key in numberKeys)
            {
                if (prevState.IsKeyDown(key) && currentState.IsKeyUp(key))
                {
                    if (key == Keys.Subtract || key == Keys.OemMinus)
                    {
                        input += "-";
                    }
                    else
                    {
                        input += key.ToString().Substring(1);
                    }
                }
            }
            if (prevState.IsKeyDown(Keys.Enter) && currentState.IsKeyUp(Keys.Enter))
            {
                clicked();
            }
        }

        /// <summary>
        /// Calls the draw method of the InputGetterDrawer associated with this object
        /// </summary>
        public override void draw()
        {
            drawer.draw();
        }
    }
}
