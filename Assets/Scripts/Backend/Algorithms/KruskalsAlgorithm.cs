
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class KruskalsAlgorithm : LoggedAlgorithm
{
    private float cost;
    private List< Vertex > mstVertices;
    private List< Edge > mstEdges;

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
            this.CreateError( "Kruskal's algorithm is unsupported on directed graphs." );

        this.mstVertices = new List< Vertex >( this.Graph.Vertices );
        this.mstEdges = new List< Edge >();
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
            new List< Vertex >( this.Graph.Vertices ),
            null,
            null,
            null
        );

        // add consider step
        this.AddStep(
            StepType.CONSIDER,
            "Find minimally weighted edge that connects disjoint trees.",
            null,
            null,
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( this.mstEdges ),
            new List< Vertex >( Edge.GetIncidentVertices( edges ) ),
            new List< Edge >( edges )
        );

        for ( int i = 0; i < edges.Count; ++i )
        {
            HashSet< Vertex > tree1 = KruskalsAlgorithm.GetComponentOf( forest, edges[ i ].vert1 );
            HashSet< Vertex > tree2 = KruskalsAlgorithm.GetComponentOf( forest, edges[ i ].vert2 );
            if ( tree1 != tree2 )
            {
                forest.Remove( tree1 );
                tree2.UnionWith( tree1 );
                this.mstEdges.Add( edges[ i ] );

                // add result step
                this.AddStep(
                    StepType.ADD_TO_RESULT,
                    "Add minimally weighted edge with weight " + edges[ i ].Weight + " that connects disjoint trees.",
                    new List< Vertex >() { edges[ i ].vert1, edges[ i ].vert2 },
                    new List< Edge >() { edges[ i ] },
                    new List< Vertex >( this.Graph.Vertices ),
                    new List< Edge >( this.mstEdges ),
                    null,
                    null
                );

                // add consider step
                this.AddStep(
                    StepType.CONSIDER,
                    "Find minimally weighted edge that connects disjoint trees.",
                    null,
                    null,
                    new List< Vertex >( this.Graph.Vertices ),
                    new List< Edge >( this.mstEdges ),
                    new List< Vertex >( Edge.GetIncidentVertices( edges.GetRange( i, edges.Count - i ) ) ),
                    new List< Edge >( edges.GetRange( i, edges.Count - i ) )
                );
            }
        }

        this.cost = this.mstEdges.Select( e => e.Weight ).Aggregate( ( sum, w ) => sum + w );

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Kruskal's Algorithm finished.",
            null,
            null,
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( this.mstEdges ),
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
        throw new System.Exception( "Kruskal could not find vertex in collection of components." ); // TODO: make custom error
        // this.CreateError( "Kruskal could not find vertex in collection of components." );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "kruskal cost" ] = ( this.cost, typeof ( float ) );
        result.results[ "kruskal mst vertices" ] = ( this.mstVertices, typeof ( List< Vertex > ) );
        result.results[ "kruskal mst edges" ] = ( this.mstEdges, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash() => typeof ( KruskalsAlgorithm ).GetHashCode();

    public override int GetHashCode() => KruskalsAlgorithm.GetHash();
}
