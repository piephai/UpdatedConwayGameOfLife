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

        private int rows = 16;
        private int columns = 16;
        private int generations = 50;
        private double updateRate = 5.0;
        private double randomFactor = 0.5;
        private string inputFile = null;
        // MY CHANGES
        private int neighbourOrder = 1;
        private string neighbourhoodType;
        

        public int Rows 
        {
            get => rows;
            set 
            {
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Row dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                rows = value;
            } 
        }

        public int Columns
        {
            get => columns;
            set
            {
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Column dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                columns = value;
            }
        }

        public int Generations
        {
            get => generations;
            set
            {
                if (value < MIN_GENERATION)
                {
                    throw new ArgumentException($"Generation count \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_GENERATION} and above)");
                }
                generations = value;
            }
        }

        public double UpdateRate
        {
            get => updateRate;
            set
            {
                if (value < MIN_UPDATE || value > MAX_UPDATE)
                {
                    throw new ArgumentException($"Update rate \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_UPDATE} - {MAX_UPDATE})");
                }
                updateRate = value;
            }
        }

        public double RandomFactor
        {
            get => randomFactor;
            set
            {
                if (value < MIN_RANDOM || value > MAX_RANDOM)
                {
                    throw new ArgumentException($"Random factor \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_RANDOM} - {MAX_RANDOM})");
                }
                randomFactor = value;
            }
        }

        public string InputFile
        {
            get =>  inputFile;
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File \'{value}\' does not exist.");
                }
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Incompatible file extension \'{Path.GetExtension(value)}\'");
                }
                inputFile = value;
            }
        }

        // my changes

        public int NeighbourOrder
        {
            get => neighbourOrder;
            set
            {
                // Checking neighbourhood order is between 1 and 10 (inclusive)    
                if (value > MIN_ORDER && value <= MAX_ORDER)
                {
                    int lessThanSmallestDimension = value / 2;

                    // Checking order is less than half of the smallest dimension
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
                    else
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
                }
                else
                {
                    throw new ArgumentException($"Neighbour order \'{value:F2}\' is outside of the acceptable" +
                        $"range of values ({MIN_ORDER} - {MAX_ORDER})");
                }
            }
        }

        public string NeighbourhoodType
        {
            get => neighbourhoodType;
            set
            {
                if (value == "moore")
                {
                    neighbourhoodType = value;
                    //implement more
                }
                else if (value == "vonNeumann")
                {
                    neighbourhoodType = value;
                    //implement more
                }
                else
                {
                    throw new ArgumentException($"The input: \'{value}\' is not a valid parameter. " +
                    $"The neighbour centre must either be moore or vonNeumann.");
                }
            }
        }




        public bool Periodic { get; set; } = false;

        public bool StepMode { get; set; } = false;

        public override string ToString()
        {
            const int padding = 30;

            string output = "\n";
            output += "Input File: ".PadLeft(padding) + (InputFile != null ? InputFile : "N/A") + "\n";
            output += "Generations: ".PadLeft(padding) + $"{Generations}\n";
            output += "Update Rate: ".PadLeft(padding) + $"{UpdateRate} updates/s\n";
            output += "Periodic: ".PadLeft(padding) + (Periodic ? "Yes" : "No") + "\n";
            output += "Rows: ".PadLeft(padding) + Rows + "\n";
            output += "Columns: ".PadLeft(padding) + Columns + "\n";
            output += "Random Factor: ".PadLeft(padding) + $"{100 * RandomFactor:F2}%\n";
            output += "Step Mode: ".PadLeft(padding) + (StepMode ? "Yes" : "No") + "\n";
            return output;
        }
    }
}
