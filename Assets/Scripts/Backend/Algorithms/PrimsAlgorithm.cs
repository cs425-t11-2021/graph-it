
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
        if ( !this.Graph.Vertices.Contains( root ) )
            throw new System.Exception( "Vertex for Prim's algorithm is not in graph." );
        this.Root = root;
    }

    public override void Run()
    {
        if ( this.Graph.Directed )
        {
            // Debug.Log( ( new System.Exception( "Prim's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

        this.Mst = new List< Edge >();
        HashSet< Vertex > mstVertices = new HashSet< Vertex >() { this.Root };
        int mstVerticesPrevCount = -1;
        while ( mstVerticesPrevCount != mstVertices.Count )
        {
            mstVerticesPrevCount = mstVertices.Count;
            List< Edge > incidentEdges = new List< Edge >( this.Graph.GetIncidentEdges( mstVertices ).OrderBy( edge => edge.weight ) );
            foreach ( Edge edge in incidentEdges )
            {
                if ( !mstVertices.Contains( edge.vert1 ) || !mstVertices.Contains( edge.vert2 ) )
                {
                    mstVertices.Add( edge.vert1 );
                    mstVertices.Add( edge.vert2 );
                    this.Mst.Add( edge );
                }
            }
        }
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.Root );
}
