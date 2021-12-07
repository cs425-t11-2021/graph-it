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

        // Deselect all when the user clicks out of the graph
        if (Input.GetMouseButtonDown(0))
        {
            // Check if cursor is over collider, if not, deselect all graph objects
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex", "Edge", "UI"));  //11f since camera is at z = -10
            if (!hit)
            {
                //DeSelectAll();
            }
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
            Debug.Log("Deleting" + edgeObj.GetID());
            // Update the graph ds
            Controller.singleton.graph.RemoveEdge(edgeObj.GetID());

            Destroy(edgeObj.gameObject);
        }
        this.selectedEdges = new List<EdgeObj>();

        // For each vertex to be deleted, find all edges connecting to the vertex, then destroy the vertex object
        foreach (VertexObj vertexObj in this.selectedVertices)
        {
            // TODO: Find a faster way to do this without having to find all the edge objects in the scene
            EdgeObj[] allEdgeObjs = Controller.singleton.graphObj.GetComponentsInChildren<EdgeObj>();
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

    // Method called to remove all selections
    public void DeSelectAll()
    {
        Debug.Log("Deselecting All Components");
        for (int i = this.selectedEdges.Count - 1; i >= 0; i--)
        {
            this.selectedEdges[i].SetSelected(false);
        }
        this.selectedEdges = new List<EdgeObj>();
        for (int i = this.selectedVertices.Count - 1; i >= 0; i--)
        {
            this.selectedVertices[i].SetSelected(false);
        }
        this.selectedVertices = new List<VertexObj>();
    }

    // Method called to select all objects
    public void SelectAll()
    {
        Debug.Log("Selecting All Components");
        EdgeObj[] allEdgeObjs = Controller.singleton.graphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (!selectedEdges.Contains(edgeObj))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.singleton.graphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            if (!selectedVertices.Contains(vertexObj))
                vertexObj.SetSelected(true);
        }
    }
}
