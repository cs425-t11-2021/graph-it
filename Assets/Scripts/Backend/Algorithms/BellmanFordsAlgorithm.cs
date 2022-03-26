
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BellmanFordsAlgorithm : Algorithm
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
        this.type = AlgorithmType.DISPLAY;
        // Add the root vertex to vertex parms array
        this.vertexParms = new Vertex[] { this.src, this.dest };
    }

    public override void Run()
    {
        if ( this.Graph.Weighted && !this.Graph.FullyWeighted )
            throw new System.Exception( "Graph is not fully weighted." );

        // initialize data
        List< Edge > edges = this.Graph.Adjacency.Values.ToList();
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Edge > prev = new Dictionary< Vertex, Edge >();
        List< Edge > path = new List< Edge >();
        foreach ( Vertex vert in this.Graph.Vertices )
            dist[ vert ] = Double.PositiveInfinity;
        dist[ src ] = 0;

        // relax edges
        for ( int i = 0; i < this.Graph.Order - 1; i++ )
        {
            foreach ( Edge edge in edges )
            {
                if ( dist[ edge.vert1 ] + edge.Weight < dist[ edge.vert2 ] )
                {
                    dist[ edge.vert2 ] = dist[ edge.vert1 ] + edge.Weight;
                    prev[ edge.vert2 ] = edge;
                }
            }
        }

        // check for negative cycles
        foreach ( Edge edge in edges )
        {
            if ( dist[ edge.vert1 ] + edge.Weight < dist[ edge.vert2 ] )
                throw new System.Exception( "Negative weight cycle found." );
        }

        // foreach ( KeyValuePair< Vertex, Edge > kvp in prev )
        // {
        //     Logger.Log( "", this, LogType.INFO );
        //     Logger.Log( kvp.Key.Label, this, LogType.INFO );
        //     Logger.Log( kvp.Value.Label, this, LogType.INFO );
        // }
        // Logger.Log( "", this, LogType.INFO );

        // get path from tree
        Vertex prevVert = this.dest;
        // Logger.Log(prevVert.ToString(), this, LogType.INFO);
        while ( !( prevVert is null ) && prevVert != this.src )
        {
            Edge prevEdge = prev.GetValue( prevVert );
            // Logger.Log((bool) prevEdge?.IncidentOn( prevVert ) ? "true" : "false", this, LogType.INFO);
            // Logger.Log(prevEdge?.Label, this, LogType.INFO);
            path.Add( prevEdge );
            prevVert = prevEdge?.vert1;
        }

        if ( prevVert is null )
            this.Path = null;
        else
            this.Path = path;
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( BellmanFordsAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => BellmanFordsAlgorithm.GetHash( this.src, this.dest );
}
