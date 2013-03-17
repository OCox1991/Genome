using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Genome
{
    class IntLeafOption : LeafOption
    {
        private Func<int> getMethod;
        private Action<int> setMethod;
        private int maxValue;
        private int minValue;

        public IntLeafOption(string text, string description, Menu menu, Func<int> getMethod, Action<int> setMethod, int maxValue, int minValue)
            : base(getMethod().ToString(), text, description, menu)
        {
            init(getMethod, setMethod, maxValue, minValue);
        }

        public IntLeafOption(string text, string description, Menu menu, Func<int> getMethod, Action<int> setMethod)
            : base(getMethod().ToString(), text, description, menu)
        {
            init(getMethod, setMethod, int.MaxValue, 0);
        }

        private void init(Func<int> getMethod, Action<int> setMethod, int maxValue, int minValue)
        {
            this.getMethod = getMethod;
            this.setMethod = setMethod;
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        public override void clicked()
        {
            menu.getInput(this);
        }

        public void takeInput(string input)
        {
            try
            {
                int val = int.Parse(input);
                if (val > maxValue && maxValue != -1)
                {
                    val = maxValue;
                }
                else if (val < minValue && minValue != -1)
                {
                    val = maxValue;
                }
                else
                {
                    setMethod(val);
                }
            }
            catch
            {
                //do nothing if a non well formed value has been entered
            }
        }

        public override void update(GameTime gameTime)
        {
            button.Text = getMethod().ToString();
            button.update(gameTime);
        }
    }
}
