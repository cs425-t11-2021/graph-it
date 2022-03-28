
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

        // relax edges
        for ( int i = 1; i < this.Graph.Order; i++ )
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Graph.Adjacency )
            {
                if ( dist[ kvp.Key.Item1 ] + kvp.Value.Weight < dist[ kvp.Key.Item2 ] )
                {
                    dist[ kvp.Key.Item2 ] = dist[ kvp.Key.Item1 ] + kvp.Value.Weight;
                    prev[ kvp.Key.Item2 ] = kvp.Value;
                }
            }
        }

        // check for negative cycles
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Graph.Adjacency )
        {
            if ( dist[ kvp.Key.Item1 ] + kvp.Value.Weight < dist[ kvp.Key.Item2 ] )
                throw new System.Exception( "Negative weight cycle found." );
        }

        // get path from tree
        float cost = 0;
        Vertex prevVert = this.dest;
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
                cost += prevEdge.Weight;
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
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( BellmanFordsAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => BellmanFordsAlgorithm.GetHash( this.src, this.dest );
}
