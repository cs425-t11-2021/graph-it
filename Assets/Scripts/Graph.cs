using System.Collections;
using System.Collections.Generic;

// first attempt at a graph data structure
// currently assumes the graph is undirected, can change later
public class Graph
{
    public Dictionary<int, List<int>> adj;

    private int next_id = 0;

    // What is this method supposed to return? -Jimson
    public int AddNode()
    {
        adj.Add(next_id, new List<int>());
        next_id++;

        // Tempoary - Jimson
        return 0;
    }

    public void AddEdge(int u, int v)
    {
        if (!(adj.ContainsKey(u) && adj.ContainsKey(v)))
        {
            throw new System.Exception("One or more nodes have not been added to the graph.");
        }

        if (u == v)
        {
            adj[u].Add(u);
        }
        else
        {
            adj[u].Add(v);
            adj[v].Add(u);
        }
    }

    // What is this method supposed to return? -Jimson
    public int RemoveNode(int u)
    {
        adj.Remove(u);

        foreach ( KeyValuePair<int, List<int>> kvp in adj)
        {
            // removes all occurences of u in the linked list
            while (kvp.Value.Remove(u));
        }

        // Tempoary
        return 0;
    }
}
