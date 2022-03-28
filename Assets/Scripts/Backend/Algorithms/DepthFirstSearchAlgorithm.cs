
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DepthFirstSearchAlgorithm : LoggedAlgorithm
{
    public List< Edge > Tree { get; private set; }
    private Vertex root;
    private Action< Edge, Vertex > action;

    public DepthFirstSearchAlgorithm( AlgorithmManager algoManager,  bool display, Vertex root, Action< Edge, Vertex > action ) : base( algoManager )
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
        this.DepthFirstSearchHelper( this.root, visited, this.action );
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

    public static int GetHash( Vertex vert, Action< Edge, Vertex > action ) => ( typeof ( DepthFirstSearchAlgorithm ), vert, action ).GetHashCode();

    public override int GetHashCode() => DepthFirstSearchAlgorithm.GetHash( this.root, this.action );
}
