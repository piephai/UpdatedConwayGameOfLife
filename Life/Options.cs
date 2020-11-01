using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Life
{
    class Options
    {
        private const int MIN_DIMENSION = 4;
        private const int MAX_DIMENSION = 48;
        private const int MIN_GENERATION = 4;
        private const double MIN_UPDATE = 1.0;
        private const double MAX_UPDATE = 30.0;
        private const double MIN_RANDOM = 0.0;
        private const double MAX_RANDOM = 1.0;
        private const int MIN_ORDER = 1;
        private const int MAX_ORDER = 10;
        private const int MAX_MEMORY = 512;
        private const int MIN_MEMORY = 4;
        
        private int rows = 16;
        private int columns = 16;
        private int generations = 50;
        private double updateRate = 5.0;
        private double randomFactor = 0.5;
        private string outputFile = null;
        private string inputFile = null;
        private int neighbourOrder = 1;
        private string neighbourhoodType = "Moore";
        private bool centreCount = false;
        private int memory = 16;
        private bool ghostMode = false;
        private List <int> survivalRate = new List<int> {2, 3 };
        private List<int> birthRate = new List<int> { 3 };
        private bool containElipse = false;
        private List<string> survivalString = new List<string> { "2...3" };
        private List<string> birthRateString = new List<string> { "3" };
        private bool isAllDead = false;

        // Public get set accessor for the private rows value
        public int Rows 
        {
            get => rows;
            set 
            {   
                /* If the value is smaller than the minimum dimension or larger than the max dimension it will throw
                 * an exception */
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Row dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                rows = value;
            } 
        }

        // Public get set accessor for the private columns value
        public int Columns
        {
            get => columns;
            set
            {
                /* If the value is smaller than the minimum dimension or larger than the max dimension it will throw
                 * an exception */
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Column dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                columns = value;
            }
        }

        // Public get set accessor to check for a true or false depending on whether all the cells are dead
        public bool IsAllDead
        {
            get => isAllDead;
            set
            {
                isAllDead = value;
            }
        }

        // Public get set accessor for the private generations value
        public int Generations
        {
            get => generations;
            set
            {
                // If the value is smaller than the minimum generation it will throw an exception
                if (value < MIN_GENERATION)
                {
                    throw new ArgumentException($"Generation count \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_GENERATION} and above)");
                }
                generations = value;
            }
        }

        // Public get set accessor for the private update rate value
        public double UpdateRate
        {
            get => updateRate;
            set
            {
                /* If the value is smaller than the minimum update rate or larger than the max update rate it will
                 * throw an exception */
                if (value < MIN_UPDATE || value > MAX_UPDATE)
                {
                    throw new ArgumentException($"Update rate \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_UPDATE} - {MAX_UPDATE})");
                }
                updateRate = value;
            }
        }

        // Public get set accessor to check for a true or false value depending on whether it is an Ellipse
        public bool Elipse
        {
            get => containElipse;
            set
            {
                containElipse = value;
            }
        }

        // Public get set accessor for the private random factor value
        public double RandomFactor
        {
            get => randomFactor;
            set
            {   /* If the value is smaller than the minimum random factor or larger than the max random factor it will
                 * throw an exception*/
                if (value < MIN_RANDOM || value > MAX_RANDOM)
                {
                    throw new ArgumentException($"Random factor \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_RANDOM} - {MAX_RANDOM})");
                }
                randomFactor = value;
            }
        }

        // Public get set accessor to check and assign the input file
        public string InputFile
        {
            get =>  inputFile;
            set
            {   
                // If the input file does not exist it will throw an exception
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File \'{value}\' does not exist.");
                }
                // If the input file does not contain the .seed extension then it will throw an exception
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Incompatible file extension \'{Path.GetExtension(value)}\'");
                }
                inputFile = value;
            }
        }

        // Public get set accessor for the private neighbour order value
        public int NeighbourOrder
        {
            get => neighbourOrder;
            set
            {
                // Checking neighbourhood order is between 1 and 10 (inclusive)    
                if (value >= MIN_ORDER && value <= MAX_ORDER)
                {
                    int lessThanSmallestDimension = value / 2;

                    // If rows is smaller columns it will check the order is less than half of the rows dimension
                    if (rows < columns)
                    {
                        if (lessThanSmallestDimension < rows)
                        {
                            neighbourOrder = value;
                        }
                        else
                        {
                            throw new ArgumentException($"Neighbour order must be less than half of the rows dimension ({rows})");
                        }
                    }
                    // If columns is smaller rows it will check the order is less than half of the columns dimension
                    else if (columns < rows)
                    {
                        if (lessThanSmallestDimension < columns)
                        {
                            neighbourOrder = value;
                        }
                        else
                        {
                            throw new ArgumentException($"Neighbour order must be less than half of the columns dimension ({columns})");
                        }
                    }
                    else
                    {   
                        // If the rows and columns are both bigger than the halved value 
                        if(lessThanSmallestDimension < rows && lessThanSmallestDimension < columns)
                        {
                            neighbourOrder = value;
                        }
                        else
                        {
                            throw new ArgumentException($"Neighbour order must be less than half of the smallest dimension");
                        }
                    }
                }
                // If the value is outside of the specifications it will throw an exception
                else
                {
                    throw new ArgumentException($"Neighbour order \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_ORDER} - {MAX_ORDER})");
                }
            }
        }

        // Public get set accessor for the private neighbourhood type value
        public string NeighbourhoodType
        {
            get => neighbourhoodType;
            set
            {
                string mooreString = "moore";
                string vonString = "vonNeumann";

                // Checking if the value is equal to "moore" case insensitive 
                if (String.Equals(value, mooreString, StringComparison.CurrentCultureIgnoreCase))
                {
                    neighbourhoodType = value;
                }
                // Checking if the value is equal to "conNeumann" case insensitive
                else if (String.Equals(value, vonString, StringComparison.CurrentCultureIgnoreCase))
                {
                    neighbourhoodType = value;
                }
                // If it is equal to neither it will throw an exception
                else
                {
                    throw new ArgumentException($"The input: \'{value}\' is not a valid parameter. " +
                    $"The neighbour centre must either be moore or vonNeumann.");
                }
            }
        }

        // Public get set accessor for the private Centre Count value
        public bool CentreCount
        {
            get => centreCount;
            set
            {
                centreCount = value;
            }
        }

        // Public get set accessor for the private survival rate value
        public List <int> SurvivalRate
        {
            get => survivalRate;
            set
            {
                
                survivalRate = value;
            }
        }

        // Public get set accessor for the private birth rate value
        public List<int> BirthRate
        {
            get => birthRate;
            set
            {
               
                birthRate = value;
            }
        }

        // Public get set accessor for the private survival string value
        public List <string> SurvivalString
        {
            get => survivalString;
            set
            {
                survivalString = value;
            }
        }

        // Public get set accessor for the private birth rate string value
        public List<string> BirthrateString
        {
            get => birthRateString;
            set
            {
                birthRateString = value;
            }
        }

        // Public get set accessor for the private generational memory value
        public int GenerationalMemory
        {
            get => memory;
            set
            {   /* If the value is smaller than the minimum memory or larger than the max memory it will
                 * throw an exception*/
                if (value >= MIN_MEMORY && value <= MAX_MEMORY)
                {
                    memory = value;
                }
                else
                {
                    throw new ArgumentException($"Generation memory must be an integer between 4 and 512 (inclusive)");
                }
            }
        }

        // Public get set accessor for the output file
        public string OutputFile // change?
        {
            get => outputFile;
            set
            {
                // Checking if the output file exists because then it cannot be created
                if (File.Exists(value))
                {
                    throw new ArgumentException($"File \'{value}\' exists already");
                }
                // Checking if the file does not contain the .seed extension
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Incompatible file extension \'{Path.GetExtension(value)}\'");
                }
                outputFile = value;
            }
        }

        // Public get set accessor to check for a true or false value for ghost mode
        public bool GhostMode
        {
            get => ghostMode;
            set
            {
                ghostMode = value;
            }
        }

        // Public get set accessor to check for a true or false value for periodic mode
        public bool Periodic { get; set; } = false;

        // Public get set accessor to check for a true or false value for step mode
        public bool StepMode { get; set; } = false;

        /// <summary>
        /// This method creates an output string and adds the information of each options to the string.
        /// </summary>
        /// <returns> This returns an string of all the options and their relevant settings or values </returns>
        public override string ToString()
        {
            const int padding = 30;

            string output = "\n";
            output += "Input File: ".PadLeft(padding) + (InputFile != null ? InputFile : "N/A") + "\n";
            output += "Output File: ".PadLeft(padding) + (OutputFile != null ? OutputFile : "N/A") + "\n";
            output += "Generations: ".PadLeft(padding) + $"{Generations}\n";
            output += "Update Rate: ".PadLeft(padding) + $"{UpdateRate} updates/s\n";
            output += "Memory: ".PadLeft(padding) + $"{memory}\n";
            output += "Rules: ".PadLeft(padding) + "S( ";
            for (int i = 0; i < survivalString.Count; i++)
            {
                output += survivalString[i] + " ";
            }
            output += ")" + " B( ";
            for (int i = 0; i < birthRateString.Count; i++)
            {
                output += birthRateString[i] + " ";
            }
            output += ")\n";
            output += "Neighbourhood: ".PadLeft(padding) + $"{neighbourhoodType}" + " (" + $"{ neighbourOrder}" + ")";
            // Checking whether centre count is counted or not counted
            if (centreCount)
            {
                output += " (" + "centre-counted" + ")\n";
            }
            else
            {
                output += "\n";
            }

            output += "Periodic: ".PadLeft(padding) + (Periodic ? "Yes" : "No") + "\n";
            output += "Rows: ".PadLeft(padding) + Rows + "\n";
            output += "Columns: ".PadLeft(padding) + Columns + "\n";
            output += "Random Factor: ".PadLeft(padding) + $"{100 * RandomFactor:F2}%\n";
            output += "Step Mode: ".PadLeft(padding) + (StepMode ? "Yes" : "No") + "\n";
            output += "Ghost Mode: ".PadLeft(padding) + (GhostMode ? "Yes" : "No") + "\n";
            
            return output;
        }
    }
}
