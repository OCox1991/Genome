using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Genome
{
    /// <summary>
    /// The SingleStringDrawer is a generic drawer that can draw a single string at the centre of the screen as well as a menu button,
    /// used by the judging and initialisation states.
    /// </summary>
    class SingleStringDrawer
    {
        SimulationState state;
        SpriteFont font;
        SpriteBatch batch;
        MenuButton b;

        /// <summary>
        /// Sets up the drawer, getting the font and spritebatch from the Display class and initialising the MenuButton
        /// </summary>
        /// <param name="option">The SimulationState that this object is in charge of drawing</param>
        public SingleStringDrawer(SimulationState state)
        {
            this.state = state;
            font = Display.getFont();
            batch = Display.getSpriteBatch();
            b = new MenuButton(new Vector2(0, 0));
        }

        /// <summary>
        /// Draws the string that is returned when using the ToString method of the provided SimulationState in the centre of the
        /// screen, as well as a MenuButton in the top left.
        /// </summary>
        public void draw()
        {
            Display.drawButton(b);
            string s = state.ToString();
            float centreX = Display.getWindowWidth()/2 - (font.MeasureString(s).X / 2);
            float centreY = Display.getWindowHeight()/2 - (font.MeasureString(s).Y / 2);
            batch.Begin();
#if DEBUG
            Console.WriteLine(s);
#endif
            batch.DrawString(font, s, new Vector2(centreX, centreY), Color.Black);
            batch.End();
        }
    }
}
