
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DijkstrasAlgorithm : LoggedAlgorithm
{
    private Vertex src;
    private Vertex dest;

    private bool hasPath;
    private float cost;
    private List< Vertex > vertexPath;
    private List< Edge > edgePath;

    public DijkstrasAlgorithm( AlgorithmManager algoManager, bool display, Vertex src, Vertex dest ) : base( algoManager )
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
        if ( this.Graph.NegativeWeighted )
            this.CreateError( "Dijkstra's algorithm is unsupported on negative weighted graphs." );

        this.vertexPath = new List< Vertex >();
        this.edgePath = new List< Edge >();
        HashSet< Vertex > visited = new HashSet< Vertex >(); // for step through
        HashSet< Edge > visitedEdges = new HashSet< Edge >(); // for step through
        HashSet< Vertex > notVisited = new HashSet< Vertex >( this.Graph.Vertices );
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();
        this.hasPath = true;

        foreach ( Vertex v in this.Graph.Vertices )
        {
            prev[ v ] = null;
            dist[ v ] = float.PositiveInfinity;
        }
        dist[ this.src ] = 0;

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Set all vertices to distance " + float.PositiveInfinity + " except source which has distance 0.",
            newVerts : new List< Vertex >() { this.src },
            resultVerts : new List< Vertex >() { this.src }
        );

        while ( notVisited.Count > 0 )
        {
            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Search for closest non-visited vertex.",
                resultVerts : visited,
                resultEdges: visitedEdges,
                considerVerts : notVisited
            );

            // find u in notVisited such that dist[u] is minimal
            Vertex u = notVisited.First();
            foreach ( Vertex v in notVisited )
            {
                if ( dist[ v ] < dist[ u ] )
                    u = v;
            }

            notVisited.Remove( u );
            visited.Add( u );

            // add result step
            this.AddStep(
                StepType.ADD_TO_RESULT,
                "Add closest non-visited vertex with distance " + dist[ u ] + ".",
                newVerts : new List< Vertex >() { u },
                resultVerts : visited,
                resultEdges : visitedEdges
            );

            // update neighbors of u
            List< Edge > newVistedEdges = new List< Edge >();
            foreach ( Vertex v in notVisited )
            {
                if ( this.Graph.IsAdjacentDirected( u, v ) )
                {
                    float tmp = dist[ u ] + this.Graph[ u, v ].Weight;
                    if ( tmp < dist[ v ] )
                    {
                        dist[ v ] = tmp;
                        prev[ v ] = u;
                        newVistedEdges.Add( this.Graph[ u, v ] );
                    }
                }
            }
            visitedEdges.UnionWith( newVistedEdges );

            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Update all distances of non-visited vertices from newly added vertex.",
                newVerts : new List< Vertex >() { u },
                newEdges : newVistedEdges,
                resultVerts : visited,
                resultEdges : visitedEdges,
                considerVerts : notVisited
            );
        }

        this.cost = dist[ this.dest ];

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Obtain path length " + this.cost + " from source to destination.",
            resultVerts : visited,
            resultEdges : visitedEdges
        );

        // put together final path 
        Vertex curr = this.dest;
        while ( !( curr is null ) && curr != this.src )
        {
            this.vertexPath.Add( curr );
            this.edgePath.Add( this.Graph[ prev[ curr ], curr ] );

            // add result step
            this.AddStep(
                StepType.ADD_TO_RESULT,
                "Construct path from destination.",
                newVerts : new List< Vertex >() { curr },
                newEdges : new List< Edge >() { this.Graph[ prev[ curr ], curr ] },
                resultVerts : this.vertexPath,
                resultEdges : this.edgePath
            );

            curr = prev[ curr ];
        }
        if ( curr is null )
        {
            // add error step
            this.AddStep(
                StepType.ERROR,
                "Path could not be constructed. Cost " + float.PositiveInfinity + ".",
                resultVerts : this.vertexPath,
                resultEdges : this.edgePath
            );
            this.hasPath = false;
        }

        this.vertexPath.Add( this.src );
        this.vertexPath.Reverse();

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Dijkstra's Algorithm finished.",
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
        result.results[ "dijkstra has path" ] = ( this.hasPath, typeof ( bool ) );
        result.results[ "dijkstra cost" ] = ( this.cost, typeof ( float ) );
        result.results[ "dijkstra vertices" ] = ( this.vertexPath, typeof ( List< Vertex > ) );
        result.results[ "dijkstra edges" ] = ( this.edgePath, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( DijkstrasAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => DijkstrasAlgorithm.GetHash( this.src, this.dest );
}