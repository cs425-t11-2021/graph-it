using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // Singleton
    public static SelectionManager singleton;

    // Lists to store the graph objects of currently selected vertices and edges
    // TODO: Figure out a way of doing this without storing Unity objects
    private List<VertexObj> selectedVertices;
    private List<EdgeObj> selectedEdges;

    private void Awake()
    {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[SelectionManager] Singleton pattern violation");
            Destroy(this);
            return;
        }

        // Initialize data structures
        this.selectedVertices = new List<VertexObj>();
        this.selectedEdges = new List<EdgeObj>();
    }

    private void Update()
    {
        // Delete selection if backspace or delete key is pressed
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelection();
        }
    }

    // Add a vertex to selectedVertices
    public void SelectVertex(VertexObj vertexObj)
    {
        this.selectedVertices.Add(vertexObj);
    }

    // Add an to selectedEdges
    public void SelectEdge(EdgeObj edgeObj)
    {
        this.selectedEdges.Add(edgeObj);
    }

    // Remove a vertex from selectedVertices
    public bool DeselectVertex(VertexObj vertexObj)
    {
        return this.selectedVertices.Remove(vertexObj);
    }

    // Remove am edge from selectedEdges
    public bool DeselectEdge(EdgeObj edgeObj)
    {
        return this.selectedEdges.Remove(edgeObj);
    }

    // Method called to delete the currently selected vertices and edges, updates graph objects and graph database
    public void DeleteSelection()
    {
        // Destroy the graph objects corresponding to the currently selected vertices and edges
        // TODO: object pooling

        // Destroy the gameObjects for edges in selectedEdges
        foreach (EdgeObj edgeObj in this.selectedEdges)
        {
            // Update the graph ds
            Controller.singleton.graph.RemoveEdge(edgeObj.GetID());

            Destroy(edgeObj.gameObject);
        }
        this.selectedEdges = new List<EdgeObj>();

        // For each vertex to be deleted, find all edges connecting to the vertex, then destroy the vertex object
        foreach (VertexObj vertexObj in this.selectedVertices)
        {
            // TODO: Find a faster way to do this without having to find all the edge objects in the scene
            EdgeObj[] allEdgeObjs = GameObject.FindObjectsOfType<EdgeObj>();
            foreach (EdgeObj edgeObj in allEdgeObjs)
            {
                if (edgeObj.targetVertexObj == vertexObj.gameObject)
                {
                    Destroy(edgeObj.gameObject);
                }
            }

            // Update the graph ds
            Controller.singleton.graph.RemoveVertex(vertexObj.GetID());

            Destroy(vertexObj.gameObject);
        }
        this.selectedVertices = new List<VertexObj>();
    }
}
