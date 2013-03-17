using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class OptionButton : TextButton
    {
        private MenuOption associatedOption;

        public OptionButton(Vector2 topLeft, Vector2 size, TextureNames texture, MenuOption option) : base("unset", topLeft, size, texture)
        {
            associatedOption = option;
        }

        protected override void clicked()
        {
            associatedOption.clicked();
        }
    }
}
