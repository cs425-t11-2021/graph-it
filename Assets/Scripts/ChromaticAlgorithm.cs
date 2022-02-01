using System;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class ChromaticAlgorithm : IAlgorithm
{
	private Graph graph;
    private Thread curr_thread;

    public int chromatic_number;

	public ChromaticAlgorithm( Graph graph) // pass delegate method
	{
		this.graph = graph;

        this.curr_thread = null;
	}

	public void Run()
	{
        // TODO: if curr_thread already exists, abort it and let it restart
        if (curr_thread != null && curr_thread.IsAlive) {
            curr_thread.Abort();
        }

		// create new thread using RunHelper
        this.curr_thread = new Thread(new ThreadStart(RunHelper));
        curr_thread.Start();
	}

    private void RunHelper()
    {
        RunInMain.singleton.queuedTasks.Enqueue((GraphInfo.singleton.test, 0));
        try
        {
            // compute chromatic number
            this.chromatic_number = GetChromaticNumber();

            // run delegate
            RunInMain.singleton.queuedTasks.Enqueue((GraphInfo.singleton.updateChromaticNumber, this.chromatic_number));
        }
        catch ( ThreadAbortException e ) { } // thread has been aborted
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
