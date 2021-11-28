using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Vertex
{
    public int id;
    public string label;
    public Nullable< double > x_pos, y_pos;

    public int color; // NOTE: int may not suffice, feel free to change
    public int style; // all possible visual styles of vertices. NOTE: could make static to make all vertices share same style
    public int label_style; // NOTE: may be unnecessary

    // default value position is null. NOTE: feel free to change if better convention is known
    public Vertex( int id, string label="", Nullable< double > x_pos=null, Nullable< double > y_pos=null, int color=0, int style=0, int label_style=0 )
    {
        this.id = id;
        this.label = label;
        this.x_pos = x_pos;
        this.y_pos = y_pos;
        this.color = color;
        this.style = style;
        this.label_style = label_style;
    }
}


public struct Edge
{
    public int id;
    public Tuple< Vertex, Vertex > incidence; // NOTE: can act as directed or undirected, avoiding KeyValuePair as a key and value doesn't perfectly describe the incidence relationship
    public string label;
    public double weight; // NOTE: if label is a weight, we store numerical weight here for quicker computations
    public bool directed;

    public int style;
    public int thickness;
    
    // when undirected, following styles should be ignored
    public int tail_style;
    public int head_style;

    public Edge( int id, Tuple< Vertex, Vertex > incidence, string label="", double weight=0, bool directed=false, int style=0, int thickness=0, int tail_style=0, int head_style=0 )
    {
        this.id = id;
        this.label = label;
        this.weight = weight;
        this.directed = directed;
        this.incidence = incidence;
        this.style = style;
        this.thickness = thickness;
        this.tail_style = tail_style;
        this.head_style = head_style;
    }
}


public class Graph
{
    public List< Vertex > vertices;
    public Dictionary< int, List< Edge > > adj;

    private int next_vert_id = 0;
    private int next_edge_id = 0;

    public Graph()
    {
        this.vertices = new List< Vertex >();
        this.adj = new Dictionary< int, List< Edge > >();
    }

    ~Graph() {} // TODO

    // assuming vertex is created via user interface and so position is specified
    public void AddVertex( double x_pos, double y_pos )
    {
        this.vertices.Add( new Vertex( id : this.next_vert_id, x_pos : x_pos, y_pos : y_pos ) );
        this.adj.Add( this.next_vert_id, new List< Edge >() );
        this.next_vert_id++;
    }

    public void AddEdge( Vertex vert1, Vertex vert2 )
    {
        if ( !this.adj.ContainsKey( vert1.id ) && !this.adj.ContainsKey( vert2.id ) || !this.vertices.Contains( vert1 ) && !this.vertices.Contains( vert2 ) )
            throw new System.Exception( "One or more vertices have not been added to the graph." );

        // TODO: must determine when an edge will be directed or undirected by default
        // if ( vert1.id == vert2.id )
        //     adj[ vert1.id ].Add( new Edge( id : this.next_edge_id++, incidence : new Tuple< Vertex, Vertex >( vert1, vert2 ) ) );
        // else
        // {
        //     adj[ vert1.id ].Add( new Edge( id : this.next_edge_id++, incidence : new Tuple< Vertex, Vertex >( vert1, vert2 ) ) );
        //     adj[ vert2.id ].Add( new Edge( id : this.next_edge_id++, incidence : new Tuple< Vertex, Vertex >( vert1, vert2 ) ) );
        // }
        this.adj[ vert1.id ].Add( new Edge( id : this.next_edge_id, incidence : new Tuple< Vertex, Vertex >( vert1, vert2 ) ) );
        this.next_vert_id++;
    }

    // TODO: determine if input should be vertex or vertex id
    public Vertex RemoveVertex( Vertex vert )
    {
        this.adj.Remove( vert.id ); // TODO: ensure all edges are unallocated

        foreach ( KeyValuePair< int, List< Edge > > kvp in this.adj )
        {
            foreach ( Edge edge in kvp.Value )
            {
                if ( edge.incidence.Item1.id == vert.id || edge.incidence.Item2.id == vert.id )
                    kvp.Value.Remove( edge );
            }
        }
        return vert;
    }

    public Edge RemoveEdge( Edge edge )
    {
        foreach ( KeyValuePair< int, List< Edge > > kvp in this.adj )
            kvp.Value.Remove( edge );
        return edge;
    }

    public void Import() {} // TODO

    public void Export() {} // TODO
}
