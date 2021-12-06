using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // Singleton
    public static SelectionManager singleton;

    // Lists to store the indices of currently selected vertices and edges
    private List<int> selectedVertices;
    private List<int> selectedEdges;

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
        this.selectedVertices = new List<int>();
        this.selectedEdges = new List<int>();
    }

    // Add a vertex to selectedVertices
    public void SelectVertex(int id)
    {
        this.selectedVertices.Add(id);
    }

    // Add an to selectedEdges
    public void SelectEdge(int id)
    {
        this.selectedEdges.Add(id);
    }

    // Remove a vertex from selectedVertices
    public bool DeselectVertex(int id)
    {
        return this.selectedVertices.Remove(id);
    }

    // Remove am edge from selectedEdges
    public bool DeselectEdge(int id)
    {
        return this.selectedEdges.Remove(id);
    }

    // Method called to delete the currently selected vertices and edges, updates graph objects and graph database
    public void DeleteSelection()
    {
        // Destroy the graph objects corresponding to the currently selected vertices and edges
        // TODO: object pooling
        Transform graphObj = Controller.singleton.graphObj;
        for (int i = graphObj.childCount - 1; i >= 0; i--)
        {
            // If the vertex object's id is in the selectedVertices list, destroy its gameObject
            VertexObj vertexObj = graphObj.GetChild(i).GetComponent<VertexObj>();
            if (selectedVertices.Contains(vertexObj.GetID())) {
                Destroy(vertexObj.gameObject);
            }
            // Else look through its edges and destroy any edge objects with id in selectedEdges
            else
            {
                for (int j = graphObj.GetChild(i).childCount - 1; j >= 0; j--)
                {
                    EdgeObj edgeObj = graphObj.GetChild(i).GetChild(j).GetComponent<EdgeObj>();
                    if (selectedEdges.Contains(edgeObj.GetID()))
                    {
                        Destroy(edgeObj.gameObject);
                    }
                }
            }
        }

        // Delete the selected vertices and edges from graph ds
        foreach (int edgeID in selectedEdges)
        {
            Controller.singleton.graph.RemoveEdge(edgeID);
        }
        foreach (int vertexID in selectedEdges)
        {
            Controller.singleton.graph.RemoveVertex(vertexID);
        }
    }
}
