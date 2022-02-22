
// All code developed by Team 11

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

[System.Serializable]
public class Graph
{
    public List< Vertex > Vertices { get; private set; } // TODO: make concurrent
    public ConcurrentDictionary< ( Vertex, Vertex ), Edge > Adjacency { get; private set; } // not yet supporting multiple edges
    public Stack< GraphModification > Changes { get; private set; } // logs all changes to graph
    private Stack< GraphModification > redoChanges; // logs all undone changes

    // parameters
    public bool Directed // true if any edge is directed
    {
        get => this.IsDirected();
    }

    public bool Weighted // true if any edge is weighted
    {
        get => this.IsWeighted();
    }

    public bool FullyWeighted // true when all edges are weighted
    {
        get => this.IsFullyWeighted();
    }

    public bool Simple // false if multiple edges. false if loops on vertices (unless directed)
    {
        get => this.IsSimple();
    }

    // TODO: need HasLoops and HasMultipleEdges

    public static void PrintStack( Stack< GraphModification > s ) // temp, for testing undo/redo
    {
        // If stack is empty then return
        if (s.Count == 0)
            return;
         
        GraphModification x = s.Peek();
     
        // Pop the top element of the stack
        s.Pop();
     
        // Recursively call the function PrintStack
        PrintStack(s);
     
        // Print the stack element starting
        // from the bottom
        Debug.Log(x.Mod);
     
        // Push the same element onto the stack
        // to preserve the order
        s.Push(x);
    }


    public Graph()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new ConcurrentDictionary< ( Vertex, Vertex ), Edge >();
        this.Changes = new Stack< GraphModification >();
    }

    public Graph( Graph graph )
    {
        this.Vertices = new List< Vertex >( graph.Vertices );
        this.Adjacency = new ConcurrentDictionary< ( Vertex, Vertex ), Edge >( graph.Adjacency );
    }

    public void Clear()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new ConcurrentDictionary< ( Vertex, Vertex ), Edge >();
    }

    // temp
    private Vertex GetVertex( uint id )
    {
        foreach ( Vertex vert in this.Vertices )
        {
            if ( vert.GetId() == id )
                return vert;
        }
        throw new System.Exception( "Vertex could not be found." );
    }

    public Edge this[ Vertex vert1, Vertex vert2 ]
    {
        get => vert1 > vert2 ? this.Adjacency.GetValue( ( vert2, vert1 ) ) : this.Adjacency.GetValue( ( vert1, vert2 ) );
    }

    private List< Edge > GetDirectedEdges()
    {
        List< Edge > edges = this.Adjacency.Values.ToList();
        foreach ( Edge edge in edges )
        {
            if ( !edge.directed )
            {
                // TODO: fix this, this will mess with this.Changes
                edge.Reverse();
                edges.Add( edge );
                edge.Reverse();
            }
        }

        return edges;
    }

    // TODO: add more parameters
    public Vertex AddVertex( float x, float y, bool recordChange=true )
    {
        return this.AddVertex( new Vertex( this.CreateModification, x : x, y : y ), recordChange );
    }

    public Vertex AddVertex( Vertex vert, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_VERTEX, vert );
        this.Vertices.Add( vert );
        return vert;
    }

    public Edge AddEdge( Vertex vert1, Vertex vert2, bool directed=false, bool recordChange=true )
    {
        if ( directed || vert1 < vert2 )
            return this.AddEdge( new Edge( vert1, vert2, directed ) );
        else
            return this.AddEdge( new Edge( vert2, vert1 ) );
    }

    public Edge AddEdge( Edge edge, bool recordChange=true )
    {
        if ( !this.Vertices.Contains( edge.vert1 ) || !this.Vertices.Contains( edge.vert2 ) )
            throw new System.Exception( "Edge is incident to one or more vertices that have not been added to the graph." );
        if ( edge.vert1 > edge.vert2 && !edge.directed )
            throw new System.Exception( "Edge must be directed." );
        else
            this.Adjacency[ ( edge.vert1, edge.vert2 ) ] = edge;
        return edge;
    }

    public void RemoveVertex( Vertex vert, bool recordChange=true )
    {
        this.RemoveEdges( this.Adjacency.Values.Where( edge => edge.vert1 == vert || edge.vert2 == vert ).ToList(), recordChange );
        this.Vertices.Remove( vert );
        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_VERTEX, vert );
    }

    public void RemoveVertices( List< Vertex > verts, bool recordChange=true )
    {
        foreach ( Vertex vert in verts )
            this.RemoveVertex( vert, recordChange );
    }

    public void RemoveEdge( Edge edge, bool recordChange=true )
    {
        if ( edge.directed || edge.vert1 < edge.vert2 )
            this.Adjacency.TryRemove( ( edge.vert1, edge.vert2 ), out _ );
        else
            this.Adjacency.TryRemove( ( edge.vert2, edge.vert1 ), out _ );
    }

    public void RemoveEdges( List< Edge > edges, bool recordChange=true )
    {
        foreach ( Edge edge in edges )
            this.RemoveEdge( edge );
    }

    public Edge ReverseEdge( Edge edge )
    {
        if ( !edge.directed )
            throw new System.Exception( "Cannot reverse undirected edge." );
      
        if ( edge != this[ edge.vert1, edge.vert2 ] )
            throw new System.Exception( "The provided edge to reverse is not in the graph." );

        this.RemoveEdge( edge, false );
        edge.Reverse();
        this.AddEdge( edge, false );
        return edge;
    }

    public void Undo()
    {

    }

    public void Redo()
    {

    }

    public bool IsAdjacent( Vertex vert1, Vertex vert2 ) => this.Adjacency.ContainsKey( ( vert1, vert2 ) ) || this.Adjacency.ContainsKey( ( vert2, vert1 ) );

    private bool IsDirected()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( kvp.Value.directed )
                return true;
        }
        return false;
    }

    private bool IsWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( kvp.Value.weighted )
                return true;
        }
        return false;
    }

    private bool IsFullyWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( !kvp.Value.weighted )
                return false;
        }
        return true;
    }

    private bool IsSimple()
    {
        if ( !this.Directed )
        {
            foreach ( Vertex vert in this.Vertices )
            {
                // if ( this[ vert, vert ].Count > 0 )
                   // return false;
            }
        }
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            // if ( kvp.Value.Count > 0 )
                // return false;
        }
        return true;
    }

    public void CreateModification( Modification mod, System.Object modified )
    {
        new GraphModification( this, mod, modified );
    }


    // file io methods ////////////////////////////////////////////////

    // TODO: relax import file formatting
    // currently not importing directed info
    public void Import( string path )
    {
        this.Clear();
        try
        {
            if ( !File.Exists( path ) )
                throw new System.Exception( "The provided file cannot be found." );

            Dictionary< uint, uint > vertexIndices = new Dictionary< uint, uint >();
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
                this.ParseLine( line, flag, vertexIndices );
            }
        }
        catch ( Exception ex )
        {
            throw ex;
        }
    }

    private void ParseLine( string line, bool flag, Dictionary< uint, uint > indices )
    {
        if ( flag )
            this.AddVertex( this.ParseVertex( line, indices ) );
        else
            this.AddEdge( this.ParseEdge( line, indices ) );
    }

    private Vertex ParseVertex( string line, Dictionary< uint, uint > indices )
    {
        Dictionary< string, string > vectData = line.Replace( " ", "" )
                                                    .Split( ',' )
                                                    .Select( part  => part.Split( ':' ) )
                                                    .Where( part => part.Length == 2 )
                                                    .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        Vertex vert = new Vertex(
            vectData[ "label" ],
            float.Parse( vectData[ "x" ], CultureInfo.InvariantCulture.NumberFormat ),
            float.Parse( vectData[ "y" ], CultureInfo.InvariantCulture.NumberFormat ),
            System.Convert.ToUInt32( vectData[ "style" ] ),
            System.Convert.ToUInt32( vectData[ "color" ] )
        );
        indices.Add( System.Convert.ToUInt32( vectData[ "id" ] ), vert.GetId() );
        return vert;
    }

    // requires that all new vertices are already added
    private Edge ParseEdge( string line, Dictionary< uint, uint > indices )
    {
        Dictionary< string, string > edgeData = line.Replace( " ", "" )
                                                    .Split( ',' )
                                                    .Select( part  => part.Split( ':' ) )
                                                    .Where( part => part.Length == 2 )
                                                    .ToDictionary( sp => sp[ 0 ], sp => sp[ 1 ] );

        return new Edge(
            this.GetVertex( indices[ System.Convert.ToUInt32( edgeData[ "vert1" ] ) ] ),
            this.GetVertex( indices[ System.Convert.ToUInt32( edgeData[ "vert2" ] ) ] ),
            System.Convert.ToBoolean( edgeData[ "directed" ] ),
            edgeData[ "label" ],
            System.Convert.ToUInt32( edgeData[ "style" ] ),
            System.Convert.ToUInt32( edgeData[ "color" ] ),
            System.Convert.ToUInt32( edgeData[ "thickness" ] )//,
            // System.Convert.ToInt32( edgeData[ "tail style" ] ),
            // System.Convert.ToInt32( edgeData[ "head style" ] ) 
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
            throw ex;
        }
    }

    private void ExportVertices( FileStream fs )
    {
        Graph.ExportText( fs, "vertices:\n" );
        foreach ( Vertex vert in this.Vertices )
            Graph.ExportText( fs, vert.ToString() + '\n' );
    }

    private void ExportEdges( FileStream fs )
    {
        Graph.ExportText( fs, "edges:\n" );
        foreach ( Edge edge in this.Adjacency.Values )
            Graph.ExportText( fs, edge.ToString() + '\n' );
    }

    private static void ExportText( FileStream fs, string value )
    {
        byte[] info = new UTF8Encoding( true ).GetBytes( value );
        fs.Write( info, 0, info.Length );
    }


    // algorithms ////////////////////////////////////////////////

    public List< Edge > Prim( Vertex vert )
    {
        if ( this.Directed )
            throw new System.Exception( "Prim's algorithm is unsupported on directed graphs." );

        List< Edge > mst = new List< Edge >();
        HashSet< Vertex > mstVertices = new HashSet< Vertex >() { vert };
        int mstVerticesPrevCount = -1;
        while ( mstVerticesPrevCount != mstVertices.Count )
        {
            mstVerticesPrevCount = mstVertices.Count;
            List< Edge > incidentEdges = new List< Edge >( this.GetIncidentEdges( mstVertices ).OrderBy( edge => edge.weight ) );
            foreach ( Edge edge in incidentEdges )
            {
                if ( !mstVertices.Contains( edge.vert1 ) || !mstVertices.Contains( edge.vert2 ) )
                {
                    mstVertices.Add( edge.vert1 );
                    mstVertices.Add( edge.vert2 );
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
        List< Edge > incidentEdges = new List< Edge >();
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( verts.Contains( kvp.Value.vert1 ) || !kvp.Value.directed && verts.Contains( kvp.Value.vert2 ) )
                incidentEdges.Add( kvp.Value );
        }
        return incidentEdges;
    }

    public int GetVertexDegree( Vertex u )
    {
        int count = 0;
        foreach ( Vertex v in this.Vertices )
        {
            if ( this.IsAdjacent( u, v ) )
                count++;
        }

        return count;
    }

    public List< Edge > Kruskal()
    {
        if ( this.Directed )
            throw new System.Exception( "Kruskal's algorithm is unsupported on directed graphs." );

        List< Edge > mst = new List< Edge >();
        List< Edge > edges = new List< Edge >( this.Adjacency.Values.OrderBy( edge => edge.weight ) );
        HashSet< HashSet< Vertex > > forest = new HashSet< HashSet< Vertex > >();
        foreach ( Vertex vert in this.Vertices )
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
        throw new System.Exception( "Vertex could not be found in collection of components." );
    }

    // public int GetNumComponents()
    // {

    // }

    // public void CountComponents( Edge edge, Vertex vert )
    // {

    // }

    public List< Vertex > Dijkstra( Vertex src, Vertex dest )
    {
        HashSet< Vertex > notVisited = new HashSet< Vertex >( this.Vertices );
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in this.Vertices )
            dist[ v ] = double.PositiveInfinity;

        dist[ src ] = 0;

        while ( notVisited.Count() > 0 )
        {
            // find u in notVisited such that dist[u] is minimal
            Vertex u = notVisited.First();
            foreach ( Vertex v in notVisited )
            {
                if ( dist[ v ] < dist[ u ] )
                    u = v;
            }

            notVisited.Remove( u );

            // update neighbors of u
            foreach ( Vertex v in notVisited )
            {
                if ( this.IsAdjacent( u, v ) )
                {
                    double tmp = dist[ u ] + this[ u, v ].weight;
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
        if ( this.Weighted && !this.FullyWeighted )
            throw new System.Exception( "Graph is not fully weighted." );

        // initialize data
        List< Edge > edges = this.Adjacency.Values.ToList();
        Dictionary< Vertex, double > dist = new Dictionary< Vertex, double >();
        Dictionary< Vertex, Edge > prev = new Dictionary< Vertex, Edge >();
        foreach ( Vertex vert in this.Vertices )
            dist[ vert ] = Double.PositiveInfinity;
        dist[ src ] = 0;

        // relax edges
        for ( int i = 0; i < this.Vertices.Count - 1; i++ )
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
                throw new System.Exception( "Negative weight cycle found." );
        }

        return prev.Values.ToList();
    }
}
