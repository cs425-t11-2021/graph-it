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

        // Testing: Press S to generate random circular graph
        if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("[Tester] Regenerating circular graph.");

            CreateCircularGraph();
            Controller.singleton.CreateGraphObjs();
        }

        // Testing: Press C to generate random complete graph
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log("[Tester] Regenerating complete graph.");

            CreateCompleteGraph();
            Controller.singleton.CreateGraphObjs();
        }

        // Testing: Press B to generate random bipartite graph
        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("[Tester] Regenerating bipartite graph.");

            CreateBipartiteGraph();
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
        }
    }

    private void CreateCircularGraph() {
        graph_ds = Controller.singleton.graph;

        int size = Random.Range(3, 20);

        // Add random number of vertices
        for (int i = 0; i < size; i++) {
            graph_ds.AddVertex();
        }

        // Connect adjacent vertices
        for (int i = 0; i < size - 1; i++) {
            graph_ds.AddEdge(graph_ds.vertices[i], graph_ds.vertices[i+1]);
        }
        graph_ds.AddEdge(graph_ds.vertices[0], graph_ds.vertices[size - 1]);
    }

    private void CreateCompleteGraph() {
        graph_ds = Controller.singleton.graph;

        int size = Random.Range(3, 10);

        // Add random number of vertices
        for (int i = 0; i < size; i++) {
            graph_ds.AddVertex();
        }

        // Connected vertex to every other vertex
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i != j) {
                    graph_ds.AddEdge(graph_ds.vertices[i], graph_ds.vertices[j]);
                }
            }
        }
    }

    private void CreateBipartiteGraph() {
        graph_ds = Controller.singleton.graph;

        int u_count = Random.Range(3, 6);
        int v_count = Random.Range(3, 6);

        for (int i = 0; i < u_count; i++) {
            graph_ds.AddVertex();
        }
        for (int i = 0; i < v_count; i++) {
            graph_ds.AddVertex();
        }

<<<<<<< HEAD
        // Print out info about the graph ds into console
        foreach (var kvp in graph_ds.incidence) {
            Debug.Log("Vertex: " + kvp.Key + " Edges: " + string.Join(",", kvp.Value));
=======
        for (int i = 0; i < u_count; i++) {
            for (int j = 0; j < v_count; j++) {
                if (Random.value < .5f) {
                    graph_ds.AddEdge(graph_ds.vertices[i], graph_ds.vertices[u_count + j]);
                }
                // if (Random.value < .33f) {
                //     graph_ds.AddEdge(graph_ds.vertices[u_count + j], graph_ds.vertices[i]);
                // }
            }
>>>>>>> main
        }
    }
}
