//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct which stores a instance of graph datastructure along with its associated Unity objects
public class GraphInstance
{
    public uint id;
    public Graph graph;
    public Transform container;
    public List<VertexObj> vertexObjs;
    public List<EdgeObj> edgeObjs;
    public AlgorithmManager algorithmManager;

    public GraphInstance(Transform container, uint id, AlgorithmManager algoMan, Graph existingGraph = null)
    {
        this.id = id;
        this.graph = existingGraph ?? new Graph();
        this.vertexObjs = new List<VertexObj>();
        this.edgeObjs = new List<EdgeObj>();
        this.container = container;
        this.algorithmManager = algoMan;
    }
}

// Central class for managing the graph instance, creation and deletion of graph objects
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
    public event Action<GraphInstance> OnInstanceChanged;

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

    public void ClearCurrentInstance() {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();

        // Reset toolbar toggles
        Toolbar.Singleton.ResetAll();

        // If snap to grid is enabled, clear out the grid
        if (Grid.Singleton.GridEnabled)
        {
            Grid.Singleton.ClearGrid();
        }

        GraphInstance previousInstance = this.activeGraphInstance;

        // Destroy current graph objects and create a new instance
        CreateGraphInstance(true);
        Destroy(previousInstance.container.gameObject);
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
        GraphInstance newInstance = new GraphInstance(Instantiate(new GameObject("GraphObjContainer"), Vector3.zero, Quaternion.identity).transform, this.newInstanceID++, new AlgorithmManager(), existingGraph);
        this.instances.Add(newInstance);
        Logger.Log("Creating a new graph instance with id " + newInstance.id + ".", this, LogType.INFO);
        
        // Create a new tab associated with this instance
        TabBar.Singleton.CreateNewTab("Graph" + newInstance.id, newInstance);

        // If set as active option is enabled, set the new instance as the active instance
        if (setAsActive)
        {
            ChangeActiveInstance(newInstance, false);
        }

        return newInstance;
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
        
        Logger.Log("Changing active graph instance to " + instance.id + ".", this, LogType.INFO);
        if (this.activeGraphInstance != null)
            this.GraphObjContainer.gameObject.SetActive(false);
        this.activeGraphInstance = instance;
        this.GraphObjContainer.gameObject.SetActive(true);
        
        OnInstanceChanged?.Invoke(instance);

        if (updateGraphInfo)
        {
            GraphInfo.Singleton.InitiateAlgorithmManager(instance.algorithmManager);
        }
    }

    public void RemoveGraphInstance(GraphInstance instance)
    {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();
        // Reset toolbar toggles
        Toolbar.Singleton.ResetAll();
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
    }

    // Add a new vertex at a given position
    public void AddVertex(Vector2 pos) {
        Vertex vertex = this.Graph.AddVertex(pos.x, pos.y);
        CreateVertexObj(vertex);
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    // Create a new vertex object to correspond to a passed in graph vertex
    private void CreateVertexObj(Vertex vertex, bool invokeEvents = true) {
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

    public void RemoveVertex(VertexObj vertexObj, bool updateDS = true) {
        for (int i = this.activeGraphInstance.edgeObjs.Count - 1; i >= 0; i--)
        {
            if (this.activeGraphInstance.edgeObjs[i].Vertex1 == vertexObj || this.activeGraphInstance.edgeObjs[i].Vertex2 == vertexObj)
            {
                RemoveEdge(this.activeGraphInstance.edgeObjs[i], updateDS);
            }
        }

        // Update the graph ds
        if (updateDS)
            Controller.Singleton.Graph.Remove(vertexObj.Vertex);
        this.activeGraphInstance.vertexObjs.Remove(vertexObj);
        Destroy(vertexObj.gameObject);
        Logger.Log("Removed a vertex from the current graph instance.", this, LogType.INFO);
        
        this.OnGraphModified?.Invoke();
    }

    public void RemoveEdge(EdgeObj edgeObj, bool updateDS = true) {
        if (updateDS)
            Controller.Singleton.Graph.Remove(edgeObj.Edge);
        this.activeGraphInstance.edgeObjs.Remove(edgeObj);
        Destroy(edgeObj.transform.parent.gameObject);
        Logger.Log("Removed an edge from the current graph instance.", this, LogType.INFO);
        
        this.OnGraphModified?.Invoke();
    }

    public void RemoveEdge(Edge edge)
    {
        RemoveEdge(GetEdgeObj(edge));
    }

    public void AddEdge(Vertex vertex1, Vertex vertex2, bool directed = false) {
        // If the requested edge already exists, return
        if (!directed && Controller.Singleton.Graph.IsAdjacent(vertex1, vertex2))  {
            Logger.Log("Attempting to add undirected edge between two vertices that are already connected.", this, LogType.WARNING);
            return;
        }

        // If both vertices are the same, return
        if (vertex1 == vertex2) {
            Edge curvedEdge = this.Graph.AddEdge(vertex1, vertex2, directed);
            CreateCurvedEdgeObj(curvedEdge, int.MaxValue);
            GraphInfo.Singleton.UpdateGraphInfo();
        }
        else if (Controller.Singleton.Graph.IsAdjacent(vertex2, vertex1))
        {
            // TEMPOARY
            EdgeObj foundEdge = null;
            foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
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
            foundEdge.Curvature = 8;
            
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
    private void CreateEdgeObj(Edge edge, bool invokeEvents = true) {
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
        edgeObj.Curvature = curvature;
        Logger.Log("Creating new edge in the current graph instance.", this, LogType.INFO);
        // Add edge to the list of edges in instance
        this.activeGraphInstance.edgeObjs.Add(edgeObj);

        // Send the OnEdgeObjectCreation event
        this.OnEdgeObjectCreation?.Invoke(edgeObj);
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