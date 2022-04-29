
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class BellmanFordsAlgorithm : LoggedAlgorithm
{
    private Vertex src;
    private Vertex dest;

    private float cost;
    private List< Edge > edgePath;
    private List< Vertex > vertexPath;

    public BellmanFordsAlgorithm( AlgorithmManager algoManager,  bool display, Vertex src, Vertex dest ) : base( algoManager )
    {
        this.src = src;
        this.dest = dest;

        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
        // Add the root vertex to vertex parms array
        this.vertexParms = new Vertex[] { this.src, this.dest };
    }

    public override void Run()
    {
        if ( this.Graph.Weighted && !this.Graph.FullyWeighted )
            this.CreateError( "Bellman-Ford cannot be executed on partially weighted graph." );

        // initialize data
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Edge > prev = new Dictionary< Vertex, Edge >();
        this.edgePath = new List< Edge >();
        this.vertexPath = new List< Vertex >();
        foreach ( Vertex vert in this.Graph.Vertices )
            dist[ vert ] = float.PositiveInfinity;
        dist[ src ] = 0;

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Set all vertices to distance " + float.PositiveInfinity + " except source which has distance 0.",
            newVerts : new List< Vertex >() { this.src },
            resultVerts : new List< Vertex >() { this.src }
        );

        // add consider step
        this.AddStep(
            StepType.CONSIDER,
            "Update distances between source and all vertices for each individual edge.",
            resultVerts : new List< Vertex >() { this.src },
            considerVerts : this.Graph.Vertices,
            considerEdges : this.Graph.Adjacency.Values
        );

        // relax edges
        for ( int i = 1; i < this.Graph.Order; i++ )
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Graph.Adjacency )
            {
                if ( dist[ kvp.Key.Item1 ] + kvp.Value.Weight < dist[ kvp.Key.Item2 ] )
                {
                    dist[ kvp.Key.Item2 ] = dist[ kvp.Key.Item1 ] + kvp.Value.Weight;
                    prev[ kvp.Key.Item2 ] = kvp.Value;

                    // add result step
                    this.AddStep(
                        StepType.ADD_TO_RESULT,
                        "Update distance between source and all vertices for each individual edge.",
                        newVerts : new List< Vertex >() { kvp.Key.Item1, kvp.Key.Item2 },
                        newEdges : new List< Edge >() { kvp.Value },
                        resultVerts : new List< Vertex >() { this.src }
                    );
                }
            }
        }

        // add consider step
        this.AddStep(
            StepType.CONSIDER,
            "Search for negative weight cycles.",
            considerVerts : this.Graph.Vertices,
            considerEdges : this.Graph.Adjacency.Values
        );

        // check for negative cycles
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Graph.Adjacency )
        {
            if ( dist[ kvp.Key.Item1 ] + kvp.Value.Weight < dist[ kvp.Key.Item2 ] )
            {
                // add error step
                this.AddStep(
                    StepType.ERROR,
                    "Negative weight cycle detected."
                );

                this.CreateError( "Bellman-Ford detected negative weight cycle." );
            }
        }

        // get path from tree
        this.cost = 0;
        Vertex prevVert = this.dest;
        this.vertexPath.Add( prevVert );

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Construct path from destination.",
            resultVerts : this.vertexPath
        );

        while ( !( prevVert is null ) && prevVert != this.src )
        {
            Edge prevEdge = prev.GetValue( prevVert );
            if ( prevEdge is null )
                this.CreateError( "Bellman-Ford failed to construct path." );
            else
            {
                if ( this.edgePath.Contains( prevEdge ) )
                    this.CreateError( "Bellman-Ford failed to construct path." );
                this.edgePath.Add( prevEdge );
                prevVert = prevEdge.vert1 == prevVert ? prevEdge.vert2 : prevEdge.vert1;
                this.vertexPath.Add( prevVert );
                this.cost += prevEdge.Weight;

                // add result step
                this.AddStep(
                    StepType.ADD_TO_RESULT,
                    "Construct path from destination.",
                    newVerts : new List< Vertex >() { prevVert },
                    newEdges : new List< Edge >() { prevEdge },
                    resultVerts : this.vertexPath,
                    resultEdges : this.edgePath
                );
            }
        }

        if ( prevVert is null )
            this.CreateError( "Bellman-Ford failed to construct path." );
        else
            this.edgePath.Reverse();

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Bellman-Ford's Algorithm finished.",
            resultVerts : vertexPath,
            resultEdges : edgePath
        );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "bellmanford cost" ] = ( this.cost, typeof ( float ) );
        result.results[ "bellmanford vertices" ] = ( this.vertexPath, typeof ( List< Vertex > ) );
        result.results[ "bellmanford edges" ] = ( this.edgePath, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( BellmanFordsAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => BellmanFordsAlgorithm.GetHash( this.src, this.dest );
}
