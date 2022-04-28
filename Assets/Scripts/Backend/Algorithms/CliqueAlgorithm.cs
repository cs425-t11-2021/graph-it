
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class CliqueAlgorithm : Algorithm
{
    private int omega;
    private List< Vertex > cliqueVertices;
    private List< Edge > cliqueEdges;


    public CliqueAlgorithm(AlgorithmManager algoManager, bool display) : base(algoManager)
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
        this.omega = this.Graph.Order;

        while ( !this.IsClique( set ) || !this.HasSpecifiedCardinality( set, this.omega ) )
        {
            if ( this.omega < 0 ) // something really bad happended
                this.CreateError( "Maximum clique could not be computed." );
            if ( set.All( s => s ) )
            {
                this.omega--;
                Array.Clear( set, 0, set.Length );
            }
            this.UpdateSet( set );
        }

        // retrieve max clique
        this.cliqueVertices = new List< Vertex >();
        this.cliqueEdges = new List< Edge >();
        for ( int i = 0; i < set.Length; ++i )
        {
            if ( set[ i ] )
                this.cliqueVertices.Add( this.Graph.Vertices[ i ] );
        }
        foreach ( Vertex vert1 in this.cliqueVertices )
        {
            foreach ( Vertex vert2 in this.cliqueVertices )
            {
                if ( vert1 != vert2 )
                    this.cliqueEdges.Add( this.Graph[ vert1, vert2 ] );
            }
        }
    }

    private bool IsClique( bool[] set )
    {
        for ( int i = 0; i < set.Length; ++i )
        {
            for ( int j = 0; j < set.Length; ++j )
            {
                if ( i != j && set[ i ] && set[ j ] && !this.Graph.IsAdjacent( this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] ) )
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
        result.results[ "clique number" ] = ( this.omega, typeof ( int ) );
        result.results[ "maximum clique vertices" ] = ( this.cliqueVertices, typeof ( List< Vertex > ) );
        result.results[ "maximum clique edges" ] = ( this.cliqueEdges, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash() => typeof ( CliqueAlgorithm ).GetHashCode();

    public override int GetHashCode() => CliqueAlgorithm.GetHash();
}
