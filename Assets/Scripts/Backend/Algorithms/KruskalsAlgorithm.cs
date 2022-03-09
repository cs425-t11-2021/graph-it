
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class KruskalsAlgorithm : Algorithm
{
    public List< Edge > Mst { get; private set; }

    public KruskalsAlgorithm( Graph graph, CancellationToken token, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, token, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        if ( this.Graph.Directed )
            throw new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." );

        this.Mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( this.Graph.Adjacency.Values.OrderBy( edge => edge.Weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        if ( this.IsKillRequested() )
            this.Kill();
        foreach ( Vertex vert in this.Graph.Vertices )
            forest.Add( new HashSet< Vertex >() { vert } );
        if ( this.IsKillRequested() )
            this.Kill();
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
