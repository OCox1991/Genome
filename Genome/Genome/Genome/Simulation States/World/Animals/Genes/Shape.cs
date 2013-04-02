using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// Shape is a kind of lightweight Gene, however it cannot be a subclass of Gene since all Gene constructors try to perform pattern matching when created
    /// </summary>
    class Shape
    {
        List<ParamToken> posMods;
        List<ParamToken> negMods;
        Cell[][] cells;

        public Shape(Cell[][] cells, List<ParamToken> posMods, List<ParamToken> negMods)
        {
            this.cells = cells;
            this.posMods = posMods;
            this.negMods = negMods;
        }

        public Shape(int[][] cells, List<ParamToken> posMods, List<ParamToken> negMods)
        {
            this.posMods = posMods;
            this.negMods = negMods;
            this.cells = new Cell[cells.Length][];
            for(int i = 0; i < cells.Length; i++)
            {
                this.cells[i] = new Cell[cells[i].Length];
                for (int j = 0; j < cells.Length; j++)
                {
                    int c = cells[i][j];
                    this.cells[i][j] = new Cell(c, c);
                }
            }
        }

        #region methods
        /// <summary>
        /// Accessor method for the positive modifiers applied by this shape
        /// </summary>
        /// <returns>A list of tokens representing the positive modifiers applied by this shape</returns>
        public List<ParamToken> getPosMods()
        {
            return posMods;
        }

        /// <summary>
        /// Accessor method for the negative modifiers applied by this shape
        /// </summary>
        /// <returns>A list of tokens representing the negative modifiers applied by this shape</returns>
        public List<ParamToken> getNegMods()
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

        /// <summary>
        /// Returns the width of the shape
        /// </summary>
        /// <returns>The size of a row of the shape</returns>
        public int sizeRow()
        {
            return cells.Length;
        }

        /// <summary>
        /// Return the height of the shape
        /// </summary>
        /// <returns>Returns the size of column 0 of the shape</returns>
        public int sizeCol()
        {
            return cells[0].Length;
        }

        #endregion
    }
}
