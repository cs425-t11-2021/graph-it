using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : MonoBehaviour
{

    // Reference to main graph ds
    private Graph graph_ds;

    private void Start() {
        CreateRandomGraph(10);
    }

    private void Update() {
        // Ignore keyboard input if UI active
        if (Controller.singleton.IsUIactive()) return;

        // Testing: Press D to delete exisintg graph
        if (Input.GetKeyDown(KeyCode.D)) {
            Debug.Log("[Tester] Deleting current graph.");

            Controller.singleton.ClearGraphObjs();
            Controller.singleton.graph = new Graph();
        }

        // Testing: Press R to regenerate random graph
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("[Tester] Regenerating random graph.");

            CreateRandomGraph(5);
            Controller.singleton.CreateGraphObjs();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            CreateRandomGraph(3);
            Controller.singleton.CreateGraphObjs();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            CreateRandomGraph(4);
            Controller.singleton.CreateGraphObjs();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            CreateRandomGraph(5);
            Controller.singleton.CreateGraphObjs();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            CreateRandomGraph(6);
            Controller.singleton.CreateGraphObjs();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            CreateRandomGraph(7);
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

    private void CreateRandomGraph(int num) {
        Controller.singleton.ClearGraphObjs();
        Controller.singleton.graph = new Graph();

        graph_ds = Controller.singleton.graph;

        // Add 10 vertices to the graph
        for (int i = 0; i < num; i++) {
            graph_ds.AddVertex();
        }

        List<(Vertex, Vertex)> existing = new List<(Vertex, Vertex)>();
        // Add 9 random edges to the graph
        for (int i = 0; i < num; i++) {
            Vertex from = graph_ds.vertices[Random.Range(0, graph_ds.vertices.Count)];
            Vertex to = graph_ds.vertices[Random.Range(0, graph_ds.vertices.Count)];

            if (from != to && !existing.Contains((from, to)))
            {
                graph_ds.AddEdge(from, to);
                existing.Add((from, to));
                existing.Add((to, from));
            }
        }
    }

    private void CreateCircularGraph() {
        Controller.singleton.ClearGraphObjs();
        Controller.singleton.graph = new Graph();

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
        Controller.singleton.ClearGraphObjs();
        Controller.singleton.graph = new Graph();

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
        Controller.singleton.ClearGraphObjs();
        Controller.singleton.graph = new Graph();

        graph_ds = Controller.singleton.graph;

        int u_count = Random.Range(3, 6);
        int v_count = Random.Range(3, 6);

        for (int i = 0; i < u_count; i++) {
            graph_ds.AddVertex();
        }
        for (int i = 0; i < v_count; i++) {
            graph_ds.AddVertex();
        }

        // Print out info about the graph ds into console
        for (int i = 0; i < u_count; i++) {
            for (int j = 0; j < v_count; j++) {
                if (Random.value < .5f) {
                    graph_ds.AddEdge(graph_ds.vertices[i], graph_ds.vertices[u_count + j]);
                }
            }
        }
    }
}
