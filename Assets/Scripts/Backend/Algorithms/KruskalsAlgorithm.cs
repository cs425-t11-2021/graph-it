
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class KruskalsAlgorithm : Algorithm
{
    public List< Edge > Mst { get; private set; }

    public KruskalsAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager )
    {
        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
    }

    public override void Run()
    {
        if ( this.Graph.Directed )
            throw new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." );

        this.Mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( this.Graph.Adjacency.Values.OrderBy( edge => edge.Weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        foreach ( Vertex vert in this.Graph.Vertices )
            forest.Add( new HashSet< Vertex >() { vert } );
        foreach ( Edge edge in edges )
        {
            HashSet< Vertex > tree1 = KruskalsAlgorithm.GetComponentOf( forest, edge.vert1 );
            HashSet< Vertex > tree2 = KruskalsAlgorithm.GetComponentOf( forest, edge.vert2 );
            if ( tree1 != tree2 )
            {
                forest.Remove( tree1 );
                tree2.UnionWith( tree1 );
                this.Mst.Add( edge );
            }
        }
    }

    private static HashSet< Vertex > GetComponentOf( HashSet< HashSet< Vertex > > components, Vertex vert )
    {
        foreach ( HashSet< Vertex > component in components )
        {
            if ( component.Contains( vert ) )
                return component;
        }
        throw new System.Exception( "Vertex could not be found in collection of components." );
    }

    public static int GetHash() => typeof ( KruskalsAlgorithm ).GetHashCode();

    public override int GetHashCode() => KruskalsAlgorithm.GetHash();
}
