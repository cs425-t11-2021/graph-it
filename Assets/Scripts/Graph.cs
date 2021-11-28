using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Vertex : IEquatable< Vertex >
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

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( id, label, x_pos, y_pos, color, style, label_style ).GetHashCode();

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );
}


public struct Edge : IEquatable< Edge >
{
    public int id;
    public Vertex vert1, vert2;
    public string label;
    public double weight; // NOTE: if label is a weight, we store numerical weight here for quicker computations
    public bool directed;

    public int style;
    public int thickness;
    
    // when undirected, following styles should be ignored
    public int tail_style;
    public int head_style;

    public Edge( int id, Vertex vert1, Vertex vert2, string label="", double weight=0, bool directed=false, int style=0, int thickness=0, int tail_style=0, int head_style=0 )
    {
        this.id = id;
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.label = label;
        this.weight = weight;
        this.directed = directed;
        this.style = style;
        this.thickness = thickness;
        this.tail_style = tail_style;
        this.head_style = head_style;
    }

    public bool IncidentOn( Vertex vert )
    {
        return vert == this.vert1 || vert == this.vert2;
    }

    public override bool Equals( object obj ) => obj is Edge other && this.Equals( other );

    public bool Equals( Edge edge ) => this.id == edge.id;

    public override int GetHashCode() => ( id, vert1, vert2, label, weight, directed, style, thickness, tail_style, head_style ).GetHashCode();

    public static bool operator ==( Edge lhs, Edge rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Edge lhs, Edge rhs) => !( lhs == rhs );
}


public class Graph
{
    public List< Vertex > vertices;
    public Dictionary< int, List< Edge > > incidence;

    private int next_vert_id = 0;
    private int next_edge_id = 0;

    // TODO: default settings
    public bool directed;

    public Graph()
    {
        this.vertices = new List< Vertex >();
        this.incidence = new Dictionary< int, List< Edge > >();
        this.directed = false;
    }

    ~Graph() {} // TODO

    // assuming vertex is created via user interface and so position is specified
    public void AddVertex( Nullable< double > x_pos=null, Nullable< double > y_pos=null)
    {
        this.vertices.Add( new Vertex( this.next_vert_id, x_pos : x_pos, y_pos : y_pos ) );
        this.incidence.Add( this.next_vert_id, new List< Edge >() );
        this.next_vert_id++;
    }

    public void AddEdge( Vertex vert1, Vertex vert2 )
    {
        if ( !this.incidence.ContainsKey( vert1.id ) && !this.incidence.ContainsKey( vert2.id ) || !this.vertices.Contains( vert1 ) && !this.vertices.Contains( vert2 ) )
            throw new System.Exception( "One or more vertices have not been added to the graph." );

        this.incidence[ vert1.id ].Add( new Edge( this.next_edge_id++, vert1, vert2 ) );
        if ( !this.directed )
            this.incidence[ vert2.id ].Add( new Edge( this.next_edge_id++, vert2, vert1 ) );
    }

    // TODO: determine if input should be vertex or vertex id
    public Vertex RemoveVertex( Vertex vert )
    {
        this.incidence.Remove( vert.id ); // TODO: ensure all edges are unallocated

        foreach ( KeyValuePair< int, List< Edge > > kvp in this.incidence )
        {
            foreach ( Edge edge in kvp.Value )
            {
                if ( edge.IncidentOn( vert ) )
                    kvp.Value.Remove( edge );
            }
        }
        return vert;
    }

    public Edge RemoveEdge( Edge edge )
    {
        foreach ( KeyValuePair< int, List< Edge > > kvp in this.incidence )
            kvp.Value.Remove( edge );
        return edge;
    }

    public void Import() {} // TODO

    public void Export() {} // TODO
}
