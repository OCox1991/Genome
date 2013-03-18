using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    abstract class MenuOption
    {
        protected OptionButton button;
        protected string buttonText;
        protected string text;
        protected string description;
        protected Vector2 location;
        protected Menu menu;

        public MenuOption(string buttonText, string text, string description, Menu menu)
        {
            this.description = description;
            this.buttonText = buttonText;
            this.location = new Vector2(0,0);
            this.text = text;
            this.menu = menu;
            button = new OptionButton(new Vector2(0,0), new Vector2(100, 35), TextureNames.EMPTYBTN, this);
            button.Text = buttonText;
            Vector2 bLoc = new Vector2(location.X + Display.measureString(text).X + 5, location.Y);
            button.setLocation(bLoc, new Vector2(button.getWidth(), button.getHeight()));
        }

        public OptionButton getButton()
        {
            return button;
        }

        public string getText()
        {
            return text;
        }

        public string getDescription()
        {
            return description;
        }

        public void setLocation(Vector2 loc)
        {
            location = loc;
            Vector2 bLoc = new Vector2(location.X + Display.measureString(text).X + 5, location.Y - (button.getHeight() / 2 - Display.measureString(text).Y / 2));
            button.setLocation(bLoc, new Vector2(button.getWidth(), button.getHeight()));
        }

        public abstract void update(GameTime gameTime);

        public abstract void draw();

        public abstract void clicked();
    }
}
