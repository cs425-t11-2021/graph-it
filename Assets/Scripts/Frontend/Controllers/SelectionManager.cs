//All code developed by Team 11
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : SingletonBehavior<SelectionManager>
{
    // Lists to store the graph objects of currently selected vertices and edges
    private List<VertexObj> selectedVertices;
    private List<EdgeObj> selectedEdges;

    // Readonly Properties for the above lists
    public List<VertexObj> SelectedVertices { get => this.selectedVertices; }
    public List<EdgeObj> SelectedEdges { get => this.selectedEdges; }

    // Event for when the selection is updated (select or deselect)
    public event Action<int, int> OnSelectionChange;

    private void Awake()
    {
        // Initialize data structures
        this.selectedVertices = new List<VertexObj>();
        this.selectedEdges = new List<EdgeObj>();
    }

    // Select a vertex if it's not selected
    public void SelectVertex(VertexObj vertexObj, bool callEvent = true)
    {
        if (!vertexObj.Selected) {
            SetVertexSelection(vertexObj, true, callEvent);
        }
    }

    // Unselect a vertex if it is selected
    public void DeselectVertex(VertexObj vertexObj, bool callEvent = true)
    {
        if (vertexObj.Selected) {
            SetVertexSelection(vertexObj, false, callEvent);
        }
    }

    // Select the vertex if it's currently unselected, deselect it otherwise
    public void ToggleVertexSelection(VertexObj vertexObj) {
        SetVertexSelection(vertexObj, !vertexObj.Selected, true);
    }

    // Add a vertex to the list of selected vertices or remove it
    private void SetVertexSelection(VertexObj vertexObj, bool select, bool callEvent) {
        // Add or remove vertex object from selected list
        if (vertexObj.Selected) {
            this.selectedVertices.Remove(vertexObj);
        }
        else {
            this.selectedVertices.Add(vertexObj);
        }
        vertexObj.Selected = select;

        if (callEvent)
            // Call selection changed event
            this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);

        Logger.Log(String.Format("{0} vertex: {1}.", select ? "Selecting" : "Deselecting", vertexObj.name), this, LogType.INFO);
    }

    // Select and edge if it's unselected
    public void SelectEdge(EdgeObj edgeObj, bool callEvent = true)
    {
        if (!edgeObj.Selected) {
            SetEdgeSelection(edgeObj, true, callEvent);
        }
    }

    // Deselect and edge if it is selected
    public void ToggleEdgeSelection(EdgeObj edgeObj) {
        SetEdgeSelection(edgeObj, !edgeObj.Selected, true);
    }

    public void DeselectEdge(EdgeObj edgeObj, bool callEvent = true)
    {
        if (edgeObj.Selected) {
            SetEdgeSelection(edgeObj, false, callEvent);
        }
    }

    // Set the selectedness of an edge object
    private void SetEdgeSelection(EdgeObj edgeObj, bool select, bool callEvent) {
        // Add or remove vertex object from selected list
        if (edgeObj.Selected) {
            this.selectedEdges.Remove(edgeObj);
        }
        else {
            this.selectedEdges.Add(edgeObj);
        }
        edgeObj.Selected = select;

        if (callEvent)
            // Call selection changed event
            this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);

        Logger.Log(String.Format("{0} edge: {1}.", select ? "Selecting" : "Deselecting", edgeObj.name), this, LogType.INFO);
    }

    // Method called to delete the currently selected vertices and edges, updates graph objects and graph database
    public void DeleteSelection()
    {
        // Destroy the graph objects corresponding to the currently selected vertices and edges
        // Destroy the gameObjects for edges in selectedEdges
        foreach (EdgeObj edgeObj in this.selectedEdges)
        {
            // Update the graph ds
            Controller.Singleton.Graph.RemoveEdge(edgeObj.Edge);
            Destroy(edgeObj.transform.parent.gameObject);
        }

        this.selectedEdges = new List<EdgeObj>();

        // For each vertex to be deleted, find all edges connecting to the vertex, then destroy the vertex object
        foreach (VertexObj vertexObj in this.selectedVertices)
        {
            foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
            {
                if (edgeObj.ToVertexObj == vertexObj.gameObject)
                {
                    Destroy(edgeObj.transform.parent.gameObject);
                }
            }

            // Update the graph ds
            Controller.Singleton.Graph.RemoveVertex(vertexObj.Vertex);

            Destroy(vertexObj.gameObject);
        }
        this.selectedVertices = new List<VertexObj>();

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Method called to remove all selections
    public void DeSelectAll()
    {
        for (int i = this.selectedEdges.Count - 1; i >= 0; i--)
        {
            DeselectEdge(this.selectedEdges[i], false);
        }
        this.selectedEdges = new List<EdgeObj>();
        for (int i = this.selectedVertices.Count - 1; i >= 0; i--)
        {
            DeselectVertex(this.selectedVertices[i], false);
        }
        // this.selectedVertices = new List<VertexObj>();

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Method called to select all objects
    public void SelectAll()
    {
        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (!edgeObj.Selected)
                SelectEdge(edgeObj, false);
        }

        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (!vertexObj.Selected)
                SelectVertex(vertexObj, false);
        }

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Gets the number of vertices currently selected
    public int SelectedVertexCount() {
        return selectedVertices.Count;
    }

    // Gets the number of edges currently selected
    public int SelectedEdgeCount() {
        return selectedEdges.Count;
    }

    // TODO: Move somewhere else
    // Runs the prim algorithm on the currently selected vertex
    public void RunPrims() {
        if (selectedVertices.Count != 1) {
            Debug.Log( ( new System.Exception( "Cannot start Prim's algorithm." ) ).ToString() );
            throw new System.Exception( "Cannot start Prim's algorithm." );
        }

        List<Edge> primEdges = Controller.Singleton.Graph.Prim(selectedVertices[0].Vertex);
        List<Vertex> primVertices = new List<Vertex>();
        foreach (Edge e in primEdges) {
            primVertices.Add(e.vert1);
            primVertices.Add(e.vert2);
        }

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (primEdges.Contains(edgeObj.Edge))
                SelectEdge(edgeObj);
        }

        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (primVertices.Contains(vertexObj.Vertex))
                SelectVertex(vertexObj);
        }
    }
    public void RunKruskal() {
        List<Edge> kruskalEdges = Controller.Singleton.Graph.Kruskal();
        List<Vertex> kruskalVertices = new List<Vertex>();
        foreach (Edge e in kruskalEdges) {
            kruskalVertices.Add(e.vert1);
            kruskalVertices.Add(e.vert2);
        }

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (kruskalEdges.Contains(edgeObj.Edge))
                SelectEdge(edgeObj);
        }

        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (kruskalVertices.Contains(vertexObj.Vertex))
                SelectVertex(vertexObj);
        }
    }

    public void RunDijkstra() {
        if (selectedVertices.Count != 2) {
            Debug.Log( ( new System.Exception( "Cannot start Dijkstra's algorithm." ) ).ToString() );
            throw new System.Exception( "Cannot start Dijkstra's algorithm." );
        }

        List<Edge> dijkstraEdges = new List<Edge>();
        Debug.Log(selectedVertices[0].Vertex);
        Debug.Log(selectedVertices[1].Vertex);
        List<Vertex> dijkstraVertices = Controller.Singleton.Graph.Dijkstra(selectedVertices[0].Vertex, selectedVertices[1].Vertex);
        // foreach (Edge e in dijkstraEdges) {
        //     dijkstraVertices.Add(e.vert1);
        //     dijkstraVertices.Add(e.vert2);
        // }

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (dijkstraEdges.Contains(edgeObj.Edge))
                SelectEdge(edgeObj);
        }

        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (dijkstraVertices.Contains(vertexObj.Vertex))
                SelectVertex(vertexObj);
        }
    }

    // Add a new edge between the first two selected vertices
    public void AddEdgeBetweenTwoSelectedVertices() {
        VertexObj vertexObj1 = selectedVertices[0];
        VertexObj vertexObj2 = selectedVertices[1];

        Controller.Singleton.AddEdge(vertexObj1.Vertex, vertexObj2.Vertex);
        DeSelectAll();
    }

    // Add a new edge between the first vertexObj selected and a passed in vertexObj
    public void AddEdge(VertexObj target) {
        Controller.Singleton.AddEdge(selectedVertices[0].Vertex, target.Vertex);
    }

    // Change the type of selected edges
    public void ChangeSelectedEdgesType() {
        this.selectedEdges.ForEach(e => e.ToggleEdgeType());
    }
}
