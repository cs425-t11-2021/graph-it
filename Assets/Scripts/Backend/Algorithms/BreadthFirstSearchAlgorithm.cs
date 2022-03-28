
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class BreadthFirstSearchAlgorithm : LoggedAlgorithm
{
    public List< Edge > Tree { get; private set; }
    private Vertex root;
    private Action< Edge, Vertex > action;

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
        this.Tree = new List< Edge >();
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
                    this.Tree.Add( edge );
                    this.action( edge, vert2 );
                }
            }
        }
    }

    public static int GetHash( Vertex vert, Action< Edge, Vertex > action ) => ( typeof ( BreadthFirstSearchAlgorithm ), vert, action ).GetHashCode();

    public override int GetHashCode() => BreadthFirstSearchAlgorithm.GetHash( this.root, this.action );
}
