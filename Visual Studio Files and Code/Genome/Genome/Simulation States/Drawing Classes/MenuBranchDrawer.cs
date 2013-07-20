using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genome
{
    /// <summary>
    /// The MenuBranchDrawer is in charge of drawing the menu branches, which are lists with buttons associated with them
    /// </summary>
    class MenuBranchDrawer
    {
        private BranchOption option;
        private SpriteFont font;
        private SpriteBatch sb;

        /// <summary>
        /// Sets up the drawer, getting the font and spritebatch from the Display class
        /// </summary>
        /// <param name="option">The BranchOption that this object is in charge of drawing</param>
        public MenuBranchDrawer(BranchOption option)
        {
            this.option = option;
            font = Display.getFont();
            sb = Display.getSpriteBatch();
        }

        /// <summary>
        /// Draws the BranchOption associated with this drawer
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
            starty += 20;
            List<MenuOption> options = option.getVisOptions();
            foreach (MenuOption o in options)
            {
                o.setLocation(new Vector2(startx, starty));
                sb.DrawString(font, o.getText(), new Vector2(startx, starty), Color.Black);
                starty += 50;
            }
            sb.End();
            foreach (MenuOption o in options)
            {
                Display.drawButton((TextButton)o.getButton());
            }
        }
    }
}
