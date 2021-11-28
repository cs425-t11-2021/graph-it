using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : MonoBehaviour
{

    // Reference to main graph ds
    private Graph graph_ds;

    private void Start() {
        graph_ds = Controller.singleton.graph;

        // Add 10 vertices to the graph
        for (int i = 0; i < 10; i++) {
            graph_ds.AddVertex();
        }

        // Add 20 random edges to the graph
        for (int i = 0; i < 9; i++) {
            graph_ds.AddEdge(graph_ds.vertices[Random.Range(0, 10)], graph_ds.vertices[Random.Range(0, 10)]);
            // graph_ds.AddEdge(graph_ds.vertices[i], graph_ds.vertices[i+1]);
        }
        // graph_ds.AddEdge(graph_ds.vertices[0], graph_ds.vertices[9]);
    }
}
