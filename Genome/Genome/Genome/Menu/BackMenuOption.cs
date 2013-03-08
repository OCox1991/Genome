using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class BackMenuOption : MenuOption
    {
        public BackMenuOption(Menu menu) : base(menu, "Back")
        {

        }

        protected override void selected()
        {
            menu.Options = menu.PrevOptions;
            menu.Description = menu.PrevDescription;
            menu.Title = menu.PrevTitle;
        }
    }
}
