﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Gene
    {
        private Cell[][] cells;
        private static List<Shape> recognisedShapes;
        private List<ParamToken> posMods;
        private List<ParamToken> negMods;
        private int[] colourCount;

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
                    int c = r.Next(7);
                    cells[i][j] = new Cell(c, c);
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

            foreach(Shape s in recognisedShapes)
            {
                patternMatch(s);
            }
            countColours();
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
            return getCell(row, col).getRandomPart();
        }


        /// <summary>
        /// Returns the dominant colour of a specified cell
        /// </summary>
        /// <param name="row">the row in which to find the cell</param>
        /// <param name="col">the column in which to find the cell</param>
        /// <returns></returns>
        public int getColour(int row, int col)
        {
            return getCell(row, col).getDomColour();
        }

        /// <summary>
        /// Gets a specified cell
        /// </summary>
        /// <param name="row">The row to find the cell in</param>
        /// <param name="col">The column to find the cell in</param>
        /// <returns>The cell found at the specified location</returns>
        public Cell getCell(int row, int col)
        {
            return cells[row][col];
        }

        public List<ParamToken> getPosMods()
        {
            return posMods;
        }

        public List<ParamToken> getNegMods()
        {
            return negMods;
        }

        /// <summary>
        /// Matches a given shape across the entire genome, adding the necessary posMods and negMods
        /// </summary>
        /// <param name="s">The shape to match across the whole genome</param>
        private void patternMatch(Shape s)
        {
            for(int row = 0; row < cells.Length - (s.sizeRow() - 1); row++)
            {
                for (int col = 0; col < cells.Length - (s.sizeCol() - 1); col++)
                {
                    //add some check with Shape size here
                    if (cellMatch(s, row, col))
                    {
                        posMods.AddRange(s.getPosMods());
                        negMods.AddRange(s.getNegMods());
                    }
                }
            }
        }

        /// <summary>
        /// Checks if some shape matches at a given location. This is done by checking the next few rows and columns from the
        /// given location and checking they all match the shape.
        /// </summary>
        /// <param name="s">The shape to check</param>
        /// <param name="row">The row of the top left cell to look at</param>
        /// <param name="col">The column of the top left cell to look at</param>
        /// <returns>True if all cells match the given colours of the shape and false otherwise. The colour -1 is used to represent a wildcard</returns>
        private Boolean cellMatch(Shape s, int row, int col)
        {
            Boolean isMatch = true;
            if (row > cells.Length - 2 || col > cells.Length - 2) //if we have gone far enough along that the matching would go off the edge
            {
                isMatch = false;
            }

            int i = 0;
            int j = 0;
            while(i < 3 && isMatch == true) //using while loops here to allow early termination if it doesn't match
            {
                while(j < 3 && isMatch == true)
                {
                    if (s.getColour(i,j) != getColour(i,j) && s.getColour(i,j) != -1)
                    {
                        isMatch = false;
                    }
                    j++;
                }
                i++;
            }
            return isMatch;
        }

        /// <summary>
        /// The poll method looks at the cells surrounding the given cell, as well as the given cell itself and returns the
        /// majority colour of these 9 cells.
        /// </summary>
        /// <param name="row">The row index of the given cell</param>
        /// <param name="col">The column index of the given cell</param>
        /// <returns>The colour in the majority at the given location, -1 if an invalid location was entered and the first occuring tied colour
        /// (going left to right, top to bottom) in the case of a tie</returns>
        public int poll(int row, int col)
        {
            int maxColour = -1;
            if (row < 1 || row > cells.Length || col < 1 || col > cells[row].Length) //if we are in the first or last row or column
            {
                //do nothing, maxColour will be returned to give the value -1;
            }
            else
            {
                int[] results = new int[8];
                for (int i = 0; i < 3; i++) //this loop produces an array where the number of each colour is mapped to the number of times it occurs
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int lookupRow = row + (i-1);
                        int lookupCol = col + (j-1);
                        results[getColour(lookupRow, lookupCol)]++;
                    }
                }
                
                int maxVal = 0; //the maximum value found so far
                Boolean[] tieColours = new Boolean[8]; //used to keep track of what colours are part of the tie
                Boolean tie = true; //used to keep track of if a tie is occuring
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i] > maxVal) //if a better max has been found
                    {
                        if (tie) //will break any ties that exist
                        {
                            for (int iter = 1; iter < tieColours.Length; iter++ )
                            {
                                tieColours[iter] = false; //this will always be called on the first loop to make sure these are all the correct value
                            }
                            tie = false;
                        }

                        maxVal = results[i]; //update the max occurences value
                        maxColour = i; //update which colour is winning
                    }
                    else if(results[i] == maxVal)
                    {
                        tieColours[maxColour] = true; //set the colours that are tieing to be true
                        tieColours[i] = true;
                        tie = true; //set the fact that a tie is occuring to be true
                    }
                }
                //on loop end: maxVal > 0, if tie == false all elements of tieColours are also false

                //code to deal with ties
                int k = 0;
                int l = 0;
                while(tie && k < 3)
                {
                    while(tie && l < 3)
                    {
                        int lookupRow = row + (k - 1);
                        int lookupCol = col + (l - 1);
                        int colour = getColour(lookupRow, lookupCol);

                        if (tieColours[colour]) //then we've found the topmost leftmost colour out of all the tied colours
                        {
                            maxColour = colour;
                            tie = false;
                        }

                        k++; //otherwise look at the next cell
                    }
                    l++;
                }
            }
            return maxColour;
        }

        /// <summary>
        /// Counts the occurences of each colour in the genome, used for setting diet and colour for the creature
        /// </summary>
        public void countColours()
        {
            colourCount = new int[8]; //meaning it contains the numbers 1 to 7;
            for (int r = 0; r < cells.Length; r++)
            {
                for (int c = 0; c < cells[r].Length; c++)
                {
                    colourCount[cells[r][c].getDomColour()]++; //increment the index of the array associated with the colour
                }
            }
        }

        public int getSizeX()
        {
            return cells.Length;
        }

        public int getSizeY()
        {
            return cells[0].Length;
        }

        #endregion
    }
}
