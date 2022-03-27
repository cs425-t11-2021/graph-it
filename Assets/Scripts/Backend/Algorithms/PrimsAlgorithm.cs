
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class PrimsAlgorithm : Algorithm
{
    // TODO: root should be private
    public List< Edge > Mst { get; private set; }
    private Vertex root;

    public PrimsAlgorithm( AlgorithmManager algoManager,  bool display, Vertex root ) : base( algoManager )
    {
        if ( !this.Graph.Vertices.Contains( root ) )
            throw new System.Exception( "Vertex for Prim's algorithm is not in graph." );
        this.root = root;
        
        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
        // Add the root vertex to vertex parms array
        vertexParms = new Vertex[] { this.root };
    }

    public override void Run()
    {
        if ( this.Graph.Directed )
        {
            // Debug.Log( ( new System.Exception( "Prim's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            RunInMain.Singleton.queuedTasks.Enqueue(() => NotificationManager.Singleton.CreateNotification("<color=red>Prim's algorithm is unsupported on directed graphs.</color>", 3));
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

        List< Edge > mst = new List< Edge >();
        HashSet< Vertex > mstVertices = new HashSet< Vertex >() { this.root };
        int mstVerticesPrevCount = -1;
        while ( mstVerticesPrevCount != mstVertices.Count )
        {
            mstVerticesPrevCount = mstVertices.Count;
            IEnumerable< Edge > incidentEdges = this.Graph.GetIncidentEdges( mstVertices ).OrderBy( edge => edge.Weight );
            foreach ( Edge edge in incidentEdges )
            {
                if ( !mstVertices.Contains( edge.vert1 ) || !mstVertices.Contains( edge.vert2 ) )
                {
                    mstVertices.Add( edge.vert1 );
                    mstVertices.Add( edge.vert2 );
                    mst.Add( edge );
                    break;
                }
            }
        }
        this.Mst = mst;
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.root );
}
