using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    abstract class Menu : SimulationState
    {
        protected string title;
        protected string description;
        protected List<MenuOption> options;

        protected Stack<List<MenuOption>> prevOptions;
        protected Stack<string> prevTitle;
        protected Stack<string> prevDescription;

        #region public accessor
        public string Title
        {
            get { return title; }
            set
            {
                prevTitle.Push(title);
                title = value; 
            }
        }

        public string PrevTitle
        {
            get
            {
                if (prevTitle.Count == 0)
                {
                    return title;
                }
                else
                {
                    return prevTitle.Pop();
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                prevDescription.Push(description);
                description = value; 
            }
        }

        public string PrevDescription
        {
            get
            {
                if (prevDescription.Count == 0)
                {
                    return description;
                }
                else
                {
                    return prevDescription.Pop();
                }
            }
        }

        public List<MenuOption> Options
        {
            get { return options; }
            set
            {
                prevOptions.Push(options);
                options = value; 
            }
        }

        public List<MenuOption> PrevOptions
        {
            get
            {
                if (prevOptions.Count == 0)
                {
                    return options;
                }
                else
                {
                    return prevOptions.Pop();
                }
            }
        }

        private int optionSelected;
        public int OptionSelected
        {
            get { return optionSelected; }
            set { optionSelected = value; }
        }
        #endregion

        public Menu(List<MenuOption> options)
        {
            this.options = options;
            init();
        }

        public Menu()
        {
            options = new List<MenuOption>();
            init();
        }

        private void init()
        {
            prevOptions = new Stack<List<MenuOption>>();
            prevTitle = new Stack<string>();
            prevDescription = new Stack<string>();
        }

        public void deselect()
        {
            foreach (MenuOption m in options)
            {
                m.deselect();
            }
        }

        public void addOption(MenuOption o)
        {
            options.Add(o);
        }

        public abstract override void update(Microsoft.Xna.Framework.GameTime gameTime);

        public abstract override void draw();
    }
}
