using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Vertex : IEquatable< Vertex >
{
    // TODO: make id long
    public int id;
    public string label;
    public double? x_pos, y_pos;

    public int style;
    public int color;
    public int label_style;

    // default value position is null
    public Vertex( int id, string label="", double? x_pos=null, double? y_pos=null, int style=0, int color=0, int label_style=0 )
    {
        this.id = id;
        this.label = label;
        this.x_pos = x_pos;
        this.y_pos = y_pos;
        this.style = style;
        this.color = color;
        this.label_style = label_style;
    }

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( id, label, x_pos, y_pos, style, color, label_style ).GetHashCode();

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    public static bool operator <=( Vertex lhs, Vertex rhs ) => lhs < rhs || lhs == rhs;

    public static bool operator >( Vertex lhs, Vertex rhs ) => !( lhs <= rhs );

    public static bool operator >=( Vertex lhs, Vertex rhs ) => !( lhs < rhs );

    public override string ToString() => String.Format( "id: {0}, label: {1}, x_pos: {2}, y_pos: {3}, style: {4}, color: {5}, label_style: {6}", this.id, this.label, this.x_pos, this.y_pos, this.style, this.color, this.label_style );
}


public struct Edge : IEquatable< Edge >
{
    // TODO: make id long
    public int id;
    public Vertex vert1, vert2;
    public string label;
    public double weight;
    public bool directed;

    public int style;
    public int color;
    public int thickness;
    public int label_style;

    // when undirected, following styles should be ignored
    // public int tail_style;
    // public int head_style;

    // public Edge( int id, Vertex vert1, Vertex vert2, string label="", double weight=1, bool directed=false, int style=0, int color=0, int thickness=0, int label_style=0, int tail_style=0, int head_style=0 )
    public Edge( int id, Vertex vert1, Vertex vert2, string label="", double weight=1, bool directed=false, int style=0, int color=0, int thickness=0, int label_style=0 )
    {
        this.id = id;
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.label = label;
        this.weight = weight;
        this.directed = directed;
        this.style = style;
        this.color = color;
        this.thickness = thickness;
        this.label_style = label_style;
        // this.tail_style = tail_style;
        // this.head_style = head_style;
    }

    public void ResetWeight() => this.weight = 1;

    public bool IncidentOn( int id ) => id == this.vert1.id || id == this.vert2.id;

    public bool IncidentOn( Vertex vert ) => vert == this.vert1 || vert == this.vert2;

    public override bool Equals( object obj ) => obj is Edge other && this.Equals( other );

    public bool Equals( Edge edge ) => this.id == edge.id;

    // public override int GetHashCode() => ( id, vert1.id, vert2.id, label, weight, directed, style, color, thickness, tail_style, head_style ).GetHashCode();
    public override int GetHashCode() => ( id, vert1.id, vert2.id, label, weight, directed, style, color, thickness ).GetHashCode();

    public static bool operator ==( Edge lhs, Edge rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Edge lhs, Edge rhs) => !( lhs == rhs );

    // public override string ToString() => String.Format( "id: {0}, vert1: {1}, vert2: {2}, label: {3}, weight: {4}, directed: {5}, style: {6}, color: {7}, thickness: {8}, label_style: {9}, tail_style: {10}, head_style: {11}", this.id, this.vert1.id, this.vert2.id, this.label, this.weight, this.directed, this.style, this.color, this.thickness, this.label_style, this.tail_style, this.head_style );
    public override string ToString() => String.Format( "id: {0}, vert1: {1}, vert2: {2}, label: {3}, weight: {4}, directed: {5}, style: {6}, color: {7}, thickness: {8}, label_style: {9}", this.id, this.vert1.id, this.vert2.id, this.label, this.weight, this.directed, this.style, this.color, this.thickness, this.label_style );
}


public class Graph
{
    // Vertices list is read-only from outside of class
    public List< Vertex > vertices { get; private set; }
    // Incidence dict is read-only from outside of class
    public Dictionary< ( int, int ), Edge > adjacency { get; private set; }

    // default settings
    // private bool  directed = true;

    // TODO: convert to long
    private int next_vert_id = 0;
    private int next_edge_id = 0;


    public Graph()
    {
        this.vertices = new List< Vertex >();
        this.adjacency = new Dictionary< ( int, int ), Edge >();
    }

    public Graph( Graph graph )
    {
        this.vertices = new List< Vertex >( graph.vertices );
        this.adjacency = new Dictionary< ( int, int ), Edge >( graph.adjacency );
        this.next_vert_id = graph.next_vert_id;
        this.next_edge_id = graph.next_edge_id;
    }

    public void Clear()
    {
        this.vertices = null;
        this.adjacency = null;
        this.next_vert_id = 0;
        this.next_edge_id = 0;
    }
    
    public Vertex this[ int id ] // TODO: ensure one can modify parameters of vertices, e.g. graph[ id ].style = 3;
    {
        get => this.GetVertex( id );
    }

    public Edge this[ int id1, int id2 ]
    {
        get => this.adjacency[ ( id1, id2 ) ];
    }

    public void AddVertex( double? x_pos=null, double? y_pos=null )
    {
        this.AddVertex( new Vertex( this.next_vert_id, x_pos : x_pos, y_pos : y_pos ) );
    }

    public void AddVertex( Vertex vert )
    {
        this.vertices.Add( vert );
        // this.adjacency.Add( vert.id, new List< Edge >() );
        this.next_vert_id++;
    }

    public void AddEdge( Vertex vert1, Vertex vert2 )
    {
        if ( !this.vertices.Contains( vert1 ) || !this.vertices.Contains( vert2 ) )
            Debug.Log( ( new System.Exception( "One or more vertices have not been added to the graph." ) ).ToString() );

        if ( vert1 < vert2 )
            this.adjacency[ ( vert1.id, vert2.id ) ] = new Edge( this.next_edge_id, vert1, vert2 );
        else
            this.adjacency[ ( vert2.id, vert1.id ) ] = new Edge( this.next_edge_id, vert2, vert1 );
        this.next_edge_id++;
    }

    public void AddEdge( Edge edge )
    {
        if ( !this.vertices.Contains( edge.vert1 ) || !this.vertices.Contains( edge.vert2 ) )
            Debug.Log( ( new System.Exception( "One or more vertices have not been added to the graph." ) ).ToString() );
        if ( edge.vert1 >= edge.vert2 )
            Debug.Log( ( new System.Exception( "Invalid edge added to graph." ) ).ToString() );

        this.adjacency[ ( edge.vert1.id, edge.vert2.id ) ] = edge;
        this.next_edge_id++;
    }

    public Vertex RemoveVertex( int id )
    {
        return this.RemoveVertex( this.GetVertex( id ) );
    }

    public Vertex RemoveVertex( Vertex vert )
    {
        this.vertices.Remove( vert );

        foreach ( KeyValuePair< ( int, int ), Edge > kvp in this.adjacency )
        {
            if ( kvp.Value.IncidentOn( vert ) )
                this.adjacency.Remove( kvp.Key );
        }
        return vert;
    }

    public Edge RemoveEdge( int id )
    {
        return this.RemoveEdge( this.GetEdge( id ) );
    }

    public Edge RemoveEdge( Edge edge )
    {
        this.adjacency.Remove( ( edge.vert1.id, edge.vert2.id ) );
        this.adjacency.Remove( ( edge.vert2.id, edge.vert1.id ) );
        return edge;
    }

    private Vertex GetVertex( int id )
    {
        foreach ( Vertex vert in this.vertices )
        {
            if ( vert.id == id )
                return vert;
        }
        Debug.Log( ( new System.Exception( "Vertex could not be found." ) ).ToString() );
        throw new System.Exception( "Vertex could not be found." );
    }

    private Edge GetEdge( int id )
    {
        foreach ( KeyValuePair< ( int, int ), Edge > kvp in this.adjacency )
        {
            if ( kvp.Value.id == id )
                return kvp.Value;
        }
        Debug.Log( ( new System.Exception( "Edge could not be found." ) ).ToString() );
        throw new System.Exception( "Edge could not be found." );
    }


    // file io methods ////////////////////////////////////////////////

    // TODO: relax import file formatting
    // NOTE: import does not erase existing graph data, imported data may conflict with any existing ids
    public void Import( string path )
    {
        this.Clear();
        try
        {
            if ( !File.Exists( path ) )
            {
                // Debug.Log( "file not found" );
                Debug.Log( ( new System.Exception( "The provided file cannot be found." ) ).ToString() );
            }

            bool flag = true;
            foreach ( string line in System.IO.File.ReadLines( path ) )
            {
                if ( String.IsNullOrEmpty( line ) )
                    continue;
                if ( line.Trim() == "vertices:" )
                {
                    flag = true;
                    continue;
                }
                if ( line.Trim() == "incidence:" )
                {
                    flag = false;
                    continue;
                }
                this.ParseLine( line, flag );
            }
        }
        catch ( Exception ex )
        {
            // Debug.Log( "exception thrown" );
            Debug.Log( ex.ToString() );
        }
    }

    private void ParseLine( string line, bool flag )
    {
        if ( flag )
            this.AddVertex( this.ParseVertex( line ) );
        else
            this.AddEdge( this.ParseEdge( line ) );
    }

    private Vertex ParseVertex( string line )
    {
        Dictionary< string, string > vect_data = line.Replace( " ", "" )
                                                     .Replace( "{", "" )
                                                     .Replace( "}", "" )
                                                     .Split( ',' )
                                                     .Select( part  => part.Split( ':' ) )
                                                     .Where( part => part.Length == 2 )
                                                     .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        return new Vertex(
            System.Convert.ToInt32( vect_data[ "id" ] ),
            vect_data[ "label" ],
            Graph.ToNullableDouble( vect_data[ "x_pos" ] ),
            Graph.ToNullableDouble( vect_data[ "y_pos" ] ),
            System.Convert.ToInt32( vect_data[ "style" ] ),
            System.Convert.ToInt32( vect_data[ "color" ] ),
            System.Convert.ToInt32( vect_data[ "label_style" ] )
        );
    }

    // requires that all new vertices are already added
    private Edge ParseEdge( string line )
    {
        Dictionary< string, string > edge_data = line.Replace( " ", "" )
                                                     .Replace( "{", "" )
                                                     .Replace( "}", "" )
                                                     .Split( ',' )
                                                     .Select( part  => part.Split( ':' ) )
                                                     .Where( part => part.Length == 2 )
                                                     .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        return new Edge(
            System.Convert.ToInt32( edge_data[ "id" ] ),
            this.GetVertex( System.Convert.ToInt32( edge_data[ "vert1" ] ) ),
            this.GetVertex( System.Convert.ToInt32( edge_data[ "vert2" ] ) ),
            edge_data[ "label" ],
            System.Convert.ToDouble( edge_data[ "weight" ] ),
            System.Convert.ToBoolean( edge_data[ "directed" ] ),
            System.Convert.ToInt32( edge_data[ "style" ] ),
            System.Convert.ToInt32( edge_data[ "color" ] ),
            System.Convert.ToInt32( edge_data[ "thickness" ] ),
            System.Convert.ToInt32( edge_data[ "label_style" ] )
            // System.Convert.ToInt32( edge_data[ "tail_style" ] ),
            // System.Convert.ToInt32( edge_data[ "head_style" ] ) 
        );
    }

    public void Export( string path )
    {
        try
        {
            // Debug.Log( "begin export" );
            if ( File.Exists( path ) )
                File.Delete( path );

            using ( FileStream fs = File.Create( path ) )
            {
                // Debug.Log("export file");
                this.ExportVertices( fs );
                Graph.ExportText( fs, "\n" );
                this.ExportEdges( fs );
                fs.Close();
                // Debug.Log( "closed file" );
            }
        }
        catch ( Exception ex )
        {
            // TODO: inform user of issue
            // Debug.Log( "exception thrown" );
            Debug.Log( ex.ToString() );
        }
    }

    private void ExportVertices( FileStream fs )
    {
        Graph.ExportText( fs, "vertices:\n" );
        foreach ( Vertex vert in this.vertices )
            Graph.ExportText( fs, "{ " + vert.ToString() + " }\n" );
    }

    private void ExportEdges( FileStream fs )
    {
        Graph.ExportText( fs, "incidence:\n" );
        foreach ( Edge edge in this.adjacency.Values )
            Graph.ExportText( fs, "{ " + edge.ToString() + " }\n" );
    }

    private static void ExportText( FileStream fs, string value )
    {
        byte[] info = new UTF8Encoding( true ).GetBytes( value );
        fs.Write( info, 0, info.Length );
    }


    // algorithms ////////////////////////////////////////////////

    // public bool IsBipartite()
    // {

    // }


    // helper methods ////////////////////////////////////////////////

    private static double? ToNullableDouble( string s )
    {
        double d;
        if ( double.TryParse( s, out d ) )
            return d;
        return null;
    }
}
