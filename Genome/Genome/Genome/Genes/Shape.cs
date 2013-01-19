using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Shape : Gene
    {
        ParamToken[] posMods;
        ParamToken[] negMods;

        public Shape(Cell[][] cells, ParamToken[] posMods, ParamToken[] negMods)
        {
            this.cells = cells;
            this.posMods = posMods;
            this.negMods = negMods;
        }

        #region methods
        /// <summary>
        /// Accessor method for the positive modifiers applied by this shape
        /// </summary>
        /// <returns>A list of tokens representing the positive modifiers applied by this shape</returns>
        public ParamToken[] getPosMods()
        {
            return posMods;
        }

        /// <summary>
        /// Accessor method for the negative modifiers applied by this shape
        /// </summary>
        /// <returns>A list of tokens representing the negative modifiers applied by this shape</returns>
        public ParamToken[] getNegMods()
        {
            return negMods;
        }
        #endregion
    }
}
