using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    /// <summary>
    /// The MainMenuState is a menu that contains only 4 buttons, as such it does not use the more complex
    /// tree structure of the OptionsMenuState and contains just a list of buttons.
    /// </summary>
    class MainMenuState : SimulationState
    {
        private List<Button> buttons;

        /// <summary>
        /// Initialises the MainMenuState, setting up the buttons that the menu contains
        /// </summary>
        public MainMenuState()
        {
            Vector2 screenCentre = new Vector2(Display.getWindowWidth() / 2, Display.getWindowHeight() / 2);
            Vector2 buttonSize = new Vector2(250, 50);

            float x = screenCentre.X - buttonSize.X / 2;
            float top = screenCentre.Y - buttonSize.Y * 2;

            buttons = new List<Button>();
            buttons.Add(new ResumeButton(new Vector2(x, top - 2), buttonSize, TextureNames.RESUME));
            buttons.Add(new RestartButton(new Vector2(x, top + buttonSize.Y - 1), buttonSize, TextureNames.RESTART));
            buttons.Add(new OptionsButton(new Vector2(x, top + buttonSize.Y * 2), buttonSize, TextureNames.OPTIONSMENUBTN));
            buttons.Add(new QuitButton(new Vector2(x, top + buttonSize.Y * 3 + 1), buttonSize, TextureNames.QUIT));
        }

        /// <summary>
        /// Updates the MainMenuState, updating all the buttons
        /// </summary>
        /// <param name="gameTime">The time since update was last called by the Game, passed to the buttons 
        /// when their update method is called</param>
        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (Button b in buttons)
            {
                b.update(gameTime);
            }
        }

        /// <summary>
        /// Draws the MainMenuState, uses the Display.drawButton method to draw each button in the menu
        /// </summary>
        public override void draw()
        {
            foreach (Button b in buttons)
            {
                Display.drawButton(b);
            }
        }
    }
}
