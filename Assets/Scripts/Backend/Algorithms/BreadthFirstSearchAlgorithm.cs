
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class BreadthFirstSearchAlgorithm : Algorithm
{
    public Vertex Root { get; private set; }
    public List< Edge > Tree { get; private set; }
    private Action< Edge, Vertex > action;

    public BreadthFirstSearchAlgorithm( Graph graph, Vertex root, Action< Edge, Vertex > action, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning )
    {
        this.Root = root;
        this.action = action;
    }

    public override void Run()
    {
        this.Tree = new List< Edge >();
        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        Queue< Vertex > queue = new Queue< Vertex >();
        queue.Enqueue( this.Root );
        while ( queue.Count > 0 )
        {
            Vertex vert1 = queue.Dequeue();
            foreach ( Edge edge in this.graph.GetIncidentEdges( vert1 ) )
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

    public static int GetHash( Vertex vert ) => ( typeof ( BreadthFirstSearchAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => BreadthFirstSearchAlgorithm.GetHash( this.Root );
}
