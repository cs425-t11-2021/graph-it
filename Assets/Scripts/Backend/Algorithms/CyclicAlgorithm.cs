using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class CyclicAlgorithm : Algorithm
{
    public bool IsCyclic { get; private set; }

    public CyclicAlgorithm( AlgorithmManager algoManager ) : base( algoManager, algoManager.cyclicUI, algoManager.cyclicCalc ) { }

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
                if (IsCyclicHelper(u, visited, null))
                {
                    this.IsCyclic = true;
                    return;
                }
            }
        }

        this.IsCyclic = false;
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

    public static int GetHash() => typeof ( CyclicAlgorithm ).GetHashCode();

    public override int GetHashCode() => CyclicAlgorithm.GetHash();
}
