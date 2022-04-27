
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class IndependenceAlgorithm : Algorithm
{
    private int alpha;
    private List< Vertex > maxIndependentSet;

    public IndependenceAlgorithm(AlgorithmManager algoManager, bool display) : base(algoManager)
    {
        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
    }

    public override void Run()
    {
        bool[] set = new bool[ this.Graph.Order ];
        this.alpha = this.Graph.Order;

        while ( !this.IsIndependentSet( set ) || !this.HasSpecifiedCardinality( set, this.alpha ) )
        {
            if ( this.alpha < 0 ) // something really bad happended
                this.CreateError( "Indepedent set could not be computed." );
            if ( set.All( s => s ) )
            {
                this.alpha--;
                Array.Clear( set, 0, set.Length );
            }
            this.UpdateSet( set );
        }

        // retrieve max independent set
        this.maxIndependentSet = new List< Vertex >();
        for ( int i = 0; i < set.Length; ++i )
        {
            if ( set[ i ] )
                this.maxIndependentSet.Add( this.Graph.Vertices[ i ] );
        }
    }

    private bool IsIndependentSet( bool[] set )
    {
        for ( int i = 0; i < set.Length; ++i )
        {
            for ( int j = 0; j < set.Length; ++j )
            {
                if ( set[ i ] && set[ j ] && this.Graph.IsAdjacent( this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] ) )
                    return false;
            }
        }
        return true;
    }

    private void UpdateSet( bool[] set, int index=0 )
    {
        if ( index < set.Length )
        {
            if ( !set[ index ] )
            {
                set[ index ] = true;
                for ( int i = 0; i < index; ++i )
                    set[ i ] = false;
            }
            else
                this.UpdateSet( set, ++index );
        }
    }

    private bool HasSpecifiedCardinality( bool[] set, int card )
    {
        int c = 0;
        foreach ( bool s in set )
        {
            if ( s )
                c++;
        }
        return card == c;
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "indepednence number" ] = ( this.alpha, typeof ( int ) );
        result.results[ "maximum independent set" ] = ( this.maxIndependentSet, typeof ( List< Vertex > ) );
        return result;
    }

    public static int GetHash() => typeof ( IndependenceAlgorithm ).GetHashCode();

    public override int GetHashCode() => IndependenceAlgorithm.GetHash();
}
