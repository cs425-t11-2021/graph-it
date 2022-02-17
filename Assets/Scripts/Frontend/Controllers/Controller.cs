//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct which stores a instance of graph datastructure along with its associated Unity objects
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

// Central class for managing the graph instance, creation and deletion of graph objects
public class Controller : SingletonBehavior<Controller>
{
    // Prefabs for the unity vertex and edge objects
    [SerializeField] private GameObject vertexObjPrefab;
    [SerializeField] private GameObject edgeObjPrefab;
    [SerializeField] public GameObject edgeTemplatePrefab;
    // Prefab for the graph container object
    [SerializeField] private GameObject graphObjContainerPrefab;
    // Mask of Collider Layers that should receive mouse input
    [SerializeField] private LayerMask clickableLayers;

    // Graph instance active in the current tab
    private GraphInstance currentGraphInstance;

    // Readonly property for the graph container in the current graph instance
    public Transform GraphObjContainer { get => currentGraphInstance.container; }

    // Readonly property for getting the list of vertex objects in the current graph instance
    public List<VertexObj> VertexObjs {
        get => currentGraphInstance.vertexObjs;
    }

    // Readonly property for getting the list of edge objects in the current graph instance
    public List<EdgeObj> EdgeObjs {
        get => currentGraphInstance.edgeObjs;
    }

    // Main graph DS in the active graph instance
    public Graph Graph { 
        get => currentGraphInstance.graph;
    }

    // Events called when new vertex and edge objects are created
    public event Action<VertexObj> OnVertexObjectCreation;
    public event Action<EdgeObj> OnEdgeObjectCreation;

    private void Awake() {
        // Instantiate a new garph instance and graph object container
        this.currentGraphInstance = new GraphInstance(Instantiate(graphObjContainerPrefab, Vector3.zero, Quaternion.identity).transform);

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = this.clickableLayers;
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    public void CreateObjsFromGraph() {
        // Make sure no objects are selected when they are first created
        SelectionManager.Singleton.DeSelectAll();

        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj, do the same for edges
        this.Graph.Vertices.ForEach(vertex => CreateVertexObj(vertex));
        this.Graph.Adjacency.ForEach((vertices, edge) => CreateEdgeObj(edge));

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new graph instance and removing the current one
    // TODO: Add to a list instead of deleting the old one once multi-graph support is setup
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

        // Destroy current graph objects and create a new instance
        Destroy(this.GraphObjContainer.gameObject);
        this.currentGraphInstance = new GraphInstance(Instantiate(graphObjContainerPrefab, Vector3.zero, Quaternion.identity).transform);

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Add a new vertex at a given position
    public void AddVertex(Vector2 pos) {
        Vertex vertex = this.Graph.AddVertex(pos.x, pos.y);
        CreateVertexObj(vertex);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new vertex object to correspond to a passed in graph vertex
    private void CreateVertexObj(Vertex vertex) {
        Vector2 pos;
        if (vertex.X != null && vertex.Y != null) {
            pos = new Vector2( (float) vertex.X, (float) vertex.Y);
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
        // Add vertex to the list of vertices in the graph instance
        this.currentGraphInstance.vertexObjs.Add(vertexObj);
        // Send the onVertexObjCreation event
        this.OnVertexObjectCreation?.Invoke(vertexObj);
    }

    public void RemoveVertex(VertexObj vertexObj) {
        for (int i = this.currentGraphInstance.edgeObjs.Count - 1; i >= 0; i--)
        {
            if (this.currentGraphInstance.edgeObjs[i].Vertex1 == vertexObj || this.currentGraphInstance.edgeObjs[i].Vertex2 == vertexObj)
            {
                RemoveEdge(this.currentGraphInstance.edgeObjs[i]);
            }
        }

        // Update the graph ds
        Controller.Singleton.Graph.RemoveVertex(vertexObj.Vertex);
        this.currentGraphInstance.vertexObjs.Remove(vertexObj);
        Destroy(vertexObj.gameObject);
        Logger.Log("Removed a vertex from the current graph instance.", this, LogType.INFO);
    }

    public void RemoveEdge(EdgeObj edgeObj) {
        Controller.Singleton.Graph.RemoveEdge(edgeObj.Edge);
        this.currentGraphInstance.edgeObjs.Remove(edgeObj);
        Destroy(edgeObj.transform.parent.gameObject);
        Logger.Log("Removed an edge from the current graph instance.", this, LogType.INFO);
    }

    public void AddEdge(Vertex vertex1, Vertex vertex2, bool directed = false) {
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

        Edge newEdge = this.Graph.AddEdge(vertex1, vertex2, directed);
        CreateEdgeObj(newEdge);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new edge object to correspond to a passed in graph edge
    private void CreateEdgeObj(Edge edge) {
        // Get the two vertex objects associated with the edge
        VertexObj fromVertexObj = GetVertexObj(edge.vert1);
        VertexObj toVertexObj = GetVertexObj(edge.vert2);

        if (!fromVertexObj || !toVertexObj) {
            Logger.Log("Attempting add an edge between vertices that do not exist in the data structure.", this, LogType.ERROR);
            return;
        }

        // Instantiate an edge object
        EdgeObj edgeObj = Instantiate(this.edgeObjPrefab, Vector2.zero, Quaternion.identity).transform.GetChild(0).GetComponent<EdgeObj>();
        // Find the child index of the from and to vertices and set the from vertex as the parent of edge object, then initiate the edge object
        edgeObj.transform.parent.SetParent(this.GraphObjContainer);
        edgeObj.Initiate(edge, fromVertexObj, toVertexObj);
        Logger.Log("Creating new edge in the current graph instance.", this, LogType.INFO);
        // Add edge to the list of edges in instance
        this.currentGraphInstance.edgeObjs.Add(edgeObj);

        // Send the OnEdgeObjectCreation event
        this.OnEdgeObjectCreation?.Invoke(edgeObj);
    }

    // Get a vertex object in the current graph instance given a vertex
    private VertexObj GetVertexObj(Vertex v) {
        foreach (VertexObj vertexObj in this.currentGraphInstance.vertexObjs) {
            if (vertexObj.Vertex == v) return vertexObj;
        }
        return null;
    }
}