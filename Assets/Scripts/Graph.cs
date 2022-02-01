
//All code developed by Team 11

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Vertex // TODO: place in its own file
{
    static private uint id_count;

    private uint id;
    public string label;
    public double? x_pos, y_pos;

    public uint style;
    public uint color;
    public uint label_style;

    // default value position is null
    public Vertex( string label="", double? x_pos=null, double? y_pos=null, uint style=0, uint color=0, uint label_style=0 )
    {
        this.id = Vertex.id_count++;
        this.label = label;
        this.x_pos = x_pos;
        this.y_pos = y_pos;
        this.style = style;
        this.color = color;
        this.label_style = label_style;
    }

    public uint GetId() => this.id; // temp

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( label, x_pos, y_pos, style, color, label_style ).GetHashCode();

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    // public static bool operator <=( Vertex lhs, Vertex rhs ) => lhs < rhs || lhs == rhs;

    public static bool operator >( Vertex lhs, Vertex rhs ) => lhs.id > rhs.id;

    // public static bool operator >=( Vertex lhs, Vertex rhs ) => !( lhs < rhs );

    public override string ToString() => String.Format( "id: {0}, label: {1}, x_pos: {2}, y_pos: {3}, style: {4}, color: {5}, label_style: {6}", this.id, this.label, this.x_pos, this.y_pos, this.style, this.color, this.label_style );
}


[System.Serializable]
public class Edge // TODO: place in its own file
{
    public Vertex vert1, vert2;
    public bool directed;
    public string label;
    public double weight;

    public uint style;
    public uint color;
    public uint thickness;
    public uint label_style;

    // when undirected, following should be ignored
    public uint? tail_style;
    public uint? head_style;

    public Edge( Vertex vert1, Vertex vert2, bool directed=false, string label="", double weight=1, uint style=0, uint color=0, uint thickness=0, uint label_style=0, uint? tail_style=null, uint? head_style=null )
    {
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.directed = directed;
        this.label = label;
        this.weight = weight;
        this.style = style;
        this.color = color;
        this.thickness = thickness;
        this.label_style = label_style;
        this.tail_style = tail_style;
        this.head_style = head_style;
    }

    public void Reverse()
    {
        Vertex temp = vert2;
        vert2 = vert1;
        vert1 = temp;
    }

    public void ResetWeight() => this.weight = 1;

    public bool IncidentOn( Vertex vert ) => vert == this.vert1 || vert == this.vert2;

    public override int GetHashCode() => ( vert1, vert2, directed, label, weight, style, color, thickness ).GetHashCode();

    public override string ToString()
    {
        if ( this.directed )
            return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, weight: {4}, style: {5}, color: {6}, thickness: {7}, label_style: {8}, tail_style: {9}, head_style: {10}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.weight, this.style, this.color, this.thickness, this.label_style, this.tail_style, this.head_style );
        return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, weight: {4}, style: {5}, color: {6}, thickness: {7}, label_style: {8}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.weight, this.style, this.color, this.thickness, this.label_style );
    }
}


[System.Serializable]
public class Graph
{
    // Vertices list is read-only from outside of class
    public List< Vertex > vertices { get; private set; }
    // Incidence dict is read-only from outside of class
    public Dictionary< ( Vertex, Vertex ), Edge > adjacency { get; private set; }

    private bool  directed = false;

    private int? chromatic_num;


    public Graph()
    {
        this.vertices = new List< Vertex >();
        this.adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
    }

    public Graph( Graph graph )
    {
        this.vertices = new List< Vertex >( graph.vertices );
        this.adjacency = new Dictionary< ( Vertex, Vertex ), Edge >( graph.adjacency );
    }

    public void Clear()
    {
        this.vertices = new List< Vertex >();
        this.adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
    }

    // temp
    private Vertex GetVertex( int id )
    {
        foreach ( Vertex vert in this.vertices )
        {
            if ( vert.GetId() == id )
                return vert;
        }
        Debug.Log( ( new System.Exception( "Vertex could not be found." ) ).ToString() ); // for testing purposes
        throw new System.Exception( "Vertex could not be found." );
    }

    public Edge this[ Vertex vert1, Vertex vert2 ]
    {
        get => this.adjacency[ ( vert1, vert2 ) ];
    }

    public Vertex AddVertex( double? x_pos=null, double? y_pos=null )
    {
        return this.AddVertex( new Vertex( x_pos : x_pos, y_pos : y_pos ) );
    }

    public Vertex AddVertex( Vertex vert )
    {
        this.vertices.Add( vert );
        return vert;
    }

    public Edge AddEdge( Vertex vert1, Vertex vert2, bool directed=false )
    {
        if ( directed || vert1 < vert2 )
            return this.AddEdge( new Edge( vert1, vert2 ) );
        else
            return this.AddEdge( new Edge( vert2, vert1 ) );
    }

    public Edge AddEdge( Edge edge )
    {
        if ( !this.vertices.Contains( edge.vert1 ) || !this.vertices.Contains( edge.vert2 ) )
        {
            Debug.Log( ( new System.Exception( "Edge is incident to one or more vertices that have not been added to the graph." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Edge is incident to one or more vertices that have not been added to the graph." );
        }
        if ( edge.vert1 > edge.vert2 && !edge.directed )
        {
            Debug.Log( ( new System.Exception( "Edge must be directed." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Edge must be directed." );
        }
        else
            this.adjacency[ ( edge.vert1, edge.vert2 ) ] = edge;
        return edge;
    }

    public void RemoveVertex( Vertex vect )
    {
        this.vertices.Remove( vect );

        // List< ( Vertex, Vertex ) > to_be_removed = new List< ( Vertex, Vertex ) >();
        // foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        // {
        //     if ( kvp.Value.IncidentOn( vert ) )
        //         to_be_removed.Add( kvp.Key );
        // }

        // foreach ( ( Vertex, Vertex ) remove in to_be_removed )
        //     this.adjacency.Remove( remove );

        // TODO: double check this works
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency.Where( kvp => kvp.Key.Item1 == vect || kvp.Key.Item2 == vect ).ToList() )
            this.adjacency.Remove( kvp.Key );
    }

    public void RemoveVertices( List< Vertex > vects )
    {
        foreach ( Vertex vect in vects )
            this.RemoveVertex( vect );
    }

    public void RemoveEdge( Edge edge )
    {
        if ( edge.directed || edge.vert1 < edge.vert2 )
            this.adjacency.Remove( ( edge.vert1, edge.vert2 ) );
        else
            this.adjacency.Remove( ( edge.vert2, edge.vert1 ) );
    }

    public void RemoveEdges( List< Edge > edges )
    {
        foreach ( Edge edge in edges )
            this.RemoveEdge( edge );
    }

    public Edge ReverseEdge( Edge edge )
    {
        if ( !edge.directed )
        {
            Debug.Log( ( new System.Exception( "Cannot reverse undirected edge." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Cannot reverse undirected edge." );
        }
        if ( !( edge in this[ edge.vert1, edge.vert2 ] ) )
        {
            Debug.Log( ( new System.Exception( "The provided edge to reverse is not in the graph." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "The provided edge to reverse is not in the graph." );
        }

        this.RemoveEdge( edge );
        edge.Reverse();
        this.AddEdge( edge );
        return edge;
    }

    public bool IsAdjacent( Vertex vert1, Vertex vert2 ) => this.adjacency.ContainsKey( ( vert1, vert2 ) ) || this.adjacency.ContainsKey( ( vert2, vert1 ) );


    // file io methods ////////////////////////////////////////////////

    // TODO: relax import file formatting
    // currently not importing directed info
    public void Import( string path )
    {
        // this.Clear();
        try
        {
            if ( !File.Exists( path ) )
            {
                Debug.Log( ( new System.Exception( "The provided file cannot be found." ) ).ToString() ); // for testing purposes
                throw new System.Exception( "The provided file cannot be found." );
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
                if ( line.Trim() == "edges:" )
                {
                    flag = false;
                    continue;
                }
                this.ParseLine( line, flag );
            }
        }
        catch ( Exception ex )
        {
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
                                                     .Split( ',' )
                                                     .Select( part  => part.Split( ':' ) )
                                                     .Where( part => part.Length == 2 )
                                                     .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        return new Vertex(
            vect_data[ "label" ],
            Graph.ToNullableDouble( vect_data[ "x_pos" ] ),
            Graph.ToNullableDouble( vect_data[ "y_pos" ] ),
            System.Convert.ToUInt32( vect_data[ "style" ] ),
            System.Convert.ToUInt32( vect_data[ "color" ] ),
            System.Convert.ToUInt32( vect_data[ "label_style" ] )
        );
    }

    // requires that all new vertices are already added
    private Edge ParseEdge( string line )
    {
        Dictionary< string, string > edge_data = line.Replace( " ", "" )
                                                     .Split( ',' )
                                                     .Select( part  => part.Split( ':' ) )
                                                     .Where( part => part.Length == 2 )
                                                     .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        return new Edge(
            this.GetVertex( System.Convert.ToInt32( edge_data[ "vert1" ] ) ),
            this.GetVertex( System.Convert.ToInt32( edge_data[ "vert2" ] ) ),
            edge_data[ "label" ],
            System.Convert.ToDouble( edge_data[ "weight" ] ),
            System.Convert.ToBoolean( edge_data[ "directed" ] ),
            System.Convert.ToUInt32( edge_data[ "style" ] ),
            System.Convert.ToUInt32( edge_data[ "color" ] ),
            System.Convert.ToUInt32( edge_data[ "thickness" ] ),
            System.Convert.ToUInt32( edge_data[ "label_style" ] )
            // System.Convert.ToInt32( edge_data[ "tail_style" ] ),
            // System.Convert.ToInt32( edge_data[ "head_style" ] ) 
        );
    }

    // currently not exporting directed info
    public void Export( string path )
    {
        try
        {
            if ( File.Exists( path ) )
                File.Delete( path );

            using ( FileStream fs = File.Create( path ) )
            {
                this.ExportVertices( fs );
                Graph.ExportText( fs, "\n" );
                this.ExportEdges( fs );
                fs.Close();
            }
        }
        catch ( Exception ex )
        {
            // TODO: inform user of issue
            Debug.Log( ex.ToString() );
        }
    }

    private void ExportVertices( FileStream fs )
    {
        Graph.ExportText( fs, "vertices:\n" );
        foreach ( Vertex vert in this.vertices )
            Graph.ExportText( fs, vert.ToString() + '\n' );
    }

    private void ExportEdges( FileStream fs )
    {
        Graph.ExportText( fs, "edges:\n" );
        foreach ( Edge edge in this.adjacency.Values )
            Graph.ExportText( fs, edge.ToString() + '\n' );
    }

    private static void ExportText( FileStream fs, string value )
    {
        byte[] info = new UTF8Encoding( true ).GetBytes( value );
        fs.Write( info, 0, info.Length );
    }


    // algorithms ////////////////////////////////////////////////

    public bool IsBipartite()
    {
        if ( this.chromatic_num is null )
            this.chromatic_num = this.GetChromaticNumber();
        return this.chromatic_num == 2;
    }

    // brute force method, exponential time complexity with respect to vertices
    public int GetChromaticNumber()
    {
        // if ( !( this.chromatic_num is null ) )
        //     return ( int ) this.chromatic_num;
        int chi = this.vertices.Count;
        HashSet< List< int > > colorings = this.GetAllColorings();
        foreach ( List< int > coloring in colorings )
        {
            int num_colors = ( new HashSet< int >( coloring ) ).Count;
            if ( num_colors < chi && this.IsProperColoring( coloring ) )
                chi = num_colors;
        }
        this.chromatic_num = chi;
        return chi;
    }

    private bool IsProperColoring( List< int > coloring )
    {
        foreach ( Edge edge in this.adjacency.Values )
        {
            if ( coloring[ this.vertices.IndexOf( edge.vert1 ) ] == coloring[ this.vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private HashSet< List< int > > GetAllColorings()
    {
        HashSet< List< int > > colorings = new HashSet< List< int > >();
        GetAllColoringsHelper( colorings, new List< int >(), this.vertices.Count, this.vertices.Count );
        return colorings;
    }

    private static void GetAllColoringsHelper( HashSet< List< int > > colorings, List< int > coloring, int num_vertices, int num_colors )
    {
        if ( coloring.Count >= num_vertices )
            colorings.Add( coloring );
        else
        {
            for ( int i = 0; i < num_colors; i++ )
            {
                List< int > new_coloring = new List< int >( coloring );
                new_coloring.Add( i );
                GetAllColoringsHelper( colorings, new_coloring, num_vertices, num_colors );
            }
        }
    }

    public List< Edge > Prim( Vertex vert )
    {
        List< Edge > mst = new List< Edge >();
        HashSet< Vertex > mst_vertices = new HashSet< Vertex >() { vert };
        int mst_vertices_prev_count = -1;
        while ( mst_vertices_prev_count != mst_vertices.Count )
        {
            mst_vertices_prev_count = mst_vertices.Count;
            List< Edge > incident_edges = new List< Edge >( this.GetIncidentEdges( mst_vertices ).OrderBy( edge => edge.weight ) );
            foreach ( Edge edge in incident_edges )
            {
                if ( !mst_vertices.Contains( edge.vert1 ) || !mst_vertices.Contains( edge.vert2 ) )
                {
                    mst_vertices.Add( edge.vert1 );
                    mst_vertices.Add( edge.vert2 );
                    mst.Add( edge );
                }
            }
        }
        return mst;
    }

    public List< Edge > GetIncidentEdges( Vertex vert )
    {
        return this.GetIncidentEdges( new HashSet< Vertex >() { vert } );
    }

    public List< Edge > GetIncidentEdges( HashSet< Vertex > verts )
    {
        List< Edge > incident_edges = new List< Edge >();
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        {
            foreach ( Vertex vert in verts )
            {
                if ( kvp.Value.IncidentOn( vert ) )
                    incident_edges.Add( kvp.Value );
            }
        }
        return incident_edges;
    }

    public List< Edge > Kruskal()
    {
        List< Edge > mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( this.adjacency.Values.OrderBy( edge => edge.weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        foreach ( Vertex vert in this.vertices )
            forest.Add( new HashSet< Vertex >() { vert } );
        foreach ( Edge edge in edges )
        {
            HashSet< Vertex > tree1 = Graph.GetComponentOf( forest, edge.vert1 );
            HashSet< Vertex > tree2 = Graph.GetComponentOf( forest, edge.vert2 );
            if ( tree1 != tree2 )
            {
                forest.Remove( tree1 );
                tree2.UnionWith( tree1 );
                mst.Add( edge );
            }
        }
        return mst;
    }

    private static HashSet< Vertex > GetComponentOf( HashSet< HashSet< Vertex > > components, Vertex vert )
    {
        foreach ( HashSet< Vertex > component in components )
        {
            if ( component.Contains( vert ) )
                return component;
        }
        Debug.Log( ( new System.Exception( "Vertex could not be found in collection of components." ) ).ToString() );
        throw new System.Exception( "Vertex could not be found in collection of components." );
    }

    // public int GetNumComponents()
    // {

    // }

    // public void CountComponents( Edge edge, Vertex vert )
    // {

    // }

    private void DepthFirstSearch( Vertex vert, Action< Edge, Vertex > f )
    {
        List< bool > visited = new List< bool >( this.vertices.Count );
        this.DepthFirstSearchHelper( vert, visited, f );
    }

    private void DepthFirstSearchHelper( Vertex vert, List< bool > visited, Action< Edge, Vertex > f )
    {
        foreach ( Edge edge in this.GetIncidentEdges( vert ) )
        {
            int vert1_i = this.vertices.IndexOf( edge.vert1 );
            int vert2_i = this.vertices.IndexOf( edge.vert2 );
            visited[ vert1_i ] = true;
            visited[ vert2_i ] = true;
            if ( visited[ vert1_i ] )
            {
                f( edge, edge.vert1 );
                this.DepthFirstSearchHelper( edge.vert1, visited, f );
            }
            if ( visited[ vert2_i ] )
            {
                f( edge, edge.vert2 );
                this.DepthFirstSearchHelper( edge.vert2, visited, f );
            }
        }
    }

    // TODO: keep matrix of distances, update each time an edge is added?
    public List< Vertex > Dijkstra( Vertex src, Vertex dest )
    {
        HashSet< Vertex > not_visited = new HashSet< Vertex >( this.vertices );
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in this.vertices )
            dist[ v ] = double.PositiveInfinity;

        dist[ src ] = 0;
        not_visited.Remove( src );

        while ( not_visited.Count() > 0 )
        {
            // find u in not_visited such that dist[u] is minimal
            Vertex u = not_visited.First();
            foreach ( Vertex v in not_visited )
            {
                if ( dist[ v ] < dist[ u ] )
                    u = v;
            }

            not_visited.Remove( u );

            // update neighbors of u
            foreach ( Vertex v in not_visited )
            {
                if ( this.adjacency.ContainsKey( ( u, v ) ) )
                {
                    double tmp = dist[ u ] + this.adjacency[ ( u, v ) ].weight;
                    if ( tmp < dist[ v ] )
                    {
                        dist[ v ] = tmp;
                        prev[ v ] = u;
                    }
                }
            }
        }

        // put together final path 
        List< Vertex > result = new List< Vertex >();
        Vertex curr = dest;
        while ( curr != src )
        {
            result.Add( curr );
            curr = prev[ curr ];
            if ( curr == null )
                return new List<Vertex>();
        }
        result.Add( src );
        result.Reverse();

        return result;
    }


    // helper methods ////////////////////////////////////////////////

    private static double? ToNullableDouble( string s )
    {
        double d;
        if ( double.TryParse( s, out d ) )
            return d;
        return null;
    }
}
