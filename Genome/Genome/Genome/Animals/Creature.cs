using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Creature
    {
        private Gene dna;
        //insert list of stats here

        //insert list of scenarios and responses here
        public Creature()
        {
            dna = new Gene();
            init();
        }

        public Creature(Gene dna)
        {
            this.dna = dna;
            init();
        }

        private void init()
        {

        }

        #region methods
        #region accessor
        public Gene getDna()
        {
            return dna;
        }
        #endregion

        public void act()
        {

        }

        #endregion
    }
}
