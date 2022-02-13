//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GraphInstance {
    public Graph graph;
    public Transform container;
    public List<VertexObj> vertexObjs;
    public List<EdgeObj> edgeObjs;

    public GraphInstance(Transform container) {
        this.graph = new Graph();
        this.vertexObjs = new List<VertexObj>();
        this.edgeObjs = new List<EdgeObj>();

        this.container = container;
    }
}

public class Controller : SingletonBehavior<Controller>
{
    // Prefabs for the unity vertex and edge objects
    [SerializeField] private GameObject vertexObjPrefab;
    [SerializeField] private GameObject edgeObjPrefab;
    // Prefab for the graph container object
    [SerializeField] private GameObject graphObjContainerPrefab;

    private GraphInstance currentGraphInstance;

    public Transform GraphObjContainer { get => currentGraphInstance.container; }

    public List<VertexObj> VertexObjs {
        get => currentGraphInstance.vertexObjs;
    }

    public List<EdgeObj> EdgeObjs {
        get => currentGraphInstance.edgeObjs;
    }

    // Mask of Collider Layers that should receive mouse input
    [SerializeField] private LayerMask clickableLayers;

    // Main graph DS
    public Graph Graph { 
        get => currentGraphInstance.graph;
    }

    // Events called when new vertex and edge objects are created
    public event Action<VertexObj> OnVertexObjectCreation;
    public event Action<EdgeObj> OnEdgeObjectCreation;

    private void Awake() {
        // Initiate graph ds
        Transform newGraphObjContainer = Instantiate(graphObjContainerPrefab, Vector3.zero, Quaternion.identity).transform;
        this.currentGraphInstance = new GraphInstance(newGraphObjContainer);

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = this.clickableLayers;
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    public void CreateObjsFromGraph() {
        // Make sure no objects are selected when they are first created
        SelectionManager.Singleton.DeSelectAll();

        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj
        this.Graph.vertices.ForEach(vertex => CreateVertexObj(vertex));

        this.Graph.adjacency.ForEach((vertices, edge) => CreateEdgeObj(edge));

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    public void CreateGraphInstance() {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();

        // Reset toolbar toggles
        Toolbar.Singleton.ResetAll();

        // If snap to grid is enabled, clear out the grid
        if (Grid.Singleton.GridEnabled)
        {
            Grid.Singleton.ClearGrid();
        }

        Destroy(this.GraphObjContainer.gameObject);

        this.currentGraphInstance = new GraphInstance(Instantiate(graphObjContainerPrefab, Vector3.zero, Quaternion.identity).transform);

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    public void AddVertex(Vector2 pos) {
        Vertex vertex = this.Graph.AddVertex(pos.x, pos.y);
        CreateVertexObj(vertex);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new vertex object to correspond to a passed in graph vertex
    private void CreateVertexObj(Vertex vertex) {
        Vector2 pos;
        if (vertex.x_pos != null && vertex.y_pos != null) {
            pos = new Vector2( (float) vertex.x_pos, (float) vertex.y_pos);
        }
        else {
            Logger.Log("Attempting to create a vertex object with no position given.", this, LogType.ERROR);
            return;
        }

        // Instantiate a vertex object, set its parent to the graphObj, and call the initiation function
        VertexObj vertexObj = Instantiate(this.vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
        vertexObj.transform.SetParent(this.GraphObjContainer);
        vertexObj.Initiate(vertex);

        Logger.Log("Creating new vertex in the current graph instance.", this, LogType.INFO);
        this.currentGraphInstance.vertexObjs.Add(vertexObj);

        // If snap to grid is enabled, move the new vertex obj to the nearest grid spot
        if (Grid.Singleton.GridEnabled)
        {
            vertexObj.transform.position = Grid.Singleton.FindClosestGridPosition(vertexObj);
        }

        this.currentGraphInstance.vertexObjs.Add(vertexObj);
        // Send the onVertexObjCreation event
        OnVertexObjectCreation?.Invoke(vertexObj);
    }

    public void AddEdge(Vertex vertex1, Vertex vertex2) {
        // If the requested edge already exists, return
        if (Controller.Singleton.Graph.IsAdjacent(vertex1, vertex2))  {
            Logger.Log("Attempting to add edge between two vertices that are already connected.", this, LogType.WARNING);
            return;
        }

        // If both vertices are the same, return
        if (vertex1 == vertex2) {
            Logger.Log("Attempting to add edge from a vertex to itself is not currently implmented.", this, LogType.WARNING);
            return;
        }

        Edge newEdge = this.Graph.AddEdge(vertex1, vertex2);
        CreateEdgeObj(newEdge);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new edge object to correspond to a passed in graph edge
    private void CreateEdgeObj(Edge edge) {
        VertexObj fromVertexObj = GetVertexObj(edge.vert1);
        VertexObj toVertexObj = GetVertexObj(edge.vert2);

        if (!fromVertexObj || !toVertexObj) {
            Logger.Log("Attempting add an edge between vertices that do not exist in the data structure.", this, LogType.ERROR);
            return;
        }

        // Instantiate an edge object
        EdgeObj edgeObj = Instantiate(this.edgeObjPrefab, Vector2.zero, Quaternion.identity).transform.GetChild(0).GetComponent<EdgeObj>();
        // Find the child index of the from and to vertices and set the from vertex as the parent of edge object, then initiate the edge object
        edgeObj.transform.parent.SetParent(fromVertexObj.transform);
        edgeObj.Initiate(edge, fromVertexObj.gameObject, toVertexObj.gameObject);

        Logger.Log("Creating new edge in the current graph instance.", this, LogType.INFO);
        this.currentGraphInstance.edgeObjs.Add(edgeObj);

        // Send the OnEdgeObjectCreation event
        OnEdgeObjectCreation?.Invoke(edgeObj);
    }

    private VertexObj GetVertexObj(Vertex v) {
        foreach (VertexObj vertexObj in this.currentGraphInstance.vertexObjs) {
            if (vertexObj.Vertex == v) return vertexObj;
        }
        return null;
    }
}