using System;

public class Rectangle: Shape
{
	public Rectangle(int [,] inputUniverse, string [] inputElements, bool isAliveInput): base (inputUniverse, inputElements, isAliveInput)
	{
	}

	public override int [,] GetUniverse(int[,] universe, string[] elements, bool isAlive)
    {
        int rowBottomLeft = int.Parse(elements[3]);
        int colBottomLeft = int.Parse(elements[4]);
        int rowTopRight = int.Parse(elements[5]);
        int colTopRight = int.Parse(elements[6]);

        //For rectangle cell structure

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
       

        return universe;
    }

	public override void RemoveCommaFromElements (string [] elements)
    {
        for (int i = 0; i < elements.Length; i++)
        {  //Replace all the commas (Read only so the file will not be changed)
            elements[i] = elements[i].Replace(",", "");
        }
       
    }
}
