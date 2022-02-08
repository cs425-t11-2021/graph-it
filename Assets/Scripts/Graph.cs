
// All code developed by Team 11

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Graph
{
    // Vertices list is read-only from outside of class
    public List< Vertex > vertices { get; private set; }
    // Adjacency dict is read-only from outside of class
    public Dictionary< ( Vertex, Vertex ), Edge > adjacency { get; private set; } // currently supporting only single edges between vertices

    // parameters
    private bool _directed;
    public bool directed // true if any edge is directed
    {
        get => this.IsDirected();
        private set => this._directed = value;
    }

    private bool _weighted;
    public bool weighted // true if any edge is weighted
    {
        get => this.IsWeighted();
        private set => this._weighted = value;
    }

    private bool _fully_weighted;
    public bool fully_weighted // true when all edges are weighted
    {
        get => this.IsFullyWeighted();
        private set => this._fully_weighted = value;
    }

    private bool _simple;
    public bool simple // false if multiple edges. false if loops on vertices (unless directed)
    {
        get => this.IsSimple();
        private set => this._simple = value;
    }

    private int? chromatic_num;


    public Graph() // pass default settings parameters
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
        get => vert1 > vert2 ? this.adjacency[ ( vert2, vert1 ) ] : this.adjacency[ ( vert1, vert2 ) ];
    }

    private List< Edge > GetDirectedEdges()
    {
        List< Edge > edges = this.adjacency.Values.ToList();
        foreach ( Edge edge in edges )
        {
            if ( !edge.directed )
            {
                edge.Reverse();
                edges.Add( edge );
                edge.Reverse();
            }
        }

        return edges;
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
            return this.AddEdge( new Edge( vert1, vert2, directed ) );
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
        this.directed = this.directed || edge.directed;
        return edge;
    }

    public void RemoveVertex( Vertex vect )
    {
        this.vertices.Remove( vect );
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
      
        if ( edge != this[ edge.vert1, edge.vert2 ] )
        {
            Debug.Log( ( new System.Exception( "The provided edge to reverse is not in the graph." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "The provided edge to reverse is not in the graph." );
        }

        this.RemoveEdge( edge );
        edge.Reverse();
        this.AddEdge( edge );
        return edge;
    }

    public bool IsAdjacent( Vertex vert1, Vertex vert2 ) => !( this[ vert1, vert2 ] is null );

    private bool IsDirected()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        {
            if ( kvp.Value.directed )
                return true;
        }
        return false;
    }

    private bool IsWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        {
            if ( kvp.Value.weighted )
                return true;
        }
        return false;
    }

    private bool IsFullyWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        {
            if ( !kvp.Value.weighted )
                return false;
        }
        return true;
    }

    private bool IsSimple()
    {
        if ( !this.directed )
        {
            foreach ( Vertex vert in this.vertices )
            {
                // if ( this[ vert, vert ].Count > 0 )
                   // return false;
            }
        }
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.adjacency )
        {
            // if ( kvp.Value.Count > 0 )
                // return false;
        }
        return true;
    }


    // file io methods ////////////////////////////////////////////////

    // TODO: relax import file formatting
    // currently not importing directed info
    public void Import( string path )
    {
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
            System.Convert.ToBoolean( edge_data[ "directed" ] ),
            edge_data[ "label" ],
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
        if ( this.directed )
        {
            Debug.Log( ( new System.Exception( "Prim's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );
        }

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
            if ( verts.Contains( kvp.Value.vert1 ) || kvp.Value.directed && verts.Contains( kvp.Value.vert2 ) )
                incident_edges.Add( kvp.Value );
        }
        return incident_edges;
    }

    public List< Edge > Kruskal()
    {
        if ( this.directed )
        {
            Debug.Log( ( new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." );
        }

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

    public List< Vertex > Dijkstra( Vertex src, Vertex dest )
    {
        HashSet< Vertex > not_visited = new HashSet< Vertex >( this.vertices );
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in this.vertices )
            dist[ v ] = double.PositiveInfinity;

        dist[ src ] = 0;

        foreach ( Vertex v in this.vertices )
            prev[ v ] = null;

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
            if ( curr is null )
                return new List<Vertex>();
        }
        result.Add( src );
        result.Reverse();

        return result;
    }

    // TODO: return weights
    // no partial weights
    // returns minimum spanning tree when provided a source
    public List< Edge > BellmanFord( Vertex src )
    {
        if ( this.weighted && !this.fully_weighted )
        {
            Debug.Log( ( new System.Exception( "Graph is not fully weighted." ) ).ToString() ); // for testing purposes
            throw new System.Exception( "Graph is not fully weighted." );
        }

        // initialize data
        List< Edge > edges = this.adjacency.Values.ToList();
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Edge > prev = new Dictionary< Vertex, Edge >();
        foreach ( Vertex vert in this.vertices )
            dist[ vert ] = Double.PositiveInfinity;
        dist[ src ] = 0;

        // relax edges
        for ( int i = 0; i < this.vertices.Count - 1; i++ )
        {
            foreach ( Edge edge in edges )
            {
                if ( dist[ edge.vert1 ] + edge.weight < dist[ edge.vert2 ] )
                {
                    dist[ edge.vert2 ] = dist[ edge.vert1 ] + edge.weight;
                    prev[ edge.vert2 ] = edge;
                }
            }
        }

        // check for negative cycles
        foreach ( Edge edge in edges )
        {
            if ( dist[ edge.vert1 ] + edge.weight < dist[ edge.vert2 ] )
            {
                Debug.Log( ( new System.Exception( "Negative weight cycle found." ) ).ToString() ); // for testing purposes
                throw new System.Exception( "Negative weight cycle found." );
            }
        }

        return prev.Values.ToList();
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
