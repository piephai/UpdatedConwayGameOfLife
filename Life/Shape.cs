using System;

public class Shape
{
	//private int rows, columns;
	protected string inputFile;
	protected int[,] universe;
	protected string[] elements;
	protected bool isAlive;

	public Shape(int [,] inputUniverse, string [] inputElements, bool isAliveInput)
	{
		universe = inputUniverse;
		elements = inputElements;
		isAlive = isAliveInput;
	}

	public virtual int [,] GetUniverse (int [,] universe, string [] elements, bool isAlive)
    {

		return universe;
    }
	public void RemoveCommaFromElements (string [] elements)
    {
		for (int i = 0; i < elements.Length; i++)
		{  //Replace all the commas (Read only so the file will not be changed)
			elements[i] = elements[i].Replace(",", "");
		}

	}

}
