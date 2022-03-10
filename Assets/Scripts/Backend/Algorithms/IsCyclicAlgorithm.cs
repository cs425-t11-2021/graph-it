using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class IsCyclicAlgorithm : Algorithm
{
    public bool isCyclic { get; private set; }
    public IsCyclicAlgorithm(
        Graph graph,
        Action updateUI,
        Action updateCalc,
        Action< Algorithm > markRunning,
        Action< Algorithm > markComplete,
        Action< Algorithm > unmarkRunning )
            : base(
                graph,
                updateUI,
                updateCalc,
                markRunning,
                markComplete,
                unmarkRunning ) {}

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
                    isCyclic = true;
                    return;
                }
            }
        }

        isCyclic = false;
    }

    private bool IsCyclicHelper( Vertex vert, Dictionary< Vertex, bool > visited, Vertex parent )
    {
        visited[vert] = true;

        foreach ( Vertex u in this.Graph.Vertices )
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

        return false;
    }

    public static int GetHash() => ( typeof ( IsCyclicAlgorithm ) ).GetHashCode();

    public override int GetHashCode() => IsCyclicAlgorithm.GetHash();
}
