using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : MonoBehaviour
{

    // Reference to main graph ds
    private Graph graph_ds;

    private void Start() {
        CreateRandomGraph();
        // graph_ds.AddEdge(graph_ds.vertices[0], graph_ds.vertices[9]);
    }

    private void Update() {
        // Testing: Press D to delete exisintg graph
        if (Input.GetKeyDown(KeyCode.D)) {
            Debug.Log("[Tester] Deleting current graph.");

            Controller.singleton.ClearGraphObjs();
            Controller.singleton.graph = new Graph();
        }

        // Testing: Press R to regenerate random graph
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("[Tester] Regenerating random graph.");

            CreateRandomGraph();
            Controller.singleton.CreateGraphObjs();
        }
    }

    private void CreateRandomGraph() {
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
    }
}
