using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class MainMenuState : SimulationState
    {
        private List<Button> buttons;

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

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (Button b in buttons)
            {
                b.update(gameTime);
            }
        }

        public override void draw()
        {
            foreach (Button b in buttons)
            {
                Display.drawButton(b);
            }
        }
    }
}
