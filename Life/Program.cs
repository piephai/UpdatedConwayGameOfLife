using System;
using System.Diagnostics;
using System.IO;
using Display;

namespace Life
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = ArgumentProcessor.Process(args);
            int[,] universe = InitializeUniverse(options);
            Grid grid = new Grid(options.Rows, options.Columns);

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
                    universe = EvolveUniverse(universe, options.Periodic, options.NeighbourOrder, options.NeighbourhoodType, options.CentreCount);
                }

                UpdateGrid(grid, universe);

                grid.SetFootnote($"Generation: {iteration++}");
                grid.Render();

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
            WaitSpacebar();

            grid.RevertWindow();

            Logging.Message("Press spacebar to exit program...");
            WaitSpacebar();
        }

        private static int[,] EvolveUniverse(int[,] universe, bool periodic, int order, string neighbourHoodConditions, bool centreCount)
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

                    if (universe[i, j] == ALIVE && (neighbours == 2 || neighbours == 3))
                    {
                        
                        buffer[i, j] = ALIVE;
                    }
                    else if (universe[i, j] == DEAD && neighbours == 3)
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
            if (!periodic && neighbourHoodConditions == "moore")
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount)
                        {
                            if ((r != i || c != j) && r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                                neighbours += universe[r, c];
                            }
                        }
                        else
                        {
                            if (r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                                neighbours += universe[r, c];
                            }
                        }
                    }
                }
            }
            //Periodic moore neighbourhood
            else if (periodic && neighbourHoodConditions == "moore")
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount)
                        {
                            if (r != i || c != j)
                            {
                                neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                            }
                        }
                        else
                        {
                            neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                        }
                    }
                }
            }

            //Von Neumann not accounting for the furthest corners, non-periodic
            //Incomplete
            else if (!periodic && neighbourHoodConditions == "vonNeumann")
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount)
                        {
                            if ((r != i || c != j) && r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                              
                                
                                if (r % 1 == -1 || r % 1 == 1 && c % 1 != -1 || c % 1 != 1)
                                {
                                    neighbours += universe[r, c];
                                }
                                if (r % 0 == 0)
                            }
                        }
                        else
                        {
                            if (r >= 0 && r < rows && c >= 0 && c < columns)
                            {
                                neighbours += universe[r, c];
                            }
                        }
                    }
                }
            }

            //Von Neumann periodic
            //Incomplete
            else
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (!centreCount)
                        {
                            if (r != i || c != j)
                            {
                                neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                            }
                        }
                        else
                        {
                            neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                        }
                    }
                }
            }
            

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
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    string[] elements = line.Split(" ");

                    int row = int.Parse(elements[0]);
                    int column = int.Parse(elements[1]);

                    universe[row, column] = 1;
                }
            }

            return universe;
        }
    }
}
