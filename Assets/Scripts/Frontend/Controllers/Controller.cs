//All code developed by Team 11

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Central class for managing the graph instance, creation and deletion of graph objects. This class contains utility functions that are needed for 
// various other modules in the program such as functions to find the Unity visualization object that correspond to each component in the graph data structure.
public class Controller : SingletonBehavior<Controller>
{
    // Prefabs for the unity vertex and edge objects
    [SerializeField] private GameObject vertexObjPrefab;
    [SerializeField] private GameObject edgeObjPrefab;
    [SerializeField] public GameObject edgeTemplatePrefab;
    // Mask of Collider Layers that should receive mouse input
    [SerializeField] private LayerMask clickableLayers;
    
    // List of currently available graph instances
    private List<GraphInstance> instances = new List<GraphInstance>();
    private uint newInstanceID = 0;

    // Graph instance active in the current tab
    private GraphInstance activeGraphInstance;
    public GraphInstance ActiveGraphInstance { get => this.activeGraphInstance; }

    // Readonly property for the graph container in the current graph instance
    public Transform GraphObjContainer { get => this.activeGraphInstance.container; }

    // Readonly property for getting the list of vertex objects in the current graph instance
    public List<VertexObj> VertexObjs {
        get => this.activeGraphInstance.vertexObjs;
    }

    // Readonly property for getting the list of edge objects in the current graph instance
    public List<EdgeObj> EdgeObjs {
        get => this.activeGraphInstance.edgeObjs;
    }

    // Main graph DS in the active graph instance
    public Graph Graph { 
        get => this.activeGraphInstance.graph;
    }

    public AlgorithmManager AlgorithmManager
    {
        get => this.activeGraphInstance.algorithmManager;
    }

    // Events called when new vertex and edge objects are created
    public event Action<VertexObj> OnVertexObjectCreation;
    public event Action<EdgeObj> OnEdgeObjectCreation;
    public event Action OnGraphModified;
    public event Action<GraphInstance, GraphInstance> OnInstanceChanged;
    public event Action<GraphInstance> OnInstanceCreated;
    public event Action<GraphInstance> OnInstanceDeleted;

    private void Awake() {
        // Instantiate a new garph instance and graph object container
        CreateGraphInstance(false);

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = this.clickableLayers;
    }

    private void Start()
    {
        ChangeActiveInstance(this.instances[0]);
    }

    // Creates the vertex and edge unity objects in a graph container according to the ds in a specific instance
    public void CreateObjsFromGraph(GraphInstance instance) {
        // Make sure no objects are selected when they are first created
        SelectionManager.Singleton.DeSelectAll();

        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj, do the same for edges
        instance.graph.Vertices.ForEach(vertex => CreateVertexObj(vertex, false));
        instance.graph.Adjacency.ForEach((vertices, edge) => CreateEdgeObj(edge, false));

        // Update the Grpah information UI
        OnGraphModified?.Invoke();
        GraphInfo.Singleton.InitiateAlgorithmManager(instance.algorithmManager);
    }

    public void ReplaceGraph(Graph graph)
    {
        this.ActiveGraphInstance.graph = graph;
    }

    // Create a new graph instance, optional parameters to set the new instance as the active instance or create an 
    // instance from an existing graph ds
    public GraphInstance CreateGraphInstance(bool setAsActive = false, Graph existingGraph = null) {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();

        // If snap to grid is enabled, clear out the grid
        if (Grid.Singleton.GridEnabled)
        {
            Grid.Singleton.ClearGrid();
        }
        
        // Create a new graph instance
        GraphInstance newInstance = new GraphInstance(new GameObject("GraphObjContainer" + this.newInstanceID).transform, this.newInstanceID++, new AlgorithmManager(), existingGraph);
        this.instances.Add(newInstance);
        Logger.Log("Creating a new graph instance with id " + newInstance.id + ".", this, LogType.INFO);

        OnInstanceCreated?.Invoke(newInstance);
        
        // Create a new tab associated with this instance
        TabBar.Singleton.CreateNewTab("Graph" + newInstance.id, newInstance);

        // If set as active option is enabled, set the new instance as the active instance
        if (setAsActive)
        {
            ChangeActiveInstance(newInstance, true);
        }
        
        return newInstance;
    }

    public void CreateInstanceFromSelection()
    {
        Graph newGraph = new Graph();
        Dictionary<Vertex, Vertex> vertexCorrespondanceDict = new Dictionary<Vertex, Vertex>();
        foreach (VertexObj vertexObj in SelectionManager.Singleton.SelectedVertices)
        {
            Vertex newVertex = new Vertex(vertexObj.Vertex);
            newGraph.Add(newVertex, false);
            vertexCorrespondanceDict[vertexObj.Vertex] = newVertex;
        }

        foreach (EdgeObj edgeObj in SelectionManager.Singleton.SelectedEdges)
        {
            if (SelectionManager.Singleton.SelectedVertices.Contains(edgeObj.Vertex1) &&
                SelectionManager.Singleton.SelectedVertices.Contains(edgeObj.Vertex2))
            {
                Edge newEdge = new Edge(edgeObj.Edge);
                newEdge.vert1 = vertexCorrespondanceDict[newEdge.vert1];
                newEdge.vert2 = vertexCorrespondanceDict[newEdge.vert2];
                newGraph.Add(newEdge, false);
            }
        }
    
        GraphInstance newInstance = CreateGraphInstance(true, newGraph);
        OnGraphModified?.Invoke();
        CreateObjsFromGraph(newInstance);
    }

    public void ChangeActiveInstance(GraphInstance instance, bool updateGraphInfo = true)
    {
        // If the instances list dose not contain the instance being requested, something has gone wrong
        if (!this.instances.Contains(instance))
        {
            Logger.Log("Graph instance being requested does not exist.", this, LogType.ERROR);
            throw new SystemException("Graph instance being requested does not exist.");
            return;
        }

        GraphInstance previous = this.activeGraphInstance;
        
        Logger.Log("Changing active graph instance to " + instance.id + ".", this, LogType.INFO);
        if (this.activeGraphInstance != null)
            this.GraphObjContainer.gameObject.SetActive(false);
        this.activeGraphInstance = instance;
        this.GraphObjContainer.gameObject.SetActive(true);

        if (updateGraphInfo)
        {
            GraphInfo.Singleton.InitiateAlgorithmManager(instance.algorithmManager);
        }
        
        OnInstanceChanged?.Invoke(previous, instance);
    }

    public void RemoveGraphInstance(GraphInstance instance)
    {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();
        // Reset toolbar toggles
        Toolbar.Singleton.EnterViewMode();
        // If snap to grid is enabled, clear out the grid
        if (Grid.Singleton.GridEnabled)
        {
            Grid.Singleton.ClearGrid();
        }

        if (this.activeGraphInstance == instance)
        {
            int index = this.instances.IndexOf(this.activeGraphInstance);
            if (this.instances.Count == 1)
            {
                CreateGraphInstance(true);
            }
            else if (index == this.instances.Count - 1)
            {
                ChangeActiveInstance(this.instances[index - 1]);
            }
            else
            {
                ChangeActiveInstance(this.instances[index + 1]);
            }
        }

        this.instances.Remove(instance);
        Destroy(instance.container.gameObject);
        instance.container = null;
        instance.algorithmManager.Clear();

        OnInstanceDeleted?.Invoke(instance);
    }

    // Add a new vertex at a given position
    public void AddVertex(Vector2 pos) {
        Vertex vertex = this.Graph.AddVertex(pos.x, pos.y);
        CreateVertexObj(vertex);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    public void AddCollection(List<Vertex> vertices, List<Edge> edges) {
        this.Graph.Add(vertices, edges, true);
        
    }

    // Create a new vertex object to correspond to a passed in graph vertex
    public void CreateVertexObj(Vertex vertex, bool invokeEvents = true) {
        Vector2 pos = new Vector2( vertex.Pos.X, vertex.Pos.Y );

        // Instantiate a vertex object, set its parent to the graphObj, and call the initiation function
        VertexObj vertexObj = Instantiate(this.vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
        vertexObj.transform.SetParent(this.GraphObjContainer);
        vertexObj.Initiate(vertex);

        Logger.Log("Creating new vertex in the current graph instance.", this, LogType.INFO);
        this.activeGraphInstance.vertexObjs.Add(vertexObj);

        // If snap to grid is enabled, move the new vertex obj to the nearest grid spot
        if (Grid.Singleton.GridEnabled)
        {
            vertexObj.transform.position = Grid.Singleton.FindClosestGridPosition(vertexObj);
        }

        // Send the onVertexObjCreation event
        if (invokeEvents)
        {
            this.OnVertexObjectCreation?.Invoke(vertexObj);
            this.OnGraphModified?.Invoke();
        }
    }

    public void RemoveVertex(VertexObj vertexObj, bool updateDS = true, bool invokeEvents = true) {
        for (int i = this.activeGraphInstance.edgeObjs.Count - 1; i >= 0; i--)
        {
            if (this.activeGraphInstance.edgeObjs[i].Vertex1 == vertexObj || this.activeGraphInstance.edgeObjs[i].Vertex2 == vertexObj)
            {
                RemoveEdge(this.activeGraphInstance.edgeObjs[i], updateDS, false);
            }
        }

        // Update the graph ds
        if (updateDS)
            Singleton.Graph.Remove(vertexObj.Vertex);
        this.activeGraphInstance.vertexObjs.Remove(vertexObj);
        Destroy(vertexObj.gameObject);
        Logger.Log("Removed a vertex from the current graph instance.", this, LogType.INFO);
        
        if (invokeEvents)
            this.OnGraphModified?.Invoke();
    }

    public void RemoveEdge(EdgeObj edgeObj, bool updateDS = true, bool invokeEvents = true) {
        if (updateDS)
            Singleton.Graph.Remove(edgeObj.Edge);
        this.activeGraphInstance.edgeObjs.Remove(edgeObj);
        Destroy(edgeObj.transform.parent.gameObject);
        Logger.Log("Removed an edge from the current graph instance.", this, LogType.INFO);
        
        if (invokeEvents)
            this.OnGraphModified?.Invoke();
    }

    public void RemoveCollection(List<VertexObj> vertices, List<EdgeObj> edges, bool updateDS = true) {
        foreach (EdgeObj e in edges) {
            RemoveEdge(e, false);
        }

        foreach (VertexObj v in vertices) {
            RemoveVertex(v, false);
        }

        if (updateDS)
            Singleton.Graph.Remove(vertices.Select(v => v.Vertex).ToList(), edges.Select(e => e.Edge).ToList());
        
        this.OnGraphModified?.Invoke();
    }

    public void RemoveEdge(Edge edge)
    {
        RemoveEdge(GetEdgeObj(edge));
    }

    public void AddEdge(Vertex vertex1, Vertex vertex2, bool directed = false) {
        // If the requested edge already exists, return
        if (!directed && Singleton.Graph.IsAdjacent(vertex1, vertex2))  {
            Logger.Log("Attempting to add undirected edge between two vertices that are already connected.", this, LogType.WARNING);
            return;
        }

        // If both vertices are the same, return
        if (vertex1 == vertex2) {
            Edge curvedEdge = this.Graph.AddEdge(vertex1, vertex2, directed);
            CreateCurvedEdgeObj(curvedEdge, int.MaxValue);
            GraphInfo.Singleton.UpdateGraphInfo();
        }
        else if (Singleton.Graph.IsAdjacent(vertex2, vertex1))
        {
            // TEMPOARY
            EdgeObj foundEdge = null;
            foreach (EdgeObj edgeObj in Singleton.EdgeObjs)
            {
                if (!edgeObj.Edge.Directed && ((edgeObj.Edge.vert1 == vertex2 && edgeObj.Edge.vert2 == vertex2) || (edgeObj.Edge.vert1 == vertex1 && edgeObj.Edge.vert2 == vertex2)))
                {
                    Logger.Log("Attempting to add undirected edge between two vertices that are already connected.", this, LogType.WARNING);
                    return;
                }
                
                if (edgeObj.Edge.vert1 == vertex2 && edgeObj.Edge.vert2 == vertex1)
                {
                    if (foundEdge == null)
                    {
                        foundEdge = edgeObj;
                    }
                    else
                    {
                        Logger.Log("Attempting to add edge between two vertices that are already connected.", this, LogType.WARNING);
                        return;
                    }
                }
                else if (edgeObj.Edge.vert1 == vertex1 && edgeObj.Edge.vert2 == vertex2)
                {
                    Logger.Log("Attempting to add edge between two vertices that are already connected in the same direction.", this, LogType.WARNING);
                    return;
                }
            }

            if (!foundEdge)
            {
                Logger.Log("Mismatch between graph data structure and graph objects in the instance.", this, LogType.ERROR);
                return;
            }
            foundEdge.Edge.Curvature = 8;
            
            Edge curvedEdge = this.Graph.AddEdge(vertex1, vertex2, directed);
            CreateCurvedEdgeObj(curvedEdge, 8);

        }
        else {
            Edge newEdge = this.Graph.AddEdge(vertex1, vertex2, directed);
            CreateEdgeObj(newEdge);
        }
        
        GraphInfo.Singleton.UpdateGraphInfo();
    }

        // Create a new edge object to correspond to a passed in graph edge
    public void CreateEdgeObj(Edge edge, bool invokeEvents = true) {
        // Get the two vertex objects associated with the edge
        VertexObj fromVertexObj = GetVertexObj(edge.vert1);
        VertexObj toVertexObj = GetVertexObj(edge.vert2);

        if (!edge.Directed)
        {
            foreach (EdgeObj e in this.EdgeObjs)
            {
                if (e.Vertex1 == toVertexObj && e.Vertex2 == fromVertexObj)
                {
                    return;
                }

                if (e.Vertex2 == toVertexObj && e.Vertex1 == fromVertexObj)
                {
                    return;
                }
            }
        }

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
        this.activeGraphInstance.edgeObjs.Add(edgeObj);

        // Send the OnEdgeObjectCreation event
        if (invokeEvents)
        {
            this.OnEdgeObjectCreation?.Invoke(edgeObj);
            this.OnGraphModified?.Invoke();
        }
    }

    private void CreateCurvedEdgeObj(Edge curvedEdge, int curvature = 0) {
        VertexObj vertex1 = GetVertexObj(curvedEdge.vert1);
        VertexObj vertex2 = curvedEdge.vert2 == curvedEdge.vert1 ? vertex1 : GetVertexObj(curvedEdge.vert2);

        // Instantiate an edge object
        EdgeObj edgeObj = Instantiate(this.edgeObjPrefab, Vector2.zero, Quaternion.identity).transform.GetChild(0).GetComponent<EdgeObj>();
        // Find the child index of the from and to vertices and set the from vertex as the parent of edge object, then initiate the edge object
        edgeObj.transform.parent.SetParent(this.GraphObjContainer);
        edgeObj.Initiate(curvedEdge, vertex1, vertex2);
        edgeObj.Edge.Curvature = curvature;
        Logger.Log("Creating new edge in the current graph instance.", this, LogType.INFO);
        // Add edge to the list of edges in instance
        this.activeGraphInstance.edgeObjs.Add(edgeObj);

        // Send the OnEdgeObjectCreation event
        this.OnEdgeObjectCreation?.Invoke(edgeObj);
        this.OnGraphModified?.Invoke();
    }
    
    // TODO: Find a better solution
    public void ForceInvokeModificationEvent()
    {
        this.OnGraphModified?.Invoke();
    }

    // Get a vertex object in the current graph instance given a vertex
    public VertexObj GetVertexObj(Vertex v) {
        foreach (VertexObj vertexObj in this.activeGraphInstance.vertexObjs) {
            if (vertexObj.Vertex == v) return vertexObj;
        }
        return null;
    }
    
    // Get a edge object in the current graph instance given a edge
    public EdgeObj GetEdgeObj(Edge e)
    {
        foreach (EdgeObj edgeObj in this.activeGraphInstance.edgeObjs)
        {
            if (edgeObj.Edge == e) return edgeObj;
        }
        return null;
    }
}