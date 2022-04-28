
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class BreadthFirstSearchAlgorithm : LoggedAlgorithm
{
    private Vertex root;
    private Action< Edge, Vertex > action;

    private List< Vertex > treeVertices;
    private List< Edge > treeEdges;

    public BreadthFirstSearchAlgorithm( AlgorithmManager algoManager, bool display, Vertex root, Action< Edge, Vertex > action ) : base( algoManager )
    {
        this.root = root;
        this.action = action;

        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
        // Add the root vertex to vertex parms array
        vertexParms = new Vertex[] { this.root };
    }

    public override void Run()
    {
        this.treeVertices = new List< Vertex >() { this.root };
        this.treeEdges = new List< Edge >();
        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        Queue< Vertex > queue = new Queue< Vertex >();
        queue.Enqueue( this.root );
        while ( queue.Count > 0 )
        {
            Vertex vert1 = queue.Dequeue();
            foreach ( Edge edge in this.Graph.GetIncidentEdges( vert1 ) )
            {
                Vertex vert2 = edge.vert1 == vert1 ? edge.vert2 : edge.vert1;
                if ( !visited.GetValue( vert2 ) )
                {
                    visited[ vert2 ] = true;
                    queue.Enqueue( vert2 );
                    this.treeVertices.Add( vert2 );
                    this.treeEdges.Add( edge );
                    this.action( edge, vert2 );
                }
            }
        }
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "bfs vertices" ] = ( this.treeVertices, typeof ( List< Vertex > ) );
        result.results[ "bfs edges" ] = ( this.treeEdges, typeof ( List< Edge > ) );
        return result;
    }

    public static int GetHash( Vertex vert, Action< Edge, Vertex > action ) => ( typeof ( BreadthFirstSearchAlgorithm ), vert, action ).GetHashCode();

    public override int GetHashCode() => BreadthFirstSearchAlgorithm.GetHash( this.root, this.action );
}
