
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
        ushort upperBound = ( ushort ) this.Graph.Vertices.Count;
        ushort[] coloring = new ushort[ this.Graph.Vertices.Count ];
        int chi = upperBound;
        while ( chi != 0 && coloring.Min() < upperBound - 1 )
        {
            if ( this.IsProperColoring( coloring ) )
                chi = Math.Min( chi, coloring.Distinct().Count() );
            this.UpdateColoring( coloring, upperBound );
        }
        this.ChromaticNumber = chi;

        BipartiteAlgorithm.SetChromaticNumber( this.Graph, this.ChromaticNumber );
    }

    private bool IsProperColoring( ushort[] coloring )
    {
        foreach ( Edge edge in this.Graph.Adjacency.Values )
        {
            if ( coloring[ this.Graph.Vertices.IndexOf( edge.vert1 ) ] == coloring[ this.Graph.Vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private void UpdateColoring( ushort[] coloring, ushort max )
    {
        this.UpdateColoringHelper( coloring, max, 0 );
    }

    private void UpdateColoringHelper( ushort[] coloring, ushort max, ushort index )
    {
        if ( coloring[ index ] < max )
        {
            coloring[ index ]++;
            for ( int i = 0; i < index; ++i )
                coloring[ i ] = 0;
        }
        else
            this.UpdateColoringHelper( coloring, max, ++index );
    }

    public override void Kill()
    {
        base.Kill();
        BipartiteAlgorithm.ClearChromaticNumber( this.Graph );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
