
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class KruskalsAlgorithm : Algorithm
{
    List< Edge > mst;

    public KruskalsAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        if ( this.graph.directed )
        {
            // Debug.Log( ( new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." );
        }

        this.mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( graph.adjacency.Values.OrderBy( edge => edge.weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        foreach ( Vertex vert in this.graph.vertices )
            forest.Add( new HashSet< Vertex >() { vert } );
        foreach ( Edge edge in edges )
        {
            HashSet< Vertex > tree1 = KruskalsAlgorithm.GetComponentOf( forest, edge.vert1 );
            HashSet< Vertex > tree2 = KruskalsAlgorithm.GetComponentOf( forest, edge.vert2 );
            if ( tree1 != tree2 )
            {
                forest.Remove( tree1 );
                tree2.UnionWith( tree1 );
                this.mst.Add( edge );
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
        // Debug.Log( ( new System.Exception( "Vertex could not be found in collection of components." ) ).ToString() );
        throw new System.Exception( "Vertex could not be found in collection of components." );
    }

    public static int GetHash() => typeof ( KruskalsAlgorithm ).GetHashCode();

    public override int GetHashCode() => KruskalsAlgorithm.GetHash();
}
