using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : MonoBehaviour
{

    Graph graph_ds;

    private void Start() {
        graph_ds = Controller.singleton.graph;

        // Add 10 vertices to the graph
        for (int i = 0; i < 10; i++) {
            graph_ds.AddNode();
        }

        // Add 10 random edges to the graph
        for (int i = 0; i < 10; i++) {
            graph_ds.AddEdge(Random.Range(0, 10), Random.Range(0, 10));
        }

        // Print out info about the graph ds into console
        foreach (var kvp in graph_ds.adj) {
            Debug.Log("Vertex: " + kvp.Key + " Edges: " + string.Join(",", kvp.Value));
        }
    }
}
