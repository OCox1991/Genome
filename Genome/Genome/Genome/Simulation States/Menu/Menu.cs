using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class Menu : SimulationState
    {
        protected MenuOption selectedOption;
        protected Stack<MenuOption> prevOptions;
        protected MenuButton menuButton;
        protected BackButton backButton;

        public Menu()
        {
            this.selectedOption = null;
            this.prevOptions = new Stack<MenuOption>();
            menuButton = new MenuButton(new Vector2(0, 0));
            backButton = new BackButton(new Vector2(menuButton.getWidth() + 1, 0), this);
            backButton.setVisible(false);
        }

        public void select(MenuOption o)
        {
            if (selectedOption != null)
            {
                prevOptions.Push(selectedOption);
            }
            selectedOption = o;
        }

        public void back()
        {
            selectedOption = prevOptions.Pop();
        }

        public void passInput(string input, IntLeafOption option)
        {
            back();
            option.takeInput(input);
        }

        public void getInput(IntLeafOption option)
        {
            select(new InputGetterOption(this, option));
        }

        public override void update(GameTime gameTime)
        {
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

        public override void draw()
        {
            Display.drawButton(menuButton);
            Display.drawButton(backButton);
            selectedOption.draw();
        }
    }
}
