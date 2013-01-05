using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Gene
    {
        Cell[][] cells;
        Shape[] recognisedShapes;

        /// <summary>
        /// The empty constructor for the gene, which produces a random array of cells.
        /// </summary>
        public Gene()
        {
            cells = new Cell[10][]; //Using jagged arrays here to improve extensibility
            Random r = new Random();
            for(int i = 1; i < cells.Length; i++)
            {
                cells[i] = new Cell[10];
                for(int j = 1; j < cells[i].Length; j++)
                {
                    cells[i][j] = new Cell(r.Next(7), r.Next(7));
                }
            }
            this.init();
        }

        /// <summary>
        /// An overloaded constructor for the gene that takes an array of cells. Used when breeding.
        /// </summary>
        /// <param name="cells">A 2D array of cells that represent this Genome</param>
        public Gene(Cell[][] cells)
        {
            this.cells = cells;
            this.init();
        }

        /// <summary>
        /// init handles the initialisation code that is shared between the different constructors
        /// </summary>
        private void init()
        {
            recognisedShapes = Simulation.getShapes();
        }

        #region Methods

        /// <summary>
        /// A method to breed this gene with another gene
        /// </summary>
        /// <param name="otherGene">The gene to breed this gene with</param>
        /// <returns>The result of breeding these two genes together</returns>
        public Gene breedWith(Gene otherGene)
        {
            Cell[][] newCells = new Cell[10][];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = new Cell[10];
                for (int j = 0; j < cells.Length; j++)
                {
                    int colour1 = this.getPart(i,j);
                    int colour2 = otherGene.getPart(i,j);
                    newCells[i][j] = new Cell(colour1, colour2);
                }
            }
            return new Gene(newCells);
        }

        /// <summary>
        /// Gets a random part of a given cell
        /// </summary>
        /// <param name="row">the row of the cell to look at</param>
        /// <param name="col">the column of the cell to look at</param>
        /// <returns>a random part of the specified cell</returns>
        public int getPart(int row, int col)
        {
            return cells[row][col].getRandomPart();
        }

        #endregion
    }
}
