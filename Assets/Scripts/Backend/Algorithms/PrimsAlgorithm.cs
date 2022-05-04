
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class PrimsAlgorithm : LoggedAlgorithm
{
    private Vertex root;

    private float cost;
    private List< Vertex > mstVertices;
    private List< Edge > mstEdges;

    public PrimsAlgorithm( AlgorithmManager algoManager,  bool display, Vertex root ) : base( algoManager )
    {
        if ( !this.Graph.Vertices.Contains( root ) )
            this.CreateError( "Vertex for Prim's algorithm is not in graph." );
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
            this.CreateError( "Prim's algorithm is unsupported on directed graphs." );

        this.mstEdges = new List< Edge >();
        HashSet< Vertex > mstVerticesSet = new HashSet< Vertex >() { this.root };

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Add root to tree.",
            newVerts : mstVerticesSet,
            resultVerts : mstVerticesSet
        );

        int mstVerticesPrevCount = -1;
        while ( mstVerticesPrevCount != mstVerticesSet.Count )
        {
            mstVerticesPrevCount = mstVerticesSet.Count;
            IEnumerable< Edge > incidentEdges = this.Graph.GetIncidentEdges( mstVerticesSet ).OrderBy( edge => edge.Weight );

            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Find minimally weighted incident edge.",
                resultVerts : mstVerticesSet,
                resultEdges : this.mstEdges,
                considerVerts : Edge.GetIncidentVertices( incidentEdges ),
                considerEdges : incidentEdges
            );

            foreach ( Edge edge in incidentEdges )
            {
                if ( !mstVerticesSet.Contains( edge.vert1 ) || !mstVerticesSet.Contains( edge.vert2 ) )
                {
                    mstVerticesSet.Add( edge.vert1 );
                    mstVerticesSet.Add( edge.vert2 );
                    this.mstEdges.Add( edge );

                    // add result step
                    this.AddStep(
                        StepType.ADD_TO_RESULT,
                        "Add minimally weighted incident edge with weight " + edge.Weight,
                        newVerts : new List< Vertex >() { edge.vert1, edge.vert2 },
                        newEdges : new List< Edge >() { edge },
                        resultVerts : mstVerticesSet,
                        resultEdges : this.mstEdges,
                        considerVerts : Edge.GetIncidentVertices( incidentEdges ),
                        considerEdges : incidentEdges
                    );
                    break;
                }
            }
        }
        this.mstVertices = new List< Vertex >( mstVerticesSet );
        this.cost = this.mstEdges.Select( e => e.Weight ).Aggregate( ( sum, w ) => sum + w );

        // add finished step
        this.AddStep(
            StepType.FINISHED,
            "Prim's Algorithm finished.",
            resultVerts : this.mstVertices,
            resultEdges : this.mstEdges
        );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "prim cost" ] = ( this.cost, typeof ( float ) );
        result.results[ "prim mst vertices" ] = ( this.mstVertices, typeof ( List< Vertex > ) );
        result.results[ "prim mst edges" ] = ( this.mstEdges, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash( Vertex vert ) => ( typeof ( PrimsAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => PrimsAlgorithm.GetHash( this.root );
}
