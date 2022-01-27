//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    // Singleton
    public static SelectionManager singleton;

    // Lists to store the graph objects of currently selected vertices and edges
    private List<VertexObj> selectedVertices;
    private List<EdgeObj> selectedEdges;

    // Stored position of the cursor used to detect panning
    private Vector3 lastCursorPos;

    // Event for when the selection is updated (select or deselect)
    public event Action<int, int> OnSelectionChange;

    // Whether or not to select all components
    // TODO: think of a better solution
    private bool selectAll = false;

    [SerializeField]
    private RectTransform selectionRect;

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
        if (!Controller.singleton.UIActive())
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSelection();
            }

            if (Input.GetKeyDown(KeyCode.Equals) && selectedVertices.Count == 2 && selectedEdges.Count == 0)
            {
                AddEdge();
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            this.lastCursorPos = Input.mousePosition;
        }
        else if (Toolbar.singleton.SelectionMode) {
            if (Input.GetMouseButton(0)) {
                selectionRect.gameObject.SetActive(true);
                UpdateSelectionRect();
            }
            if (Input.GetMouseButtonUp(0)) {
                Bounds bounds = UpdateSelectionRect();

                VertexObj[] vertexObjs = Controller.singleton.graphObj.GetComponentsInChildren<VertexObj>();
                foreach (VertexObj v in vertexObjs) {
                    if (bounds.Contains(v.transform.position)) {
                        v.SetSelected(true);
                    }
                }
                EdgeObj[] edgeObjs = Controller.singleton.graphObj.GetComponentsInChildren<EdgeObj>();
                foreach (EdgeObj e in edgeObjs) {
                    if (bounds.Contains(e.transform.position)) {
                        e.SetSelected(true);
                    }
                }

                selectionRect.gameObject.SetActive(false);
            }
        }
        else {
            // Deselect all when the user clicks out of the graph
            if (Input.GetMouseButtonUp(0)) {
                if (this.selectAll) this.selectAll = false;
                else {
                    if (!Controller.singleton.UIActive() && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Input.mousePosition == this.lastCursorPos) {
                            // Check if cursor is over collider, if not, deselect all graph objects
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex", "Edge", "UI"));  //11f since camera is at z = -10
                            if (!hit)
                            {
                                DeSelectAll();
                            }
                        }
                    }
                }
            }
        }
    }

    private Bounds UpdateSelectionRect() {
        Vector3 middle = (Input.mousePosition + lastCursorPos) / 2f;
        Vector2 size = new Vector2(Mathf.Abs(Input.mousePosition.x - lastCursorPos.x), Mathf.Abs(Input.mousePosition.y - lastCursorPos.y));

        this.selectionRect.position = middle;
        this.selectionRect.sizeDelta = size;

        Vector3[] worldCorners = new Vector3[4];
        selectionRect.GetWorldCorners(worldCorners);

        Bounds worldBounds = new Bounds();
        worldBounds.SetMinMax(worldCorners[0], worldCorners[2]);

        return worldBounds;
    }

    // Add a vertex to selectedVertices
    public void SelectVertex(VertexObj vertexObj)
    {
        if (!this.selectAll && !Toolbar.singleton.SelectionMode && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) {
            DeSelectAll();
        }
        this.selectedVertices.Add(vertexObj);

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Add an to selectedEdges
    public void SelectEdge(EdgeObj edgeObj)
    {
        if (!this.selectAll && !Toolbar.singleton.SelectionMode && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) {
            DeSelectAll();
        }
        this.selectedEdges.Add(edgeObj);

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Remove a vertex from selectedVertices
    public bool DeselectVertex(VertexObj vertexObj)
    {
        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count - 1, this.selectedEdges.Count);

        return this.selectedVertices.Remove(vertexObj);
    }

    // Remove am edge from selectedEdges
    public bool DeselectEdge(EdgeObj edgeObj)
    {
        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count - 1);

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
            Controller.singleton.Graph.RemoveEdge(edgeObj.Edge);

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
            Controller.singleton.Graph.RemoveVertex(vertexObj.Vertex);

            Destroy(vertexObj.gameObject);
        }
        this.selectedVertices = new List<VertexObj>();

        // Update the Grpah information UI
        GraphInfo.singleton.UpdateGraphInfo();

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Method called to remove all selections
    public void DeSelectAll()
    {
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

        // Call selection changed event
        this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
    }

    // Method called to select all objects
    public void SelectAll()
    {
        this.selectAll = true;

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

        List<Edge> primEdges = Controller.singleton.Graph.Prim(selectedVertices[0].Vertex);
        List<Vertex> primVertices = new List<Vertex>();
        foreach (Edge e in primEdges) {
            primVertices.Add(e.vert1);
            primVertices.Add(e.vert2);
        }

        EdgeObj[] allEdgeObjs = Controller.singleton.graphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (primEdges.Contains(edgeObj.Edge))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.singleton.graphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            if (primVertices.Contains(vertexObj.Vertex))
                vertexObj.SetSelected(true);
        }
    }

    // Add a new edge between the first two selected vertices
    public void AddEdge() {
        VertexObj vertexObj1 = selectedVertices[0];
        VertexObj vertexObj2 = selectedVertices[1];

        AddEdge(vertexObj1, vertexObj2);
        DeSelectAll();
    }

    // Add a new edge between two vertices
    public void AddEdge(VertexObj vertexObj1, VertexObj vertexObj2) {
        // If the requested edge already exists, return
        if (Controller.singleton.Graph.IsAdjacent(vertexObj1.Vertex, vertexObj2.Vertex))  {
            return;
        }

        // If both vertices are the same, return
        if (vertexObj1.Vertex == vertexObj2.Vertex) return;

        Controller.singleton.Graph.AddEdge(vertexObj1.Vertex, vertexObj2.Vertex);
        Controller.singleton.UpdateGraphObjs();

        GraphInfo.singleton.UpdateGraphInfo();
    }

    // Add a new edge between the first vertexObj selected and a passed in vertexObj
    public void AddEdge(VertexObj target) {
        AddEdge(selectedVertices[0], target);
    }

    public void DragSelectedVerticesStart() {
        foreach (VertexObj v in this.selectedVertices) {
            v.SetCursorDragOffset();
        }
    }

    public void DragSelectedVertices() {
        foreach (VertexObj v in this.selectedVertices) {
            v.DragVertexWithMouse();
        }
    }

    public void DragSelectedVerticesEnd() {
        foreach (VertexObj v in this.selectedVertices) {
            v.FinishDragging();
        }
    }
}
