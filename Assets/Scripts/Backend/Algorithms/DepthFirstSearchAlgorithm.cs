
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class DepthFirstSearchAlgorithm : Algorithm
{
    public Vertex Root { get; private set; }
    public List< Edge > Tree { get; private set; }
    private Action< Edge, Vertex > action;

    public DepthFirstSearchAlgorithm( Graph graph, Vertex root, Action< Edge, Vertex > action, CancellationToken token, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, token, updateUI, updateCalc, markRunning, markComplete, unmarkRunning )
    {
        this.Root = root;
        this.action = action;
    }

    public override void Run()
    {
        this.Tree = new List< Edge >();
        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        this.DepthFirstSearchHelper( this.Root, visited, this.action );
    }

    private void DepthFirstSearchHelper( Vertex vert, Dictionary< Vertex, bool > visited, Action< Edge, Vertex > f )
    {
        foreach ( Edge edge in this.Graph.GetIncidentEdges( vert ) )
        {
            if ( !visited.GetValue( edge.vert1 ) )
            {
                this.Tree.Add( edge );
                visited[ edge.vert1 ] = true;
                f( edge, edge.vert1 );
                this.DepthFirstSearchHelper( edge.vert1, visited, f );
            }
            if ( this.IsKillRequested() )
                this.Kill();
            else if ( !visited.GetValue( edge.vert2 ) )
            {
                this.Tree.Add( edge );
                visited[ edge.vert2 ] = true;
                f( edge, edge.vert2 );
                this.DepthFirstSearchHelper( edge.vert2, visited, f );
            }
            if ( this.IsKillRequested() )
                this.Kill();
        }
    }

    public static int GetHash( Vertex vert ) => ( typeof ( DepthFirstSearchAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => DepthFirstSearchAlgorithm.GetHash( this.Root );
}
