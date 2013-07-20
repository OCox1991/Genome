using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genome
{
    /// <summary>
    /// The InputGetterDrawer is responsible for drawing the InputGetterOption associated with it
    /// </summary>
    class InputGetterDrawer
    {
        private InputGetterOption option; //The option associated with this one
        private SpriteFont font; //The SpriteFont to use
        private SpriteBatch sb; //The SpriteBatch, gotten from Display during initialisation

        /// <summary>
        /// Sets up the drawer, getting the font and spritebatch from the Display class
        /// </summary>
        /// <param name="option">The InputGetterOption that this object is in charge of drawing</param>
        public InputGetterDrawer(InputGetterOption option)
        {
            this.option = option;
            font = Display.getFont();
            sb = Display.getSpriteBatch();
        }

        /// <summary>
        /// The method that does everything necessary to draw the InputGetterOption
        /// </summary>
        public void draw()
        {
            string title = option.getText();
            string desc = option.getDescription();
            string[] lines = new string[5];
            if (Display.measureString(desc).X > 800)
            {
                string[] temp = desc.Split(' ');
                int i = 0;
                int j = 0;
                bool finished = false;
                while (!finished)
                {
                    lines[i] = "";
                    while (Display.measureString(lines[i]).X < 800 && !finished)
                    {
                        lines[i] += temp[j] + ' ';
                        j++;
                        if (j == temp.Length)
                        {
                            finished = true;
                        }
                    }
                    i++;
                    if (i == lines.Length)
                    {
                        finished = true;
                    }
                }
            }
            else
            {
                lines[0] = desc;
            }
            string input = option.Input;
            OptionButton okBtn = option.getButton();
            sb.Begin();
            sb.DrawString(font, title, new Vector2(Display.getWindowWidth() / 2 - Display.measureString(title).X / 2, 30), Color.Black);
            float startx = Display.getWindowWidth() / 2 - 400;
            float starty = 70;
            foreach (string s in lines)
            {
                if (s != null)
                {
                    sb.DrawString(font, s, new Vector2(startx, starty), Color.Black);
                    starty += Display.measureString(s).Y;
                }
            }
            if (input == "")
            {
                input = "Enter input here";
            }
            else
            {
                input = "Input: " + input;
            }
            sb.DrawString(font, input, new Vector2(Display.getWindowWidth() / 2 - Display.measureString(input).X, starty + 30), Color.Black);
            okBtn.setLocation(new Vector2(Display.getWindowWidth() / 2 - okBtn.getWidth() / 2), new Vector2(okBtn.getWidth(), okBtn.getHeight()));
            sb.End();
            //Once we've drawn everything else, use the Display's method to draw the button associated with the option
            Display.drawButton(okBtn);
        }
    }
}
