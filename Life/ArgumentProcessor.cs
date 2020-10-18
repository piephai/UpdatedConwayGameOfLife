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
                        case "--input":
                            ProcessInputFile(args, i, options);
                            break;
                        case "--periodic":
                            options.Periodic = true;
                            break;
                        case "--step":
                            options.StepMode = true;
                            break;
                        case "--neighbour":
                            ProcessNeighbour(args, i, options);
                            break;
                        case "--survival":
                            ProcessSurvival(args, i, options);
                            break;
                        case "--birth":
                            ProcessBirth(args, i, options);
                            break;
                        case "--memory":
                            ProcessGenerationalMemory(args, i, options);
                            break;
                        case "--output":
                            ProcessOutputFile(args, i, options);
                            break;
                        case "--ghost":
                            ProcessGhostMode(args, i, options);
                            break;

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
            ValidateParameterCount(args, i, "input", 1);

            options.InputFile = args[i + 1];
        }

        private static void ProcessNeighbour(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "neighbour", 3);

            if (String.IsNullOrEmpty(args[i + 1]))
            {
                throw new ArgumentException($"The input: \'{args[i + 1]}\' is not a valid parameter. " +
                    $"The neighbourhood type must either be moore or vonNeumann.");
            }
            options.NeighbourhoodType = args[i + 1];

            if (!int.TryParse(args[i + 2], out int neighbourOrder))
            {
                throw new ArgumentException($"Neighbour order \'{args[i + 2]}\' is not a valid integer.");
            }
            options.NeighbourOrder = neighbourOrder;

            if (String.IsNullOrEmpty(args[i + 3]))
            {
                throw new ArgumentException($"Neighbourhood must specify whether the centre is counted" +
                    $" by including 'true' or 'false' parameter");
            }

            if (!String.IsNullOrEmpty(args[i + 3]))
            {
                string trueString = "true";
                string falseString = "false";
                string parameterToCompare = args[i + 3];

                if (String.Equals(parameterToCompare, trueString, StringComparison.CurrentCultureIgnoreCase))
                {
                    options.CentreCount = true;
                }
                else if (String.Equals(parameterToCompare, falseString, StringComparison.CurrentCultureIgnoreCase))
                {
                    options.CentreCount = false;
                }
                else
                {
                    throw new ArgumentException($"The neighbourhood parameter for <centre-count> must be 'true' or 'false'");
                }
            }
        }

        private static void ProcessSurvival(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "survival", 1);

            List <int> survivalInput = new List<int>();
            List<int> survivalRate = new List<int>();
            int indexOfElipse = 0;
            bool containElipse = false;

            for (int param = 0; param < args.Length; param++)
            {
                if (args[i + param] != "...")
                {
                    if (!int.TryParse(args[i + param], out int survivalNum))
                    {
                        throw new ArgumentException($"The supplied survival parameter is not a valid integer.");
                    }
                    survivalInput.Add(survivalNum);
                }
                else
                {
                    indexOfElipse = param;
                    containElipse = true;
                }
            }
            if (containElipse)
            {
                survivalRate = ElipseLoop(survivalInput, indexOfElipse);
            }
            else
            {
                survivalRate = survivalInput;
            }
            options.SurvivalRate = survivalRate;
        }

        private static void ProcessBirth(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "birth", 1);

            List<int> birthInput = new List<int>();
            List<int> birthRate = new List<int>();
            int indexOfElipse = 0;
            bool containElipse = false;

            for (int param = 0; param < args.Length; param++)
            {
                if (args[i + param] != "...")
                {
                    if (!int.TryParse(args[i + param], out int birthNum))
                    {
                        throw new ArgumentException($"The supplied birth parameter is not a valid integer.");
                    }
                    birthInput.Add(birthNum);
                }
                else
                {
                    indexOfElipse = param;
                    containElipse = true;
                }
            }
            if (containElipse)
            {
                birthRate = ElipseLoop(birthInput, indexOfElipse);
            }
            else
            {
                birthRate = birthInput;
            }
            options.SurvivalRate = birthRate;
        }


        private static void ProcessGenerationalMemory(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "memory", 1);
            if (!int.TryParse(args[i + 1], out int memory))
            {
                throw new ArgumentException($"Generational memory input: \'{args[i + 1]}\' is not a valid integer.");
            }
            options.GenerationalMemory = memory;
        }

        private static void ProcessGhostMode(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "ghost", 0);
            options.GhostMode = true;
        }

        private static void ProcessOutputFile(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "output", 1);

            options.OutputFile = args[i + 1];
        }

        private static void ValidateParameterCount(string[] args, int i, string option, int numParameters)
        {
            if (i >= args.Length - numParameters)
            {
                throw new ArgumentException($"Insufficient parameters for \'--{option}\' option " +
                    $"(provided {args.Length - i - 1}, expected {numParameters})");
            }

        }

        private static List<int> ElipseLoop(List<int> initialList, int indexOfElipse)
        {
            List<int> outputList = new List<int>();
            int startElement = initialList[indexOfElipse - 1];
            int endElement = initialList[indexOfElipse];

            foreach (int element in initialList)
            {
                if (element != startElement || element != endElement)
                {
                    outputList.Add(element);
                }
            }
            for (int currentElement = startElement; currentElement <= endElement; currentElement++)
            {
                outputList.Add(currentElement);
            }
            return outputList;
             

        }

    }
}
