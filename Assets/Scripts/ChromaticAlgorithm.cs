using System;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class ChromaticAlgorithm : IAlgorithm
{
	private Graph graph;
    private Thread? curr_thread;

    public int? chromatic_num;

	public ChromaticAlgorithm( Graph graph )
	{
		this.graph = graph;
        this.curr_thread = null;
        this.chromatic_num = null;

		this.Run();
	}

	public void Run()
	{
        // TODO: if curr_thread already exists, stop it from executing?

		// create new thread using RunHelper
        curr_thread = new Thread(new ThreadStart(RunHelper));
        curr_thread.Start()
	}

    private void RunHelper()
    {
        // compute chromatic number
        this.chromatic_num = GetChromaticNumber();

        // TODO: trigger event saying the result is ready
    }

	public int GetChromaticNumber()
    {
        int chi = graph.vertices.Count;
        HashSet< List< int > > colorings = this.GetAllColorings();
        foreach ( List< int > coloring in colorings )
        {
            int num_colors = ( new HashSet< int >( coloring ) ).Count;
            if ( num_colors < chi && this.IsProperColoring( coloring ) )
                chi = num_colors;
        }
        this.chromatic_num = chi;
        return chi;
    }

    private bool IsProperColoring( List< int > coloring )
    {
        foreach ( Edge edge in graph.adjacency.Values )
        {
            if ( coloring[ graph.vertices.IndexOf( edge.vert1 ) ] == coloring[ graph.vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private HashSet< List< int > > GetAllColorings()
    {
        HashSet< List< int > > colorings = new HashSet< List< int > >();
        GetAllColoringsHelper( colorings, new List< int >(), graph.vertices.Count, graph.vertices.Count );
        return colorings;
    }

    private static void GetAllColoringsHelper( HashSet< List< int > > colorings, List< int > coloring, int num_vertices, int num_colors )
    {
        if ( coloring.Count >= num_vertices )
            colorings.Add( coloring );
        else
        {
            for ( int i = 0; i < num_colors; i++ )
            {
                List< int > new_coloring = new List< int >( coloring );
                new_coloring.Add( i );
                GetAllColoringsHelper( colorings, new_coloring, num_vertices, num_colors );
            }
        }
    }
}
