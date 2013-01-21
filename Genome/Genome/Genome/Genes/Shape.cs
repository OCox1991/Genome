using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// Shape is a kind of lightweight Gene, however it cannot be a subclass of Gene since all Gene constructors try to perform pattern matching when created
    /// </summary>
    class Shape
    {
        ArrayList posMods;
        ArrayList negMods;
        Cell[][] cells;

        public Shape(Cell[][] cells, ArrayList posMods, ArrayList negMods)
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
        public ArrayList getPosMods()
        {
            return posMods;
        }

        /// <summary>
        /// Accessor method for the negative modifiers applied by this shape
        /// </summary>
        /// <returns>A list of tokens representing the negative modifiers applied by this shape</returns>
        public ArrayList getNegMods()
        {
            return negMods;
        }

        /// <summary>
        /// Gets the dominant colour of a given cell
        /// </summary>
        /// <param name="row">The row to find the cell in</param>
        /// <param name="col">The column to find the cell in</param>
        /// <returns>The dominant colour of the given cell</returns>
        public int getColour(int row, int col)
        {
            return cells[row][col].getDomColour();
        }
        #endregion
    }
}
