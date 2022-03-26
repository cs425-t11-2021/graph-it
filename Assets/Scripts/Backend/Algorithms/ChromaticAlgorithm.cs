
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    public int ChromaticNumber { get; private set; }
    public int[] Coloring { get; private set; }

    public ChromaticAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        // TODO: if HasLoops, then warn user

        this.AlgoManager.RunMaxDegree();

        this.Coloring = new int[ this.Graph.Order ];
        int chi = Math.Min( this.Graph.Order, 1 );
        this.WaitUntilMaxDegreeComplete();
        int upperBound = ( int ) this.AlgoManager.GetMaxDegree() + 1;
        // TODO: use clique number for lower bound

        while ( !this.IsProperColoring() )
        {
            if ( chi > upperBound ) // something really bad happended
                throw new System.Exception( "Coloring could not be computed." );
            if ( this.Coloring.Min() >= chi - 1 )
            {
                chi++;
                Array.Clear( this.Coloring, 0, this.Coloring.Length );
            }
            this.UpdateColoring( chi );
        }
        this.ChromaticNumber = chi;
    }

    private bool IsProperColoring()
    {
        foreach ( Edge edge in this.Graph.Adjacency.Values )
        {
            if ( edge.vert1 == edge.vert2 ) // temp, ignoring loops
                continue;
            if ( this.Coloring[ this.Graph.Vertices.IndexOf( edge.vert1 ) ] == this.Coloring[ this.Graph.Vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private void UpdateColoring( int colors, int index=0 )
    {
        if ( index < this.Coloring.Length )
        {
            if ( this.Coloring[ index ] < colors - 1 )
            {
                this.Coloring[ index ]++;
                for ( int i = 0; i < index; ++i )
                    this.Coloring[ i ] = 0;
            }
            else
                this.UpdateColoring( colors, ++index );
        }
    }

    private void WaitUntilMaxDegreeComplete()
    {
        this.WaitUntilAlgorithmComplete( MaxDegreeAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
