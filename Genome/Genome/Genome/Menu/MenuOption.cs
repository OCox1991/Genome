using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    abstract class MenuOption
    {
        protected string text;
        protected bool currentlySelected;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        protected Menu menu;

        public MenuOption(Menu menu, string text)
        {
            this.text = text;
            this.menu = menu;
            currentlySelected = false;
        }

        public void deselect()
        {
            currentlySelected = false;
        }

        public void clicked()
        {
            menu.deselect();
            currentlySelected = true;
        }

        protected abstract void selected();
    }
}
