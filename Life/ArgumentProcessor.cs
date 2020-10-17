using System;
using System.Collections.Generic;
using System.Text;

namespace Life
{
    static class ArgumentProcessor
    {

        public static Options Process(string[] args)
        {
            Options options = new Options();
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--dimensions":
                            ProcessDimensions(args, i, options);
                            break;
                        case "--generations":
                            ProcessGenerations(args, i, options);
                            break;
                        case "--max-update":
                            ProcessUpdateRate(args, i, options);
                            break;
                        case "--random":
                            ProcessRandomFactor(args, i, options);
                            break;
                        case "--seed":
                            ProcessInputFile(args, i, options);
                            break;
                        case "--periodic":
                            options.Periodic = true;
                            break;
                        case "--step":
                            options.StepMode = true;
                            break;

                        case "neighbour":


                    }
                }
                Logging.Success("Command line arguments processed without issue!");
            }
            catch (Exception exception)
            {
                Logging.Warning(exception.Message);
                Logging.Message("Reverting to defaults for unprocessed arguments...");
            }
            finally
            {
                Logging.Message("The following options will be used:");
                Console.WriteLine(options);
            }

            return options;
        }

        private static void ProcessDimensions(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "dimensions", 2);

            if (!int.TryParse(args[i + 1], out int rows))
            {
                throw new ArgumentException($"Row dimension \'{args[i + 1]}\' is not a valid integer.");
            }

            if (!int.TryParse(args[i + 2], out int columns))
            {
                throw new ArgumentException($"Column dimension \'{args[i + 2]}\' is not a valid integer.");
            }

            options.Rows = rows;
            options.Columns = columns;
        }

        private static void ProcessGenerations(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "generations", 1);

            if (!int.TryParse(args[i + 1], out int generations))
            {
                throw new ArgumentException($"Generation count \'{args[i + 1]}\' is not a valid integer.");
            }

            options.Generations = generations;
        }

        private static void ProcessUpdateRate(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "max-update", 1);

            if (!double.TryParse(args[i + 1], out double updateRate))
            {
                throw new ArgumentException($"Update rate \'{args[i + 1]}\' is not a valid double.");
            }

            options.UpdateRate = updateRate;
        }

        private static void ProcessRandomFactor(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "random", 1);

            if (!double.TryParse(args[i + 1], out double randomFactor))
            {
                throw new ArgumentException($"Random factor \'{args[i + 1]}\' is not a valid double.");
            }

            options.RandomFactor = randomFactor;
        }

        private static void ProcessInputFile(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "seed", 1);

            options.InputFile = args[i + 1];
        }

        // MY CHANGES

        private static void ProcessNeighbour(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "neighbour", 3);

            if(!int.TryParse(args[i + 2], out int neighbourOrder))
            {
                throw new ArgumentException($"Neighbour order \'{args[i + 2]}\' is not a valid integer.");
            }
            options.NeighbourOrder = neighbourOrder;

            if (String.IsNullOrEmpty(args[i + 1]))
            {
                throw new ArgumentException($"The input: \'{args[i + 1]}\' is not a valid parameter. " +
                    $"The neighbourhood type must either be moore or vonNeumann.");
            }
            options.NeighbourhoodType = args[i + 3];

            if (String.IsNullOrEmpty(args[i + 3]))
            {
                throw new ArgumentException($"The input: \'{args[i + 1]}\' is not a valid parameter. " +
                    $"The neighbour centre must either be moore or vonNeumann.");
            }
            options.CentreCount = args[i + 3];

        }

        private static void ValidateParameterCount(string[] args, int i, string option, int numParameters)
        {
            if (i >= args.Length - numParameters)
            {
                throw new ArgumentException($"Insufficient parameters for \'--{option}\' option " +
                    $"(provided {args.Length - i - 1}, expected {numParameters})");
            }
        }
    }
}
