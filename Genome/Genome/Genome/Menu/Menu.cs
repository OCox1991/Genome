using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    abstract class Menu
    {
        protected string title;
        protected string description;
        protected List<MenuOption> options;

        protected List<MenuOption> prevOptions;
        protected string prevTitle;
        protected string prevDescription;

        protected List<MenuOption> topOptions; //need to write init and access for these
        protected string topTitle;
        protected string topDescription;

        #region public accessor
        public string Title
        {
            get { return title; }
            set
            {
                prevTitle = title;
                title = value; 
            }
        }

        public string PrevTitle
        {
            get { return prevTitle; }
        }

        public string Description
        {
            get { return description; }
            set
            {
                prevDescription = description;
                description = value; 
            }
        }

        public string PrevDescription
        {
            get { return prevDescription; }
        }

        public List<MenuOption> Options
        {
            get { return options; }
            set
            {
                prevOptions = options;
                options = value; 
            }
        }

        public List<MenuOption> PrevOptions
        {
            get { return prevOptions; }
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
        }

        public void deselect()
        {
            foreach (MenuOption m in options)
            {
                m.deselect();
            }
        }
    }
}
