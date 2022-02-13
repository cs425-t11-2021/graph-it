
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class PrimsAlgorithm : Algorithm
{
    public Vertex Root { get; private set; }
    public List< Edge > Mst { get; private set; }

    public PrimsAlgorithm( Graph graph, Vertex root, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning )
    {
        if ( !this.graph.vertices.Contains( root ) )
            throw new System.Exception( "Vertex for Prim's algorithm is not in graph." );
        this.Root = root;
    }

    public override void Run()
    {
        if ( this.graph.directed )
        {
            // Debug.Log( ( new System.Exception( "Prim's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

        this.Mst = new List< Edge >();
        HashSet< Vertex > mst_vertices = new HashSet< Vertex >() { this.Root };
        int mst_vertices_prev_count = -1;
        while ( mst_vertices_prev_count != mst_vertices.Count )
        {
            mst_vertices_prev_count = mst_vertices.Count;
            List< Edge > incident_edges = new List< Edge >( this.graph.GetIncidentEdges( mst_vertices ).OrderBy( edge => edge.weight ) );
            foreach ( Edge edge in incident_edges )
            {
                if ( !mst_vertices.Contains( edge.vert1 ) || !mst_vertices.Contains( edge.vert2 ) )
                {
                    mst_vertices.Add( edge.vert1 );
                    mst_vertices.Add( edge.vert2 );
                    this.Mst.Add( edge );
                }
            }
        }
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.Root );
}