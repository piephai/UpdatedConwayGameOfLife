using System;

public class Ellipse: Shape
{
	public Ellipse(int[,] inputUniverse, string[] inputElements, bool isAliveInput) : base(inputUniverse, inputElements, isAliveInput)
	{
	}

	public override int[,] GetUniverse(int[,] universe, string[] elements, bool isAlive)
	{
        double rowBottomLeft = double.Parse(elements[3]);
        double colBottomLeft = double.Parse(elements[4]);
        double rowTopRight = double.Parse(elements[5]);
        double colTopRight = double.Parse(elements[6]);


        double height = rowTopRight - rowBottomLeft;
        double width = colTopRight - colBottomLeft;
        double centreX = (colTopRight + colBottomLeft) / 2;
         double centreY = (rowBottomLeft + rowTopRight) / 2;



        for (int row = (int)rowBottomLeft; row <= rowTopRight; row++)
        {
            for (int col = (int)colBottomLeft; col <= colTopRight; col++)
            {
                double temp = (double)(((4 * Math.Pow(row - centreY, 2.0)) / Math.Pow(height, 2.0)) + ((4 * Math.Pow(col - centreX, 2.0)) / Math.Pow(width, 2.0)));
                if (temp <= 1.0)
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
        return universe;
	}
}
