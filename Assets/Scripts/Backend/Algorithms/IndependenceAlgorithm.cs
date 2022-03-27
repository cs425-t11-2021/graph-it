
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class IndependenceAlgorithm : Algorithm
{
    public int IndependenceNumber { get; private set; }
    public List< Vertex > MaxIndependentSet { get; private set; }

    public IndependenceAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        bool[] set = new bool[ this.Graph.Order ];
        int card = this.Graph.Order;

        while ( !this.IsIndependentSet( set ) || !this.HasSpecifiedCardinality( set, card ) )
        {
            if ( card < 0 ) // something really bad happended
                throw new System.Exception( "Indepedent set could not be computed." );
            if ( set.All( s => s ) )
            {
                card--;
                Array.Clear( set, 0, set.Length );
            }
            this.UpdateSet( set );
        }
        this.IndependenceNumber = card;

        // retrieve max independent set
        List< Vertex > maxIndependentSet = new List< Vertex >();
        for ( int i = 0; i < set.Length; ++i )
        {
            if ( set[ i ] )
                maxIndependentSet.Add( this.Graph.Vertices[ i ] );
        }
        this.MaxIndependentSet = maxIndependentSet;
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

    public static int GetHash() => typeof ( IndependenceAlgorithm ).GetHashCode();

    public override int GetHashCode() => IndependenceAlgorithm.GetHash();
}
