
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DijkstrasAlgorithm : LoggedAlgorithm
{
    private Vertex src;
    private Vertex dest;

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
        vertexParms = new Vertex[] { this.src, this.dest };
    }

    public override void Run()
    {
        this.vertexPath = new List< Vertex >();
        this.edgePath = new List< Edge >();
        HashSet< Vertex > visited = new HashSet< Vertex >(); // for step through
        HashSet< Edge > visitedEdges = new HashSet< Edge >(); // for step through
        HashSet< Vertex > notVisited = new HashSet< Vertex >( this.Graph.Vertices );
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

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
            new List< Vertex >() { this.src },
            null,
            new List< Vertex >() { this.src },
            null,
            null,
            null
        );

        while ( notVisited.Count > 0 )
        {
            // add consider step
            this.AddStep(
                StepType.CONSIDER,
                "Search for closest non-visited vertex.",
                new List< Vertex >( visited ),
                new List< Edge >( visitedEdges ),
                null,
                null,
                new List< Vertex >( notVisited ),
                null
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
                new List< Vertex >( visited ),
                new List< Edge >( visitedEdges ),
                new List< Vertex >() { u },
                null,
                new List< Vertex >( notVisited ),
                null
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
                new List< Vertex >( visited ),
                new List< Edge >( visitedEdges ),
                new List< Vertex >() { u },
                new List< Edge >( newVistedEdges ),
                new List< Vertex >( notVisited ),
                null
            );
        }

        this.cost = dist[ this.dest ];

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Obtain path length " + this.cost + " from source to destination.",
            new List< Vertex >( visited ),
            new List< Edge >( visitedEdges ),
            null,
            null,
            null,
            null
        );

        // put together final path 
        Vertex curr = this.dest;
        while ( curr != this.src )
        {
            this.vertexPath.Add( curr );
            this.edgePath.Add( this.Graph[ prev[ curr ], curr ] );
            if ( curr is null )
            {
                // add error step
                this.AddStep(
                    StepType.ERROR,
                    "Path could not be constructed.",
                    new List< Vertex >( this.vertexPath ),
                    new List< Edge >( this.edgePath ),
                    null,
                    null,
                    null,
                    null
                );

                this.CreateError( "Dijkstra's path could not be constructed." );
            }

            // add result step
            this.AddStep(
                StepType.ADD_TO_RESULT,
                "Construct path from destination.",
                new List< Vertex >( this.vertexPath ),
                new List< Edge >( this.edgePath ),
                new List< Vertex >() { curr },
                new List< Edge >() { this.Graph[ prev[ curr ], curr ] },
                null,
                null
            );

            curr = prev[ curr ];
        }
        this.vertexPath.Add( this.src );
        this.vertexPath.Reverse();

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Dijkstra's Algorithm finished.",
            new List< Vertex >( vertexPath ),
            new List< Edge >( edgePath ),
            null,
            null,
            null,
            null
        );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "dijkstra cost" ] = ( this.cost, typeof ( float ) );
        result.results[ "dijkstra vertices" ] = ( this.vertexPath, typeof ( List< Vertex > ) );
        result.results[ "dijkstra edges" ] = ( this.edgePath, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( DijkstrasAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => DijkstrasAlgorithm.GetHash( this.src, this.dest );
}