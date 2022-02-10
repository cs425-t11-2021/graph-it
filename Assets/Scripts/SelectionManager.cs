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

    // Stored position of the cursor used to detect panning
    private Vector3 lastCursorWorldPos;

    // Event for when the selection is updated (select or deselect)
    public event Action<int, int> OnSelectionChange;

    // Whether or not to select all components
    // TODO: think of a better solution
    private bool selectAll = false;

    // [SerializeField]
    // private RectTransform selectionRect;

    private void Awake()
    {
        // Initialize data structures
        this.selectedVertices = new List<VertexObj>();
        this.selectedEdges = new List<EdgeObj>();
    }

    private void Update()
    {
        // Delete selection if backspace or delete key is pressed
        if (!UIManager.Singleton.CursorOnUI)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSelection();
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            this.lastCursorWorldPos = Controller.Singleton.GetCursorWorldPosition();
            if (ManipulationStateManager.Singleton.ActiveState == ManipulationState.selectionState) {
                // selectionRect.gameObject.SetActive(true);
            }
        }

        if (ManipulationStateManager.Singleton?.ActiveState == ManipulationState.selectionState) {
            // if (Input.GetMouseButton(0)) {
            //     UpdateSelectionRect();
            // }
            if (Input.GetMouseButtonUp(0)) {
                // Bounds bounds = UpdateSelectionRect();

                // VertexObj[] vertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
                // foreach (VertexObj v in vertexObjs) {
                //     if (bounds.Contains(v.transform.position)) {
                //         v.SetSelected(true);
                //     }
                // }
                // EdgeObj[] edgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
                // foreach (EdgeObj e in edgeObjs) {
                //     if (bounds.Contains((Vector2) e.transform.position)) {
                //         e.SetSelected(true);
                //     }
                // }

                // selectionRect.gameObject.SetActive(false);
            }
        }
        else {
            // Deselect all when the user clicks out of the graph
            if (Input.GetMouseButtonUp(0)) {
                if (this.selectAll) this.selectAll = false;
                else {
                    if (!UIManager.Singleton.CursorOnUI && !EventSystem.current.IsPointerOverGameObject()) {
                        if (Controller.Singleton.GetCursorWorldPosition() == this.lastCursorWorldPos) {
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

    // private Bounds UpdateSelectionRect() {
    //     Vector2 currentMouseWorldPos = Controller.Singleton.GetCursorWorldPosition();
    //     Vector2 middle = (Controller.Singleton.GetCursorWorldPosition() + lastCursorWorldPos) / 2f;
    //     Vector2 size = new Vector2(Mathf.Abs(currentMouseWorldPos.x - lastCursorWorldPos.x), Mathf.Abs(currentMouseWorldPos.y - lastCursorWorldPos.y));

    //     this.selectionRect.position = middle;
    //     this.selectionRect.sizeDelta = size;

    //     Bounds worldBounds = new Bounds(middle, size);
    //     return worldBounds;
    // }

    // Add a vertex to selectedVertices
    public void SelectVertex(VertexObj vertexObj)
    {
        if (!this.selectAll && ManipulationStateManager.Singleton.ActiveState != ManipulationState.selectionState && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) {
            DeSelectAll();
        }

        if (!this.selectedVertices.Contains(vertexObj)) {
            this.selectedVertices.Add(vertexObj);

            // Call selection changed event
            this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
        }
    }

    // Add an to selectedEdges
    public void SelectEdge(EdgeObj edgeObj)
    {
        if (!this.selectAll && ManipulationStateManager.Singleton.ActiveState != ManipulationState.selectionState && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) {
            DeSelectAll();
        }
        
        if (!this.selectedEdges.Contains(edgeObj)) {
            this.selectedEdges.Add(edgeObj);

            // Call selection changed event
            this.OnSelectionChange(this.selectedVertices.Count, this.selectedEdges.Count);
        }
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
            Controller.Singleton.Graph.RemoveEdge(edgeObj.Edge);
            Destroy(edgeObj.transform.parent.gameObject);
        }
        this.selectedEdges = new List<EdgeObj>();

        // For each vertex to be deleted, find all edges connecting to the vertex, then destroy the vertex object
        foreach (VertexObj vertexObj in this.selectedVertices)
        {
            // TODO: Find a faster way to do this without having to find all the edge objects in the scene
            EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
            foreach (EdgeObj edgeObj in allEdgeObjs)
            {
                if (edgeObj.ToVertexObj == vertexObj.gameObject)
                {
                    Destroy(edgeObj.gameObject);
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

        EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (!selectedEdges.Contains(edgeObj))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
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

        List<Edge> primEdges = Controller.Singleton.Graph.Prim(selectedVertices[0].Vertex);
        List<Vertex> primVertices = new List<Vertex>();
        foreach (Edge e in primEdges) {
            primVertices.Add(e.vert1);
            primVertices.Add(e.vert2);
        }

        this.selectAll = true;

        EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (primEdges.Contains(edgeObj.Edge))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            if (primVertices.Contains(vertexObj.Vertex))
                vertexObj.SetSelected(true);
        }
    }
    public void RunKruskal() {
        List<Edge> kruskalEdges = Controller.Singleton.Graph.Kruskal();
        List<Vertex> kruskalVertices = new List<Vertex>();
        foreach (Edge e in kruskalEdges) {
            kruskalVertices.Add(e.vert1);
            kruskalVertices.Add(e.vert2);
        }

        this.selectAll = true;

        EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (kruskalEdges.Contains(edgeObj.Edge))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            if (kruskalVertices.Contains(vertexObj.Vertex))
                vertexObj.SetSelected(true);
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

        this.selectAll = true;

        EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            if (dijkstraEdges.Contains(edgeObj.Edge))
                edgeObj.SetSelected(true);
        }

        VertexObj[] allVertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            if (dijkstraVertices.Contains(vertexObj.Vertex))
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
        if (Controller.Singleton.Graph.IsAdjacent(vertexObj1.Vertex, vertexObj2.Vertex))  {
            return;
        }

        // If both vertices are the same, return
        if (vertexObj1.Vertex == vertexObj2.Vertex) return;

        Controller.Singleton.Graph.AddEdge(vertexObj1.Vertex, vertexObj2.Vertex);
        Controller.Singleton.UpdateGraphObjs();

        GraphInfo.Singleton.UpdateGraphInfo();
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

    public void ChangeSelectedEdgesType() {
        foreach (EdgeObj e in selectedEdges) {
            e.ToggleEdgeType();
        }
    }
}
