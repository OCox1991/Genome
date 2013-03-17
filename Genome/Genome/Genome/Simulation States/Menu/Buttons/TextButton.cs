using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    abstract class TextButton : Button
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public TextButton(string text, Vector2 topLeft, Vector2 size, TextureNames texString)
            : base(topLeft, size, texString)
        {
            this.text = text;
        }

        protected abstract override void clicked();
    }
}
