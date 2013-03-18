using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genome
{
    class InputGetterOption : MenuOption
    {
        private KeyboardState currentState;
        private KeyboardState prevState;
        private IntLeafOption returnOption;
        private string input;
        public string Input
        {
            get { return input; }
        }
        private InputGetterDrawer drawer;

        public InputGetterOption(Menu menu, IntLeafOption option)
            : base("OK", "Enter Value:", option.getDescription(), menu)
        {
            returnOption = option;
            input = "";
            drawer = new InputGetterDrawer(this);
        }

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

        public string getInput()
        {
            return input;
        }

        public override void draw()
        {
            drawer.draw();
        }
    }
}
