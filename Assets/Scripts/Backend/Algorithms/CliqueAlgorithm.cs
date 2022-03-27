
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class CliqueAlgorithm : Algorithm
{
    public int CliqueNumber { get; private set; }
    public List< Vertex > MaxClique { get; private set; }

    public CliqueAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        bool[] set = new bool[ this.Graph.Order ];
        int card = this.Graph.Order;

        while ( !this.IsClique( set ) || !this.HasSpecifiedCardinality( set, card ) )
        {
            if ( card < 0 ) // something really bad happended
                throw new System.Exception( "Clique could not be computed." );
            if ( set.All( s => s ) )
            {
                card--;
                Array.Clear( set, 0, set.Length );
            }
            this.UpdateSet( set );
        }
        this.CliqueNumber = card;

        // retrieve max clique
        List< Vertex > maxClique = new List< Vertex >();
        for ( int i = 0; i < set.Length; ++i )
        {
            if ( set[ i ] )
                maxClique.Add( this.Graph.Vertices[ i ] );
        }
        this.MaxClique = maxClique;
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

    public static int GetHash() => typeof ( CliqueAlgorithm ).GetHashCode();

    public override int GetHashCode() => CliqueAlgorithm.GetHash();
}
