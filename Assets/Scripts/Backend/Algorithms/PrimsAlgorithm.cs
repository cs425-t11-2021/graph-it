
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class PrimsAlgorithm : LoggedAlgorithm
{
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
            RunInMain.Singleton.queuedTasks.Enqueue( () => NotificationManager.Singleton.CreateNotification( "<color=red>Prim's algorithm is unsupported on directed graphs.</color>", 3 ) );
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

        List< Edge > mst = new List< Edge >();
        HashSet< Vertex > mstVertices = new HashSet< Vertex >() { this.root };

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Add root to tree.",
            new List< Vertex >( mstVertices ),
            null,
            new List< Vertex >( mstVertices ),
            null,
            null,
            null
        );

        int mstVerticesPrevCount = -1;
        while ( mstVerticesPrevCount != mstVertices.Count )
        {
            mstVerticesPrevCount = mstVertices.Count;
            IEnumerable< Edge > incidentEdges = this.Graph.GetIncidentEdges( mstVertices ).OrderBy( edge => edge.Weight );

            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Find minimally weighted incident edge.",
                new List< Vertex >( mstVertices ),
                new List< Edge >( mst ),
                null,
                null,
                new List< Vertex >( Edge.GetIncidentVertices( incidentEdges ) ),
                new List< Edge >( incidentEdges )
            );

            foreach ( Edge edge in incidentEdges )
            {
                if ( !mstVertices.Contains( edge.vert1 ) || !mstVertices.Contains( edge.vert2 ) )
                {
                    mstVertices.Add( edge.vert1 );
                    mstVertices.Add( edge.vert2 );
                    mst.Add( edge );
                    // add result step
                    this.AddStep(
                        StepType.ADD_TO_RESULT,
                        "Add minimally weighted incident edge with weight " + edge.Weight,
                        new List< Vertex >( mstVertices ),
                        new List< Edge >( mst ),
                        new List< Vertex >() { edge.vert1, edge.vert2 },
                        new List< Edge >() { edge },
                        new List< Vertex >( Edge.GetIncidentVertices( incidentEdges ) ),
                        new List< Edge >( incidentEdges )
                    );
                    break;
                }
            }
        }
        this.Mst = mst;

        // add finished step
        this.AddStep(
            StepType.FINISHED,
            "Prim's Algorithm finished.",
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( mst ),
            null,
            null,
            null,
            null
        );
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.root );
}
