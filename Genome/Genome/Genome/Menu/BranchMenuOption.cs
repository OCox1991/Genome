using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class BranchMenuOption : MenuOption
    {
        private List<MenuOption> options;
        private string description;

        public BranchMenuOption(Menu menu, string text, string description, List<MenuOption> options) : base(menu, text)
        {
            this.description = description;
            this.options = options;
        }

        protected override void selected()
        {
            menu.Title = text;
            menu.Options = options;
            menu.Description = description;
        }
    }
}
