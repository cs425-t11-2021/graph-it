
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MatchingAlgorithm : Algorithm
{
    public int MaxMatchingCard { get; private set; }
    public List< Edge > MaxMatching { get; private set; }

    public MatchingAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        List< Edge > edges = this.Graph.Adjacency.Values.Where( edge => edge.Directed || edge.vert1 < edge.vert2 ).ToList();
        bool[] set = new bool[ edges.Count ];
        int card = edges.Count;

        while ( !this.IsMatching( edges, set ) || !this.HasSpecifiedCardinality( set, card ) )
        {
            if ( card < 0 ) // something really bad happended
                throw new System.Exception( "Matching could not be computed." );
            if ( set.All( s => s ) )
            {
                card--;
                Array.Clear( set, 0, set.Length );
            }
            this.UpdateSet( set );
        }
        this.MaxMatchingCard = card;

        // retrieve max matching
        List< Edge > maxMatching = new List< Edge >();
        for ( int i = 0; i < set.Length; ++i )
        {
            if ( set[ i ] )
                maxMatching.Add( edges[ i ] );
        }
        this.MaxMatching = maxMatching;
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

    public static int GetHash() => typeof ( MatchingAlgorithm ).GetHashCode();

    public override int GetHashCode() => MatchingAlgorithm.GetHash();
}
