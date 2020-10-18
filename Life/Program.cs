using System;
using System.Diagnostics;
using System.IO;
using Display;
using System.Linq;
using System.Collections.Generic;

namespace Life
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Options options = ArgumentProcessor.Process(args);
            int[,] universe = InitializeUniverse(options);
            Grid grid = new Grid(options.Rows, options.Columns);
            var universeList = new List<int[,]>();
            bool isSteadyState = false;
            bool successfullyWroteToFile = false;



            Logging.Message("Press spacebar to begin the game...");
            WaitSpacebar();

            grid.InitializeWindow();
           

            Stopwatch stopwatch = new Stopwatch();

            int iteration = 0;
            while (iteration <= options.Generations)
            {
                
                stopwatch.Restart();

                if (iteration != 0)
                {
                    universe = EvolveUniverse(universe, options.Periodic, options.NeighbourOrder, options.NeighbourhoodType, options.CentreCount, options.BirthRate, options.SurvivalRate);
                }
                
                if (universeList.Count == options.GenerationalMemory) //Remove the first item in the list if the list capacity is reached (Capacity is the generationalMemory)
                {
                    universeList.RemoveAt(0);

                }

                UpdateGrid(grid, universe);

                grid.SetFootnote($"Generation: {iteration++}");
                grid.Render();
                isSteadyState = CheckSteadyState(universeList, options, universe); //Check if the current universe have reached a steady state
                if (isSteadyState)
                { 
                    break;
                }
                universeList.Add(universe);

                if (options.StepMode)
                {
                    WaitSpacebar();
                }
                else
                {
                    while (stopwatch.ElapsedMilliseconds < 1000 / options.UpdateRate) ;
                }
            }

            grid.IsComplete = true;
            grid.Render();

            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                using StreamWriter writer = new StreamWriter(options.OutputFile);
                writer.WriteLine("#version=2.0");
                OutputToFile(options, universeList, writer);
                writer.Close();
                successfullyWroteToFile = true;
            }
            WaitSpacebar();

            grid.RevertWindow();

            
            if (isSteadyState && options.IsAllDead == false)
            {
                Logging.Message($"Steady-state detected... periodicity = {iteration - 1}");
            }
            if (isSteadyState && options.IsAllDead == true)
            {
                Logging.Message($"Steady-state detected... periodicity = N/A");
            }
            if (successfullyWroteToFile)
            {
                Logging.Success($": Final generation written to file: {options.OutputFile}");
            }
            Logging.Message("Press spacebar to exit program...");
            WaitSpacebar();
        }
        private static bool CheckSteadyState (List<int[,]> universeList, Options options, int[,] universe)
        {
            if (universeList.Count > 0) //Only check if universeList already have at least an element in it
            {
                for (int i = 0; i < universeList.Count; i++)
                {
                    int steadyStateCounter = 0;
                    int isDeadState = 0; 
                    for (int row = 0; row < options.Rows; row++)
                    {
                        for (int column = 0; column < options.Columns; column++)
                        {
                            if (universeList[i][row, column] == universe[row, column]) //Check if a grid from universe list matches that of the current universe
                            {
                                if (universe[row, column] == 0)
                                {
                                    isDeadState++;
                                }
                                steadyStateCounter++;
                            }                      
                        }
                    }
                    if (steadyStateCounter == options.Rows * options.Columns) //If it a grid from universeList matches perfectly with the current universe then steadyStateCounter will be equal to rows * columns
                    {
                        if (isDeadState == options.Rows * options.Columns)
                        {
                            options.IsAllDead = true;
                            return true;
                        }
                        return true;
                        
                    }
                    
                }
            }
            return false;
            
        }

        //private static StreamWriter InitialiseOutputFile (Options options)
        //{
            
        //    return writer;
        //}

        private static void OutputToFile (Options options, List<int[,]> universeList, StreamWriter writer)
        {
            for (int row = 0; row < options.Rows; row++)
            {
                for (int column = 0; column < options.Columns; column++)
                {
                    //writer.WriteLine(universeList[universeList.Count - 1][row, column]);

                    if (universeList[universeList.Count - 1][row, column] == 1)
                    {
                        writer.WriteLine("(o) cell: " + row + ", " + column);
                    }

                    else if (universeList[universeList.Count - 1][row, column] == 0)
                    {
                        writer.WriteLine("(x) cell: " + row + ", " + column);
                    }
                }
            } 

            
        }

 
        private static int[,] EvolveUniverse(int[,] universe, bool periodic, int order, string neighbourHoodConditions, bool centreCount, List<int> birthRate, List<int> survivalRate)
        {
            const int ALIVE = 1;
            const int DEAD = 0;
          

            int rows = universe.GetLength(0);
            int columns = universe.GetLength(1);

            int[,] buffer = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int neighbours = CountNeighbours(universe, i, j, periodic, order, neighbourHoodConditions, centreCount);

                    if (universe[i, j] == ALIVE && survivalRate.Contains(neighbours)) //Check if survival list contain the condition for a cell to stay alive (if it contains the number of allowed alive neighbouring cells)
                    {
                       
                        buffer[i, j] = ALIVE;
  
                       
                    }
                    else if (universe[i, j] == DEAD && birthRate.Contains(neighbours)) //Check if birthrate list contain the condition for a cell to become alive (if it contains the number of specified alive neighbouring cells
                    {
                        
                        buffer[i, j] = ALIVE;
                
                        
                    }
                    else
                    {
                        buffer[i, j] = DEAD;
                    }
                }
            }
            return buffer.Clone() as int[,];
        }

        private static int CountNeighbours(int[,] universe, int i, int j, bool periodic, int order, string neighbourHoodConditions, bool centreCount)
        {
            int rows = universe.GetLength(0);
            int columns = universe.GetLength(1);

            int neighbours = 0;
            
            //Non-periodic moore neighbourhood
            if (!periodic && String.Equals(neighbourHoodConditions, "moore", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount) //Accounting all neighbouring cells but not itself
                        {
                            if ((r != i || c != j) && r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                                if (universe[r, c] == 1)
                                {
                                    neighbours += universe[r, c];
                                }
                            }
                        }
                        else
                        {
                            //Accounting all neighbouring cells and itself
                            if (r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                                if (universe[r, c] == 1)
                                {
                                    neighbours += universe[r, c];
                                }
                            }
                        }
                    }
                }
            }
            //Periodic moore neighbourhood
            else if (periodic && String.Equals(neighbourHoodConditions, "moore", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount) //Accounting all neighbouring cells but not itself periodically
                        {

                            if (r != i || c != j)
                            {
                                neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                            }
                        }
                        else
                        {
                            //Accounting all neighbouring cells and itself periodically
                            neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                        }
                    }
                }
            }

            //Von Neumann not accounting for the furthest corners, non-periodic
            //Incomplete
            //else if (!periodic && neighbourHoodConditions == "vonNeumann")
            //{
            //    for (int r = i - order; r <= i + order; r++)
            //    {
            //        for (int c = j - order; c <= j + order; c++)
            //        {
            //            if (!centreCount)
            //            {
            //                if ((r != i || c != j) && r >= 0 && r < rows && c >= 0 && c < columns)
            //                {
                              
                                
            //                    if (r % 1 == -1 || r % 1 == 1 && c % 1 != -1 || c % 1 != 1)
            //                    {
            //                        neighbours += universe[r, c];
            //                    }
            //                    if (r % 0 == 0)
            //                }
            //            }
            //            else
            //            {
            //                if (r >= 0 && r < rows && c >= 0 && c < columns)
            //                {
            //                    neighbours += universe[r, c];
            //                }
            //            }
            //        }
            //    }
            //}

            //Von Neumann periodic
            //Incomplete
            //else
            //{
            //    for (int r = i - order; r <= i + order; r++)
            //    {
            //        for (int c = j - order; c <= j + order; c++)
            //        {
            //            if (!centreCount)
            //            {
            //                if (r != i || c != j)
            //                {
            //                    neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
            //                }
            //            }
            //            else
            //            {
            //                neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
            //            }
            //        }
            //    }
            //}
            

            return neighbours;
        }

        // "Borrowed" from: https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        private static int Modulus(int x, int m)
        {
           return (x % m + m) % m;
        }

        private static void UpdateGrid(Grid grid, int[,] universe)
        {
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    grid.UpdateCell(i, j, (CellState)universe[i, j]);
                }
            }
        }

        private static void WaitSpacebar()
        {
            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) ;
        }


        private static int[,] InitializeUniverse(Options options)
        {
            int[,] universe;

            if (options.InputFile == null)
            {
                universe = InitializeFromRandom(options.Rows, options.Columns, options.RandomFactor);
            }
            else
            {
                try
                {
                    universe = InitializeFromFile(options.Rows, options.Columns, options.InputFile);
                }
                catch 
                {
                    Logging.Warning($"Error initializing universe using \'{options.InputFile}\'. Reverting to randomised universe...");
                    universe = InitializeFromRandom(options.Rows, options.Columns, options.RandomFactor);
                }
            }

            return universe;
        }

        private static int[,] InitializeFromRandom(int rows, int columns, double randomFactor)
        {
            int[,] universe = new int[rows, columns];

            Random random = new Random();
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = random.NextDouble() < randomFactor ? 1 : 0;
                }
            }

            return universe;
        }
        
        private static int[,] InitializeFromFile(int rows, int columns, string inputFile)
        {
            int[,] universe = new int[rows, columns];

            using (StreamReader reader = new StreamReader(inputFile))
            {
                string line = reader.ReadLine();
                if (line == "#version=1.0")
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        string[] elements = line.Split(" ");

                        int row = int.Parse(elements[0]);
                        int column = int.Parse(elements[1]);

                        universe[row, column] = 1;
                    }
                   
                }

                else if (line == "#version=2.0")
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        string[] elements = line.Split(" ");

                        if (!(line.Contains("rectangle") || line.Contains("ellipse")))
                        {
                            if (line.Contains("(o)")) //Check if the line of text is specifying the cell to be alive (o)
                            {
                                elements[2] = elements[2].Replace(",", ""); //Replace all the commas (Read only so the file will not be changed)
                                int row = int.Parse(elements[2]);
                                int column = int.Parse(elements[3]);
                                universe[row, column] = 1;
                            }
                            else if (line.Contains("(x)")) //Check if the line of text is specifying the cell to be dead (x)
                            {
                                elements[2] = elements[2].Replace(",", "");
                                int row = int.Parse(elements[2]);
                                int column = int.Parse(elements[3]);
                                universe[row, column] = 0;
                            }
                        }
                        else
                        {
                            if (line.Contains("(o)")) 
                            {
                                ProcessVersion2ShapeCells(elements, universe, true);
                            }
                            else if (line.Contains("(x)")) 
                            {
                                ProcessVersion2ShapeCells(elements, universe, false);
                            }
                        }
                        
                    }
                }
            }

            return universe;
        }

        //Process .seed file which are version 2.0 and has a shape (rectangle or ellipse)
        private static void ProcessVersion2ShapeCells(string[] elements, int[,] universe, bool isAlive)
        {
            //elements[2] = elements[2].Replace(",", "");  
            for (int i = 0; i < elements.Length; i++)
            {  //Replace all the commas (Read only so the file will not be changed)
                elements[i] = elements[i].Replace(",", "");
            }


            int rowBottomLeft = int.Parse(elements[3]);
            int colBottomLeft = int.Parse(elements[4]);
            int rowTopRight = int.Parse(elements[5]);
            int colTopRight = int.Parse(elements[6]);

            //For rectangle cell structure
            if (elements.Contains("rectangle"))
            {
                for (int row = rowBottomLeft; row <= rowTopRight; row++)
                {
                    for (int col = colBottomLeft; col <= colTopRight; col++)
                    {
                        if (isAlive)
                        {
                            universe[row, col] = 1;
                        }
                        else
                        {
                            universe[row, col] = 0;
                        }
                    }
                }
            }
            else
            {
                int centreX = (colBottomLeft + colTopRight) / 2;
                int centreY = (rowBottomLeft + rowTopRight) / 2;
                for (int row = rowBottomLeft; row <= rowTopRight; row++)
                {
                    for (int col = colBottomLeft; col <= colTopRight; col++)
                    {
                        if ((((4*(row - centreY)^2)/rowTopRight) + ((4*(col - centreX)^2)/colTopRight)) <= 1)
                        {
                            if (isAlive)
                            {
                                universe[row, col] = 1;
                            }
                            else
                            {
                                universe[row, col] = 0;
                            }
                        }

                        
                    }
                }
            }

        }
            //For ellipse cell structure        
    }
}
