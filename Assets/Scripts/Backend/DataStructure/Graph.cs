
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
    public Dictionary< ( Vertex, Vertex ), Edge > Adjacency { get; private set; } // not yet supporting multiple edges
    public Stack< GraphModification > Changes { get; private set; } // logs all changes to graph
    private Stack< GraphModification > redoChanges; // logs all undone changes

    // parameters
    public int Order // number of vertices
    {
        get => this.Vertices.Count;
    }
    public int Size // number of edges
    {
        get => ( from kvp in this.Adjacency select kvp.Value ).Distinct().Count();
    }

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

    public bool HasLoops // true if a loop (an edge incident to only one vertex) exists
    {
        get => this.IsHasLoops();
    }

    // TODO: need HasLoops and HasMultipleEdges

    public static void PrintStack( Stack< GraphModification > s ) // temp, for testing undo/redo
    {
        if (s.Count == 0)
            return;
        GraphModification x = s.Peek();
        s.Pop();
        PrintStack(s);
        Debug.Log(x.Mod);
        s.Push(x);
    }


    public Graph()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
        this.Changes = new Stack< GraphModification >();
        // TODO: add redoChanges
    }

    public Graph( Graph graph )
    {
        this.Vertices = new List< Vertex >( graph.Vertices );
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >( graph.Adjacency );
        // TODO: add Changes and redoChanges
    }

    public void Clear()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
        // TODO: add Changes and redoChanges
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
        get => this.Adjacency.GetValue( ( vert1, vert2 ) );
    }

    // TODO: add more parameters
    public Vertex AddVertex( float x, float y, bool recordChange=true )
    {
        return this.AddVertex( new Vertex( x : x, y : y ), recordChange );
    }

    public Vertex AddVertex( Vertex vert, bool recordChange=true )
    {
        vert.CreateMod = ( Action< Modification, System.Object > ) this.CreateModificationAction;
        this.Vertices.Add( vert );

        if ( recordChange )
            new GraphModification( this, Modification.ADD_VERTEX, vert );
        return vert;
    }

    public Edge AddEdge( Vertex vert1, Vertex vert2, bool directed=false, bool recordChange=true )
    {
        return this.AddEdge( new Edge( vert1, vert2, directed ), recordChange );
    }

    public Edge AddEdge( Edge edge, bool recordChange=true )
    {
        if ( !this.Vertices.Contains( edge.vert1 ) || !this.Vertices.Contains( edge.vert2 ) )
            throw new System.Exception( "Edge is incident to one or more vertices that have not been added to the graph." );

        edge.MakeDirectedInGraph = ( Action< Edge > ) this.MakeEdgeDirectedAction;
        edge.MakeUndirectedInGraph = ( Action< Edge > ) this.MakeEdgeUndirectedAction;
        edge.ReverseInGraph = ( Action< Edge > ) this.ReverseEdgeAction;
        edge.CreateMod = ( Action< Modification, System.Object > ) this.CreateModificationAction;

        this.Adjacency[ ( edge.vert1, edge.vert2 ) ] = edge;
        if ( !edge.Directed )
            this.Adjacency[ ( edge.vert2, edge.vert1 ) ] = edge;

        if ( recordChange )
            new GraphModification( this, Modification.ADD_EDGE, edge );
        return edge;
    }

    public void Remove( List< Vertex > vertices, List< Edge > edges, bool recordChange=true )
    {
        this.Remove( vertices, false );
        this.Remove( edges, false );

        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vertexSet );
            edgeSet.UnionWith( edges );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, edgeSet ) );
        }
    }

    public void Remove( List< Vertex > vertices, Edge edge, bool recordChange=true )
    {
        this.Remove( vertices, false );
        this.Remove( edge, false );

        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vertexSet );
            edgeSet.Add( edge );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, edgeSet ) );
        }
    }

    public void Remove( Vertex vert, List< Edge > edges, bool recordChange=true )
    {
        this.Remove( vert, false );
        this.Remove( edges, false );

        if ( recordChange )
        {
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vert );
            edgeSet.UnionWith( edges );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, edgeSet ) );
        }
    }

    public void Remove( List< Vertex > vertices, bool recordChange=true )
    {
        foreach ( Vertex vert in vertices )
            this.Remove( vert, false );

        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, this.GetIncidentEdges( vertexSet ) ) );
        }
    }

    public void Remove( List< Edge > edges, bool recordChange=true )
    {
        foreach ( Edge edge in edges )
            this.Remove( edge, false );

        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( ( HashSet< Edge > ) null, new HashSet< Edge >( edges ) ) );
    }

    public void Remove( Vertex vert, Edge edge, bool recordChange=true )
    {
        this.Remove( vert, false );
        this.Remove( edge, false );

        if ( recordChange )
        {
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vert );
            edgeSet.Add( edge );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, edgeSet ) );
        }
    }

    public void Remove( Vertex vert, bool recordChange=true )
    {
        this.Remove( this.Adjacency.Values.Where( edge => edge.vert1 == vert || edge.vert2 == vert ).ToList(), recordChange );
        this.Vertices.Remove( vert );

        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, this.GetIncidentEdges( vert ) ) );
    }

    public void Remove( Edge edge, bool recordChange=true )
    {
        if ( edge.Directed )
            this.Adjacency.Remove( ( edge.vert1, edge.vert2 ) );
        else
        {
            this.Adjacency.Remove( ( edge.vert1, edge.vert2 ) );
            this.Adjacency.Remove( ( edge.vert2, edge.vert1 ) );
        }

        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( ( HashSet< Vertex > ) null, new HashSet< Edge >() { edge } ) );
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
            if ( kvp.Value.Directed )
                return true;
        }
        return false;
    }

    private bool IsWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( kvp.Value.Weighted )
                return true;
        }
        return false;
    }

    private bool IsFullyWeighted()
    {
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( !kvp.Value.Weighted )
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
                if ( !( this[ vert, vert ] is null ) )
                   return false;
            }
        }
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            // if ( kvp.Value.Count > 0 )
                // return false;
        }
        return true;
    }

    private bool IsHasLoops()
    {
        foreach ( Vertex vert in this.Vertices )
        {
            if ( !( this[ vert, vert ] is null ) )
                return true;
        }
        return false;
    }


    // actions from other classes /////////////////////////////////////

    public void ReverseEdgeAction( Edge edge )
    {
        if ( edge != this[ edge.vert1, edge.vert2 ] )
            throw new System.Exception( "The provided edge to reverse is not in the graph." );

        if ( edge.Directed )
        {
            this.Remove( edge, false );
            this.Adjacency[ ( edge.vert2, edge.vert1 ) ] = edge;
        }
    }

    public void MakeEdgeDirectedAction( Edge edge )
    {
        if ( edge != this[ edge.vert1, edge.vert2 ] )
            throw new System.Exception( "The provided edge to direct is not in the graph." );

        this.Adjacency.Remove( ( edge.vert2, edge.vert1 ) ); // TODO: record this change
    }

    public void MakeEdgeUndirectedAction( Edge edge )
    {
        if ( edge != this[ edge.vert1, edge.vert2 ] )
            throw new System.Exception( "The provided edge to undirect is not in the graph." );

        this.Adjacency[ ( edge.vert2, edge.vert1 ) ] = edge; // TODO: record this change
    }

    public void CreateModificationAction( Modification mod, System.Object modified )
    {
        new GraphModification( this, mod, modified );
    }


    // file io methods ////////////////////////////////////////////////

    // TODO: relax import file formatting
    // currently not importing tail and head style
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
        HashSet< Edge > edges = new HashSet< Edge >( this.Adjacency.Values );
        foreach ( Edge edge in edges )
            Graph.ExportText( fs, edge.ToString() + '\n' );
    }

    private static void ExportText( FileStream fs, string value )
    {
        byte[] info = new UTF8Encoding( true ).GetBytes( value );
        fs.Write( info, 0, info.Length );
    }


    // default algorithms /////////////////////////////////////////////////////

    public HashSet< Edge > GetIncidentEdges( Vertex vert )
    {
        return this.GetIncidentEdges( new HashSet< Vertex >() { vert } );
    }

    public HashSet< Edge > GetIncidentEdges( HashSet< Vertex > verts )
    {
        HashSet< Edge > incidentEdges = new HashSet< Edge >();
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( verts.Contains( kvp.Value.vert1 ) || !kvp.Value.Directed && verts.Contains( kvp.Value.vert2 ) )
                incidentEdges.Add( kvp.Value );
        }
        return incidentEdges;
    }

    public int GetVertexDegree( Vertex vert )
    {
        int degree = 0;
        int undirected = 0;
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
        {
            if ( kvp.Value.IncidentOn( vert ) )
            {
                if ( !kvp.Value.Directed )
                    undirected++;
                degree++;
            }
        }

        return degree - undirected / 2;
    }
}
