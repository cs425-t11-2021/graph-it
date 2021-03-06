
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
    public List< Vertex > Vertices { get; private set; }
    // not yet supporting multiple edges
    public Dictionary< ( Vertex, Vertex ), Edge > Adjacency { get; private set; }

    // logs all changes to graph
    public Stack< GraphModification > Changes { get; private set; }
    // logs all undone changes
    public Stack< GraphModification > UndoneChanges { get; private set; }

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
        get
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
            {
                if ( kvp.Value.Directed )
                    return true;
            }
            return false;
        }
    }

    public bool Weighted // true if any edge is weighted
    {
        get
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
            {
                if ( kvp.Value.Weighted )
                    return true;
            }
            return false;
        }
    }

    public bool FullyWeighted // true when all edges are weighted
    {
        get
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
            {
                if ( !kvp.Value.Weighted )
                    return false;
            }
            return true;
        }
    }

    public bool NegativeWeighted // true is there is an edge with negative weight
    {
        get
        {
            foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.Adjacency )
            {
                if ( kvp.Value.Weight < 0 )
                    return true;
            }
            return false;
        }
    }

    public bool Simple // false if multiple edges. false if loops on vertices (unless directed)
    {
        get
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
    }

    public bool HasLoops // true if a loop (an edge incident to only one vertex) exists
    {
        get
        {
            foreach ( Vertex vert in this.Vertices )
            {
                if ( !( this[ vert, vert ] is null ) )
                    return true;
            }
            return false;
        }
    }

    public bool HasMultipleEdges // true if there are multiple edges between vertices (not parallel directed edges)
    {
        get => false;
    }


    public Graph()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
        this.Changes = new Stack< GraphModification >();
        this.UndoneChanges = new Stack< GraphModification >();
    }

    public Graph( Graph graph )
    {
        this.Vertices = new List< Vertex >( graph.Vertices );
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >( graph.Adjacency );
        this.Changes = new Stack< GraphModification >();
        this.UndoneChanges = new Stack< GraphModification >();
    }

    public void Clear()
    {
        this.Vertices = new List< Vertex >();
        this.Adjacency = new Dictionary< ( Vertex, Vertex ), Edge >();
        this.Changes = new Stack< GraphModification >();
        this.UndoneChanges = new Stack< GraphModification >();
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


    public ( List< Vertex >, List< Edge > ) Add( List< Vertex > vertices, List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >( vertices ), new HashSet< Edge >( edges ) ) );

        this.Add( vertices, false );
        this.Add( edges, false );
        return ( vertices, edges );
    }

    public ( List< Vertex >, Edge ) Add( List< Vertex > vertices, Edge edge, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >( vertices ), new HashSet< Edge >() { edge } ) );

        this.Add( vertices, false );
        this.Add( edge, false );
        return ( vertices, edge );
    }

    public ( Vertex, List< Edge > ) Add( Vertex vert, List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >() { vert }, new HashSet< Edge >( edges ) ) );

        this.Add( vert, false );
        this.Add( edges, false );
        return ( vert, edges );
    }

    public List< Vertex > Add( List< Vertex > vertices, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >( vertices ), ( HashSet< Edge > ) null ) );

        foreach ( Vertex vert in vertices )
            this.Add( vert, false );
        return vertices;
    }

    public List< Edge > Add( List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( ( HashSet< Vertex > ) null, new HashSet< Edge >( edges ) ) );

        foreach ( Edge edge in edges )
            this.Add( edge, false );
        return edges;
    }

    public ( Vertex, Edge ) Add( Vertex vert, Edge edge, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >() { vert }, new HashSet< Edge >() { edge } ) );

        this.Add( vert, false );
        this.Add( edge, false );
        return ( vert, edge );
    }

    public Vertex Add( Vertex vert, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( new HashSet< Vertex >() { vert }, ( HashSet< Edge > ) null ) );

        vert.CreateMod = ( Action< Modification, System.Object > ) this.CreateModificationAction;
        this.Vertices.Add( vert );
        return vert;
    }

    // TODO: add more parameters
    public Vertex AddVertex( float x, float y, bool recordChange=true )
    {
        return this.Add( new Vertex( x : x, y : y ), recordChange );
    }

    public Edge Add( Edge edge, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.ADD_COLLECTION, ( ( HashSet< Vertex > ) null, new HashSet< Edge >() { edge } ) );

        if ( !this.Vertices.Contains( edge.vert1 ) || !this.Vertices.Contains( edge.vert2 ) )
            throw new System.Exception( "Edge is incident to one or more vertices that have not been added to the graph." );

        edge.MakeDirectedInGraph = ( Action< Edge > ) this.MakeEdgeDirectedAction;
        edge.MakeUndirectedInGraph = ( Action< Edge > ) this.MakeEdgeUndirectedAction;
        edge.ReverseInGraph = ( Action< Edge > ) this.ReverseEdgeAction;
        edge.CreateMod = ( Action< Modification, System.Object > ) this.CreateModificationAction;

        this.Adjacency[ ( edge.vert1, edge.vert2 ) ] = edge;
        if ( !edge.Directed )
            this.Adjacency[ ( edge.vert2, edge.vert1 ) ] = edge;

        return edge;
    }

    public Edge AddEdge( Vertex vert1, Vertex vert2, bool directed=false, bool recordChange=true )
    {
        return this.Add( new Edge( vert1, vert2, directed ), recordChange );
    }

    public void Remove( List< Vertex > vertices, List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vertexSet );
            edgeSet.UnionWith( edges );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, edgeSet ) );
        }

        this.Remove( edges, false );
        this.Remove( vertices, false );
    }

    public void Remove( List< Vertex > vertices, Edge edge, bool recordChange=true )
    {
        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vertexSet );
            edgeSet.Add( edge );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, edgeSet ) );
        }

        this.Remove( vertices, false );
        this.Remove( edge, false );
    }

    public void Remove( Vertex vert, List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
        {
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vert );
            edgeSet.UnionWith( edges );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, edgeSet ) );
        }

        this.Remove( vert, false );
        this.Remove( edges, false );
    }

    public void Remove( List< Vertex > vertices, bool recordChange=true )
    {
        if ( recordChange )
        {
            HashSet< Vertex > vertexSet = new HashSet< Vertex >( vertices );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( vertexSet, this.GetIncidentEdges( vertexSet ) ) );
        }

        foreach ( Vertex vert in vertices )
            this.Remove( vert, false );
    }

    public void Remove( List< Edge > edges, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( ( HashSet< Edge > ) null, new HashSet< Edge >( edges ) ) );

        foreach ( Edge edge in edges )
            this.Remove( edge, false );
    }

    public void Remove( Vertex vert, Edge edge, bool recordChange=true )
    {
        if ( recordChange )
        {
            HashSet< Edge > edgeSet = this.GetIncidentEdges( vert );
            edgeSet.Add( edge );
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, edgeSet ) );
        }

        this.Remove( vert, false );
        this.Remove( edge, false );
    }

    public void Remove( Vertex vert, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( new HashSet< Vertex >() { vert }, this.GetIncidentEdges( vert ) ) );

        this.Remove( this.Adjacency.Values.Where( edge => edge.vert1 == vert || edge.vert2 == vert ).ToList(), false );
        this.Vertices.Remove( vert );
    }

    public void Remove( Edge edge, bool recordChange=true )
    {
        if ( recordChange )
            new GraphModification( this, Modification.REMOVE_COLLECTION, ( ( HashSet< Vertex > ) null, new HashSet< Edge >() { edge } ) );

        if ( edge.Directed )
            this.Adjacency.Remove( ( edge.vert1, edge.vert2 ) );
        else
        {
            this.Adjacency.Remove( ( edge.vert1, edge.vert2 ) );
            this.Adjacency.Remove( ( edge.vert2, edge.vert1 ) );
        }
    }

    public void Undo()
    {
        GraphModification mod = this.Changes.Pop();
        mod.Undo();
        this.UndoneChanges.Push( mod );
    }

    public void Redo()
    {
        GraphModification mod = this.UndoneChanges.Pop();
        mod.Redo();
        this.Changes.Push( mod );
    }

    public bool IsAdjacent( Vertex vert1, Vertex vert2 ) => this.IsAdjacentDirected( vert1, vert2 ) || this.IsAdjacentDirected( vert2, vert1 );

    public bool IsAdjacentDirected( Vertex vert1, Vertex vert2 ) => this.Adjacency.ContainsKey( ( vert1, vert2 ) );

    public bool IsIncident( Edge edge1, Edge edge2 ) => edge1.IncidentOn( edge2.vert1 ) || edge1.IncidentOn( edge2.vert2 );


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
            this.Add( this.ParseVertex( line, indices ), false );
        else
            this.Add( this.ParseEdge( line, indices ), false );
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

    public HashSet< Edge > GetIncidentEdges( IEnumerable< Vertex > verts )
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
