
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    public int ChromaticNumber { get; private set; }

    public ChromaticAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        int chi = this.Graph.Vertices.Count;
        HashSet< List< int > > colorings = this.GetAllColorings();
        foreach ( List< int > coloring in colorings )
        {
            int numColors = coloring.Count;
            if ( numColors < chi && this.IsProperColoring( coloring ) )
                chi = numColors;
        }

        this.ChromaticNumber = chi;

        BipartiteAlgorithm.SetChromaticNumber( this.Graph, chi );
    }

    private bool IsProperColoring( List< int > coloring )
    {
        foreach ( Edge edge in Graph.Adjacency.Values )
        {
            if ( coloring[ this.Graph.Vertices.IndexOf( edge.vert1 ) ] == coloring[ this.Graph.Vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private HashSet< List< int > > GetAllColorings()
    {
        HashSet< List< int > > colorings = new HashSet< List< int > >();
        GetAllColoringsHelper( colorings, new List< int >(), this.Graph.Vertices.Count, this.Graph.Vertices.Count );
        return colorings;
    }

    private static void GetAllColoringsHelper( HashSet< List< int > > colorings, List< int > coloring, int numVertices, int numColors )
    {
        if ( coloring.Count >= numVertices )
            colorings.Add( coloring );
        else
        {
            for ( int i = 0; i < numColors; i++ )
            {
                List< int > newColoring = new List< int >( coloring );
                newColoring.Add( i );
                GetAllColoringsHelper( colorings, newColoring, numVertices, numColors );
            }
        }
    }

    public override void Kill()
    {
        base.Kill();
        BipartiteAlgorithm.ClearChromaticNumber( this.Graph );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
