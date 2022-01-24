
using System;
using System.Threading;

[System.Serializable]
public class Algorithm
{
	private Graph graph;

	public Algorithm( Graph graph )
	{
		this.graph = graph;
		this.Run();
	}

	public void Run()
	{
		// create new thread using Main
		// save and return result
	}


	public void Main();
}
