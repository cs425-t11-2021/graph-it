
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DepthFirstSearchAlgorithm : Algorithm
{
    public Vertex Root { get; private set; }
    public List< Edge > Tree { get; private set; }
    private Action< Edge, Vertex > action;

    public DepthFirstSearchAlgorithm( AlgorithmManager algoManager,  bool display, Vertex root, Action< Edge, Vertex > action ) : base( algoManager )
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
            else if ( !visited.GetValue( edge.vert2 ) )
            {
                this.Tree.Add( edge );
                visited[ edge.vert2 ] = true;
                f( edge, edge.vert2 );
                this.DepthFirstSearchHelper( edge.vert2, visited, f );
            }
        }
    }

    public static int GetHash( Vertex vert ) => ( typeof ( DepthFirstSearchAlgorithm ), vert ).GetHashCode();

    public override int GetHashCode() => DepthFirstSearchAlgorithm.GetHash( this.Root );
}
