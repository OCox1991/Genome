﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Genome
{
    class Gene
    {
        private Cell[][] cells;
        private static List<Shape> recognisedShapes;
        private List<ParamToken> posMods;
        private List<ParamToken> negMods;
        private int[] colourCount;
        private const int cellsX = 10;
        private const int cellsY = 10;
        private static Random random;
        private Texture2D texture;

        /// <summary>
        /// The empty constructor for the gene, which produces a random array of cells.
        /// </summary>
        public Gene()
        {
            random = WorldState.RandomNumberGenerator;
            generateCells();
            this.init();
        }

        public Gene(Random rand)
        {
            random = rand;
            generateCells();
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
        /// An overloaded constructor to allow the specification of both the random number generator and the list of cells
        /// </summary>
        /// <param name="cells">The cells to use</param>
        /// <param name="rand">The random number generator</param>
        public Gene(Cell[][] cells, Random rand)
        {
            random = rand;
            this.cells = cells;
            this.init();
        }

        private void generateCells()
        {
            cells = new Cell[cellsX][]; //Using jagged arrays here to improve extensibility
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell[cellsY];
                for (int j = 0; j < cells[i].Length; j++)
                {
                    int c = random.Next(7);
                    cells[i][j] = new Cell(c, c);
                }
            }
        }

        /// <summary>
        /// init handles the initialisation code that is shared between the different constructors
        /// </summary>
        private void init()
        {
            recognisedShapes = Simulation.getShapes();
            posMods = new List<ParamToken>();
            negMods = new List<ParamToken>();
            foreach(Shape s in recognisedShapes)
            {
                patternMatch(s);
            }
            countColours();

#if DEBUG
            Console.WriteLine();
            Console.WriteLine();
            foreach (string s in toStrings())
            {
                Console.WriteLine(s);
            }

            for (int i = 0; i < colourCount.Length; i++)
            {
                Console.WriteLine(String.Concat(new String[] { "Number of ", i.ToString(), "s: ", colourCount[i].ToString() }));
            }
#endif
            texture = Display.drawGenome(this);
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
                    int mutationChance = Simulation.getMutationChance();
                    if (random.Next(mutationChance) == 1)
                    {
                        newCells[i][j] = new Cell(random.Next(7), random.Next(7));
                    }
                    else
                    {
                        int colour1 = this.getPart(i, j);
                        int colour2 = otherGene.getPart(i, j);
                        newCells[i][j] = new Cell(colour1, colour2);
                    }
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
            for (int i = 0; i <= cells.Length - s.sizeRow(); i++)
            {
                for (int j = 0; j <= cells[i].Length - s.sizeCol(); j++)
                {
                    if (cellMatch(s, i, j))
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
        private bool cellMatch(Shape s, int row, int col)
        {
            Cell[][] checkCells = new Cell[s.sizeRow()][];
            for (int i = 0; i < s.sizeRow(); i++)
            {
                checkCells[i] = new Cell[s.sizeCol()];
                for (int j = 0; j < s.sizeCol(); j++)
                {
                    checkCells[i][j] = cells[i + row][j + col];
                }
            }
            bool isMatch = true;
            int sizeCol = s.sizeCol();
            int sizeRow = s.sizeRow();

            for (int compCol = 0; compCol < sizeCol; compCol++)
            {
                for (int compRow = 0; compRow < sizeRow; compRow++)
                {
                    if(s.getColour(compRow, compCol) == -1)
                    {
                        //do nothing, -1 represents the wildcard
                    }
                    else if(s.getColour(compRow, compCol) == checkCells[compRow][compCol].getDomColour())
                    {
                        //do nothing, the cells match
                    }
                    else
                    {
                        isMatch = false; //Otherwise they don't match so the shape doesn't match at this location
                    }
                }
            }
#if DEBUG
            if (isMatch)
            {
            }
#endif
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
            Queue<int> coloursInOrderOfAppearance = new Queue<int>();
            if (row < 1 || row > cells.Length - 1 || col < 1 || col > cells[row].Length - 1) //if we are in the first or last row or column
            {
                //do nothing, maxColour will be returned to give the value -1;
            }
            else
            {
                int[] results = new int[7];
                for (int i = -1; i <= 1; i++) //this loop produces an array where the number of each colour is mapped to the number of times it occurs
                {
                    for (int j = -1; j < 2; j++) //look at 0, 1, 2 (-1, 0, 1)
                    {
                        int lookupRow = row + i;
                        int lookupCol = col + j;
                        int colour = getColour(lookupRow, lookupCol);
                        results[colour]++;
                        coloursInOrderOfAppearance.Enqueue(colour);
                    }
                }
                
                int maxVal = 0; //the maximum value found so far
                bool[] tieColours = new Boolean[7]; //used to keep track of what colours are part of the tie
                bool tie = true; //used to keep track of if a tie is occuring
                for (int i = 0; i < results.Length; i++)//finding the max occurences in the results ie. max(results)
                {
                    if (results[i] > maxVal || maxColour == -1) //if a better max has been found OR no max colour currently exists
                    {
                        if (tie) //if something is > maxVal then any existing tie has been broken
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

                //code to deal with ties, uses the queue set up earlier

                while (tie)
                {
                    int nextCol = coloursInOrderOfAppearance.Dequeue();
                    if (tieColours[nextCol])
                    {
                        tie = false;
                        maxColour = nextCol;
                    }
                }
            }
            return maxColour;
        }

        /// <summary>
        /// Counts the occurences of each colour in the genome, used for setting diet and colour for the creature
        /// </summary>
        public void countColours()
        {
            colourCount = new int[7]; //meaning it contains the numbers 0 to 6;
            for (int r = 0; r < cells.Length; r++)
            {
                for (int c = 0; c < cells[r].Length; c++)
                {
                    colourCount[cells[r][c].getDomColour()]++; //increment the index of the array associated with the colour
                }
            }
        }

        /// <summary>
        /// Gets the width of the gene
        /// </summary>
        /// <returns>The width of the gene as an int</returns>
        public int getSizeX()
        {
            return cells.Length;
        }

        /// <summary>
        /// Gets the height of the gene
        /// </summary>
        /// <returns>The height of the gene as an int</returns>
        public int getSizeY()
        {
            return cells[0].Length;
        }

        /// <summary>
        /// Turns the gene into an array of strings, with each string being a row of the numbers of the gene. Used for debugging
        /// </summary>
        /// <returns></returns>
        public String[] toStrings()
        {
            String[] s = new String[cells[0].Length];
            for (int i = 0; i < cells.Length; i++)
            {
                s[i] = "";
                for (int j = 0; j < cells[i].Length; j++)
                {
                    s[i] = String.Concat(new String[]{s[i], cells[i][j].getDomColour().ToString()});
                }
            }
            return s;
        }

        /// <summary>
        /// Returns the texture that the gene has stored
        /// </summary>
        /// <returns>The texture representing using the colour map to create the correct colours</returns>
        public Texture2D getTexture()
        {
            return texture;
        }

        /// <summary>
        /// Gets the colour count array, which contains the amount of each colour in the gene
        /// </summary>
        /// <returns>The colour count array, an array of ints where each index points to an int representing the number of times that colour appears in the gene</returns>
        public int[] getColourCount()
        {
            return colourCount;
        }

        #endregion
    }
}
