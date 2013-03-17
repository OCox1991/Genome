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
    class SingleStringDrawer
    {
        SimulationState state;
        SpriteFont font;
        SpriteBatch batch;
        MenuButton b;
        

        public SingleStringDrawer(SimulationState state)
        {
            this.state = state;
            font = Display.getFont();
            batch = Display.getSpriteBatch();
            b = new MenuButton(new Vector2(0, 0));
        }

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
