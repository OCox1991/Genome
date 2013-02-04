using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class IntLeafMenuOption : MenuOption
    {
        Action<int> setMethod;
        int value;

        public IntLeafMenuOption(Menu menu, Action<int> method, int value) : base(menu, value.ToString())
        {
            this.setMethod = method;
            this.value = value;
        }

        protected override void selected()
        {
            setMethod(value);
        }
    }
}
