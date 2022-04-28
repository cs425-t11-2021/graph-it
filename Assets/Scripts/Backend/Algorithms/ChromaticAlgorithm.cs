
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    private int chi;
    private int lowerBound;
    private int upperBound;
    private int[] coloring;

    public ChromaticAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        // if loops, then chi = +inf
        if ( this.Graph.HasLoops )
            this.chi = int.MaxValue; // TODO: make proper infinity
        else
        {
            this.AlgoManager.RunMaxDegree();
            this.coloring = new int[ this.Graph.Order ];

            // get lower bound, use clique number if possible
            if ( this.AlgoManager.IsComplete( CliqueAlgorithm.GetHash() ) )
                this.lowerBound = ( int ) this.AlgoManager.GetClique().results[ "clique number" ].Item1;
            else
                this.lowerBound = Math.Min( this.Graph.Order, 1 );
            // get upper bound
            this.WaitUntilMaxDegreeComplete();
            this.upperBound = ( int ) this.AlgoManager.GetMaxDegree().results[ "maximum degree" ].Item1 + 1;
            this.estimated = true;

            this.chi = this.lowerBound;
            while ( !this.IsProperColoring( coloring ) )
            {
                if ( this.chi > upperBound ) // something really bad happended
                    this.CreateError( "Coloring could not be computed." );
                if ( coloring.Min() >= this.chi - 1 )
                {
                    this.chi++;
                    Array.Clear( coloring, 0, coloring.Length );
                }
                this.UpdateColoring( coloring, this.chi );
            }

            this.estimated = false;
        }

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

    public override AlgorithmResult GetResult()
    {
        AlgorithmResult result;
        if ( this.error )
            return this.GetErrorResult();
        if ( this.estimated )
        {
            result = new AlgorithmResult( AlgorithmResultType.ESTIMATE );
            result.results[ "chromatic number lower bound" ] = ( this.lowerBound, typeof ( int ) );
            result.results[ "chromatic number upper bound" ] = ( this.upperBound, typeof ( int ) );
            return result;
        }
        if ( this.running )
            return this.GetRunningResult();
        result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "chromatic number" ] = ( this.chi, typeof ( int ) );
        result.results[ "minimal coloring" ] = ( this.coloring, typeof ( int[] ) );
        return result;
    }

    private void WaitUntilMaxDegreeComplete()
    {
        this.WaitUntilAlgorithmComplete( MaxDegreeAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
