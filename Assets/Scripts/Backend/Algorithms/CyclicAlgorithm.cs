
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class CyclicAlgorithm : Algorithm
{
    private bool isCyclic;

    public CyclicAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        foreach ( Vertex u in this.Graph.Vertices )
        {
            visited[u] = false;
        }

        foreach ( Vertex u in this.Graph.Vertices )
        {
            if (!visited[u])
            {
                if (this.IsCyclicHelper(u, visited, null))
                {
                    this.isCyclic = true;
                    return;
                }
            }
        }

        this.isCyclic = false;
    }

    private bool IsCyclicHelper( Vertex vert, Dictionary< Vertex, bool > visited, Vertex parent )
    {
        visited[vert] = true;

        foreach ( Vertex u in this.Graph.Vertices )
        {
            if (this.Graph.IsAdjacent(vert, u))
            {
                if (!visited[u])
                {
                    if (IsCyclicHelper(u, visited, vert))
                    {
                        return true;
                    }
                } else if (!(parent is null) && u != parent)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "cyclic" ] = ( this.isCyclic, typeof ( bool ) );
        return result;
    }

    public static int GetHash() => typeof ( CyclicAlgorithm ).GetHashCode();

    public override int GetHashCode() => CyclicAlgorithm.GetHash();
}
