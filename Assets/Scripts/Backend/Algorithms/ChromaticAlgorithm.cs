
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    public int ChromaticNumber { get; private set; }
    public int[] Coloring { get; private set; }

    public ChromaticAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        // TODO: if HasLoops, then return +inf

        this.AlgoManager.RunMaxDegree();

        int[] coloring = new int[ this.Graph.Order ];
        int chi = Math.Min( this.Graph.Order, 1 );
        this.WaitUntilMaxDegreeComplete();
        int upperBound = ( int ) this.AlgoManager.GetMaxDegree() + 1;
        // TODO: use clique number for lower bound

        while ( !this.IsProperColoring( coloring ) )
        {
            if ( chi > upperBound ) // something really bad happended
                throw new System.Exception( "Coloring could not be computed." );
            if ( coloring.Min() >= chi - 1 )
            {
                chi++;
                Array.Clear( coloring, 0, coloring.Length );
            }
            this.UpdateColoring( coloring, chi );
        }
        this.Coloring = coloring;
        this.ChromaticNumber = chi;
    }

    private bool IsProperColoring( int[] coloring )
    {
        for ( int i = 0; i < this.Graph.Order; ++i )
        {
            for ( int j = 0; j < this.Graph.Order; ++j )
            {
                // temp i != j ignoring loops
                if ( i != j && coloring[ i ] == coloring[ j ] && this.Graph.IsAdjacent( this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] ) )
                    return false;
            }
        }
        return true;
    }

    private void UpdateColoring( int[] coloring, int colors, int index=0 )
    {
        if ( index < coloring.Length )
        {
            if ( coloring[ index ] < colors - 1 )
            {
                coloring[ index ]++;
                for ( int i = 0; i < index; ++i )
                    coloring[ i ] = 0;
            }
            else
                this.UpdateColoring( coloring, colors, ++index );
        }
    }

    private void WaitUntilMaxDegreeComplete()
    {
        this.WaitUntilAlgorithmComplete( MaxDegreeAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
