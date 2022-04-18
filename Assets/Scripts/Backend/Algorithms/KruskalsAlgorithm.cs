
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class KruskalsAlgorithm : LoggedAlgorithm
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
        if (this.Graph.Directed)
        {
            RunInMain.Singleton.queuedTasks.Enqueue(() => NotificationManager.Singleton.CreateNotification("<color=red>Kruskal's algorithm is unsupported on directed graphs.</color>", 3));
            throw new System.Exception("Kruskal's algorithm is unsupported on directed graphs.");
        }

        List< Edge > mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( this.Graph.Adjacency.Values.OrderBy( edge => edge.Weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        foreach ( Vertex vert in this.Graph.Vertices )
            forest.Add( new HashSet< Vertex >() { vert } );

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Add each vertex to its own tree.",
            new List< Vertex >( this.Graph.Vertices ),
            null,
            null,
            null,
            null,
            null
        );

        for ( int i = 0; i < edges.Count; ++i )
        {
            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Find minimally weighted edge that connects disjoint trees.",
                new List< Vertex >( this.Graph.Vertices ),
                new List< Edge >( mst ),
                null,
                null,
                new List< Vertex >( Edge.GetIncidentVertices( edges.GetRange( i, edges.Count - i ) ) ),
                new List< Edge >( edges.GetRange( i, edges.Count - i ) )
            );

            HashSet< Vertex > tree1 = KruskalsAlgorithm.GetComponentOf( forest, edges[ i ].vert1 );
            HashSet< Vertex > tree2 = KruskalsAlgorithm.GetComponentOf( forest, edges[ i ].vert2 );
            if ( tree1 != tree2 )
            {
                forest.Remove( tree1 );
                tree2.UnionWith( tree1 );
                mst.Add( edges[ i ] );

                // add result step
                this.AddStep(
                    StepType.ADD_TO_RESULT,
                    "Add minimally weighted edge with weight " + edges[ i ].Weight + " that connects disjoint trees.",
                    new List< Vertex >( this.Graph.Vertices ),
                    new List< Edge >( mst ),
                    new List< Vertex >() { edges[ i ].vert1, edges[ i ].vert2 },
                    new List< Edge >() { edges[ i ] },
                    null,
                    null
                );
            }
        }
        this.Mst = mst;

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Kruskals's Algorithm finished.",
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( mst ),
            null,
            null,
            null,
            null
        );
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
