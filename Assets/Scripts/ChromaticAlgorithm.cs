
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    public int ChromaticNumber { get; private set; }

    public ChromaticAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        int chi = this.graph.vertices.Count;
        HashSet< List< int > > colorings = this.GetAllColorings();
        foreach ( List< int > coloring in colorings )
        {
            int num_colors = ( new HashSet< int >( coloring ) ).Count;
            if ( num_colors < chi && this.IsProperColoring( coloring ) )
                chi = num_colors;
        }

        this.ChromaticNumber = chi;

        BipartiteAlgorithm.SetChromaticNumber( this.graph, chi );
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

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
