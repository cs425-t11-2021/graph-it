
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BellmanFordsAlgorithm : LoggedAlgorithm
{
    public float Cost { get; private set; }
    public List< Edge > Path { get; private set; }
    private Vertex src;
    private Vertex dest;

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
            throw new System.Exception( "Graph is not fully weighted." );

        // initialize data
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Edge > prev = new Dictionary< Vertex, Edge >();
        List< Edge > path = new List< Edge >();
        foreach ( Vertex vert in this.Graph.Vertices )
            dist[ vert ] = Single.PositiveInfinity;
        dist[ src ] = 0;

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

        // add consider step
        this.AddStep(
            StepType.CONSIDER,
            "Update distances between source and all vertices for each individual edge.",
            new List< Vertex >() { this.src },
            null,
            null,
            null,
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( this.Graph.Adjacency.Values )
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
                        new List< Vertex >() { this.src },
                        null,
                        new List< Vertex >() { kvp.Key.Item1, kvp.Key.Item2 },
                        new List< Edge >() { kvp.Value },
                        null,
                        null
                    );
                }
            }
        }

        // add consider step
        this.AddStep(
            StepType.CONSIDER,
            "Search for negative weight cycles.",
            null,
            null,
            null,
            null,
            new List< Vertex >( this.Graph.Vertices ),
            new List< Edge >( this.Graph.Adjacency.Values )
        );

        // check for negative cycles
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Graph.Adjacency )
        {
            if ( dist[ kvp.Key.Item1 ] + kvp.Value.Weight < dist[ kvp.Key.Item2 ] )
            {
                // add error step
                this.AddStep(
                    StepType.ERROR,
                    "Negative weight cycle detected.",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                );

                throw new System.Exception( "Negative weight cycle found." );
            }
        }

        // get path from tree
        float cost = 0;
        Vertex prevVert = this.dest;
        List< Vertex > vertexPath = new List< Vertex >() { prevVert };

        // add result step
        this.AddStep(
            StepType.ADD_TO_RESULT,
            "Construct path from destination.",
            vertexPath,
            null,
            null,
            null,
            null,
            null
        );

        while ( !( prevVert is null ) && prevVert != this.src )
        {
            Edge prevEdge = prev.GetValue( prevVert );
            if ( prevEdge is null )
                throw new System.Exception( "Path could not be found." );
            else
            {
                if ( path.Contains( prevEdge ) )
                    throw new System.Exception( "Path could not be found." );
                path.Add( prevEdge );
                prevVert = prevEdge.vert1 == prevVert ? prevEdge.vert2 : prevEdge.vert1;
                vertexPath.Add( prevVert );
                cost += prevEdge.Weight;

                // add result step
                this.AddStep(
                    StepType.ADD_TO_RESULT,
                    "Construct path from destination.",
                    vertexPath,
                    new List< Edge >( path ),
                    new List< Vertex >() { prevVert },
                    new List< Edge >() { prevEdge },
                    null,
                    null
                );
            }
        }

        if ( prevVert is null )
            throw new System.Exception( "Path could not be found." );
        else
        {
            path.Reverse();
            this.Path = path;
            this.Cost = cost;
        }

        // add finish step
        this.AddStep(
            StepType.FINISHED,
            "Bellman-Ford's Algorithm finished.",
            new List< Vertex >( vertexPath ),
            new List< Edge >( path ),
            null,
            null,
            null,
            null
        );
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( BellmanFordsAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => BellmanFordsAlgorithm.GetHash( this.src, this.dest );
}
