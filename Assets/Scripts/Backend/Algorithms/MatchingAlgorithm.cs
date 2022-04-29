
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MatchingAlgorithm : Algorithm
{
    private int nu;
    private List< Vertex > maxMatchingVertices = null;
    private List< Edge > maxMatchingEdges = null;

    public MatchingAlgorithm( AlgorithmManager algoManager, bool display ) : base(algoManager)
    {
        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
    }

    public override void Run()
    {
        List< Edge > edges = this.Graph.Adjacency.Values.ToList();
        if ( edges.Count == 0 )
            this.nu = 0;
        else
        {
            bool[] set = new bool[ edges.Count ];
            this.nu = edges.Count;

            while ( !this.IsMatching( edges, set ) || !this.HasSpecifiedCardinality( set, this.nu ) )
            {
                if ( this.nu < 0 ) // something really bad happended
                    this.CreateError( "Matching could not be computed." );
                if ( set.All( s => s ) )
                {
                    this.nu--;
                    Array.Clear( set, 0, set.Length );
                }
                this.UpdateSet( set );
            }

            // retrieve max matching
            this.maxMatchingEdges = new List< Edge >();
            for ( int i = 0; i < set.Length; ++i )
            {
                if ( set[ i ] )
                    this.maxMatchingEdges.Add( edges[ i ] );
            }
            this.maxMatchingVertices = new List< Vertex >( Edge.GetIncidentVertices( this.maxMatchingEdges ) );
        }
    }

    private bool IsMatching( List< Edge > edges, bool[] set )
    {
        for ( int i = 0; i < set.Length; ++i )
        {
            for ( int j = 0; j < set.Length; ++j )
            {
                if ( i != j && set[ i ] && set[ j ] && this.Graph.IsIncident( edges[ i ], edges[ j ] ) )
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
        result.results[ "maximum matching number" ] = ( this.nu, typeof ( int ) );
        result.results[ "maximum matching vertices" ] = ( this.maxMatchingVertices, typeof ( List< Vertex > ) );
        result.results[ "maximum matching edges" ] = ( this.maxMatchingEdges, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash() => typeof ( MatchingAlgorithm ).GetHashCode();

    public override int GetHashCode() => MatchingAlgorithm.GetHash();
}
