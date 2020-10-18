using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
                            ProcessPeriodic(args, i, options);
                            break;
                        case "--step":
                            ProcessStep(args, i, options);
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
                        case "--seed":
                            ProcessInputFile(args, i, options);
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
            ValidateParameterCount(args, i, "survival", 1); //Make sure that --survival will have at least one parameter
            ValueProcessor(args, i, options, "survival");

        }
        private static void ProcessBirth(string[] args, int i, Options options)
        {

            ValidateParameterCount(args, i, "birth", 1); //Make sure that --birth will have at least one parameter
            ValueProcessor(args, i, options, "birth");

        }

        /*Get the number of parameters after --survival
        Check to see if after --survival the argument contain -- 
        After that check to see if the argument is less then the argument length - 1
        After that check to see if the first character of the argument is a digit [0-9] */
        private static int CheckNumParam (string []args, int i)
        {
            int numOfParam = 1;
            int tempCounter = 1;

            while (true)
            {
                if (i + tempCounter < args.Length)
                {
                    if (args[i + tempCounter].Contains("--"))
                    {
                        break;

                    }
                   
                    numOfParam++;
                    tempCounter++;
                }
                else
                {
                    break;
                }
                
            }
            return numOfParam;
        }

        private static void ValueProcessor (string[] args, int i, Options options, string optionName)
        {
            List<int> inputList = new List<int>();
            List<string> stringList = new List<string>();
            List<int> tempList = new List<int>();
            int numOfParam = CheckNumParam(args, i);

            //Loop through all the parameters of --survival
            for (int param = 1; param < numOfParam; param++)
            {
                var argument = args[i + param];
                //Check if the current argument contain ... if it does not then check if the value can be passed into an int
                if (!argument.Contains("..."))
                {
                    if (!int.TryParse(args[i + param], out int tempNum))
                    {
                        throw new ArgumentException($"The supplied " + optionName + " parameter is not a valid integer.");
                    }
                    inputList.Add(tempNum);

                }
                else
                {

                    string[] numbers = Regex.Split(argument, @"\D+"); //Split the string up into individual digits
                    foreach (string value in numbers)
                    {//Loop through the numbers array after the regex split to turn each string element into an integer value
                        int intValue = int.Parse(value);
                        tempList.Add(intValue);
                    }
                    ElipseLoop(tempList, inputList);
                }
                stringList.Add(argument); //Add each of --survival parameter as a string to a list
            }
             //Add all values between the number on either side of the elipse. Inclusive of the numbers on either side 

            if (optionName == "survival")
            {
                options.SurvivalRate = inputList;
                options.SurvivalString = stringList;
            }
            else
            {
                options.BirthRate = inputList;
                options.BirthrateString = stringList;
            }
            
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

            if (i < args.Length - 1)
            {
                if (int.TryParse(args[i + 1], out int tempNum) || !args[i + 1].Contains("--"))
                {
                    throw new ArgumentException($"Ghost mode input: \'{args[i + 1]}\' is not valid as ghost mode has does not have a parameter");
                }
            }
 

            options.GhostMode = true;
        }

        private static void ProcessPeriodic(string[] args, int i, Options options)
        {
            if (i < args.Length - 1)
            {
                if (int.TryParse(args[i + 1], out int tempNum) || !args[i + 1].Contains("--"))
                {
                    throw new ArgumentException($"Periodic mode input: \'{args[i + 1]}\' is not valid as periodic mode has does not have a parameter");
                }
            }
            options.Periodic = true;
        }

        private static void ProcessStep(string[] args, int i, Options options)
        {
            if (i < args.Length - 1)
            {
                if (int.TryParse(args[i + 1], out int tempNum) || !args[i + 1].Contains("--"))
                {
                    throw new ArgumentException($"Step mode input: \'{args[i + 1]}\' is not valid as step mode has does not have a parameter");
                }
            }
            options.StepMode = true;
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

        private static void ElipseLoop(List<int> initialList, List<int> outputList)
        {

            int startElement = initialList[0];
            int endElement = initialList[1];
            for (int currentElement = startElement; currentElement <= endElement; currentElement++)
            {
                outputList.Add(currentElement);
            }

        }

    }
}
