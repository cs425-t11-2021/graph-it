
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class PrimsAlgorithm : Algorithm
{
    private Vertex vert;

    List< Edge > mst;

    public PrimsAlgorithm( Graph graph, Vertex vert, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning )
    {
        if ( !this.graph.vertices.Contains( vert ) )
            throw new System.Exception( "Vertex for Prim's algorithm is not in graph." );
        this.vert = vert;
    }

    public override void Run()
    {
        if ( this.graph.directed )
        {
            // Debug.Log( ( new System.Exception( "Prim's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

        this.mst = new List< Edge >();
        HashSet< Vertex > mst_vertices = new HashSet< Vertex >() { this.vert };
        int mst_vertices_prev_count = -1;
        while ( mst_vertices_prev_count != mst_vertices.Count )
        {
            mst_vertices_prev_count = mst_vertices.Count;
            List< Edge > incident_edges = new List< Edge >( this.GetIncidentEdges( mst_vertices ).OrderBy( edge => edge.weight ) );
            foreach ( Edge edge in incident_edges )
            {
                if ( !mst_vertices.Contains( edge.vert1 ) || !mst_vertices.Contains( edge.vert2 ) )
                {
                    mst_vertices.Add( edge.vert1 );
                    mst_vertices.Add( edge.vert2 );
                    this.mst.Add( edge );
                }
            }
        }
    }

    private List< Edge > GetIncidentEdges( Vertex vert )
    {
        return this.GetIncidentEdges( new HashSet< Vertex >() { vert } );
    }

    private List< Edge > GetIncidentEdges( HashSet< Vertex > verts )
    {
        List< Edge > incident_edges = new List< Edge >();
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.graph.adjacency )
        {
            if ( verts.Contains( kvp.Value.vert1 ) || kvp.Value.directed && verts.Contains( kvp.Value.vert2 ) )
                incident_edges.Add( kvp.Value );
        }
        return incident_edges;
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.vert );
}
