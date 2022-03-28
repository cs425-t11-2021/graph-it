
// All code developed by Team 11

using System;
using System.Collections.Generic;

[System.Serializable]
public class FleurysAlgorithm : Algorithm
{
    public bool EulerianCircuitExists { get; private set; }
    public List< Edge > Result { get; private set; }

    private Graph graphCopy;

    public FleurysAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager )
    {
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
    }

    public override void Run()
    {
        graphCopy = new Graph(this.Graph);

        // check first if graph is an Euler graph:
        //      - there are either 0 or 2 vertices with odd degree
        //      - graph is connected

        Vertex initial = this.graphCopy.Vertices[0];
        this.Result = new List< Edge >();

        int numOddDegrees = 0;
        foreach ( Vertex u in this.graphCopy.Vertices )
        {
            if (this.graphCopy.GetVertexDegree(u) % 2 == 1) 
            {
                initial = u;
                numOddDegrees++;
            }
        }

        if (numOddDegrees != 0 && numOddDegrees != 2)
        {
            this.EulerianCircuitExists = false;
            return;
        }

        this.EulerianCircuitExists = true;


        // run Depth First search to determine if graph is connected

        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        foreach (Vertex vert in this.graphCopy.Vertices)
        {
            visited[vert] = false;
        }

        if (dfsCount(this.graphCopy.Vertices[0], visited) < this.graphCopy.Vertices.Count)
        {
            this.EulerianCircuitExists = false;
            return;
        }

        // start from an odd degree vertex (if it exists, otherwise any vertex)
        // traverse through the graph until a circuit is formed, by always choosing a "non-bridge" over a "bridge"
        Vertex curr = initial;
        while (this.graphCopy.GetVertexDegree(curr) > 0)
        {
            foreach (Vertex vert in this.graphCopy.Vertices)
            {
                if (this.graphCopy.IsAdjacent(curr, vert))
                {
                    if (isValidNextEdge(curr, vert))
                    {
                        this.Result.Add(this.graphCopy[curr, vert]);
                        this.graphCopy.Remove(this.graphCopy[curr, vert], false);
                        curr = vert;
                        break;
                    }
                }
            }
        }
    }

    private bool isValidNextEdge( Vertex u, Vertex v)
    {
        // The edge u-v is valid in one of the
        // following two cases:
 
        // 1) If v is the only adjacent vertex of u
        // ie size of adjacent vertex list is 1
        if (this.graphCopy.GetVertexDegree(u) == 1)
        {
            return true;
        }
 
        // 2) If there are multiple adjacents, then
        // u-v is not a bridge Do following steps
        // to check if u-v is a bridge
        // 2.a) count of vertices reachable from u
        Dictionary< Vertex, bool > visited = new Dictionary< Vertex, bool >();
        foreach (Vertex vert in this.graphCopy.Vertices)
        {
            visited[vert] = false;
        }
        int count1 = dfsCount(u, visited);
 
        // 2.b) Remove edge (u, v) and after removing
        // the edge, count vertices reachable from u
        Edge e = this.graphCopy[u, v];
        this.graphCopy.Remove(e, false);

        foreach (Vertex vert in this.graphCopy.Vertices)
        {
            visited[vert] = false;
        }
        int count2 = dfsCount(u, visited);
 
        // 2.c) Add the edge back to the graph
        this.graphCopy.AddEdge(e, false);
        return count1 == count2;
    }

    private int dfsCount( Vertex v, Dictionary< Vertex, bool > visited )
    {
        // mark the current node as visited
        visited[v] = true;
        int count = 1;
         
        // iterate over adjacencies of v
        foreach( Vertex u in this.graphCopy.Vertices )
        {
            if (this.graphCopy.IsAdjacent(v, u))
            {
                if (!visited[u])
                {
                    count = count + dfsCount(u, visited);
                }
            }
        }
        return count;
    }

    public static int GetHash() => typeof ( FleurysAlgorithm ).GetHashCode();

    public override int GetHashCode() => FleurysAlgorithm.GetHash();
}
