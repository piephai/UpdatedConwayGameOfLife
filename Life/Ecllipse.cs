using System;

public class Ecllipse: Shape
{
	public Ecllipse(int[,] inputUniverse, string[] inputElements, bool isAliveInput) : base(inputUniverse, inputElements, isAliveInput)
	{
	}

	public override int[,] GetUniverse(int[,] universe, string[] elements, bool isAlive)
	{
		return universe;
	}
}
