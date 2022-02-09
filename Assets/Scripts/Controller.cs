//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : SingletonBehavior<Controller>
{
    // Prefabs for the unity vertex and edge objects
    [SerializeField]
    private GameObject vertexObjPrefab;
    [SerializeField]
    private GameObject edgeObjPrefab;

    // Property of refernece to the parent object for all the vertex and edge objects in the scene hiearchy
    [SerializeField]
    private Transform graphObj;
    public Transform GraphObj {get => this.graphObj; private set => this.graphObj = value;}

    // Mask of Collider Layers that should receive mouse input
    [SerializeField]
    private LayerMask clickableLayers;

    // Main graph DS
    public Graph Graph { get; private set; }

    // Events called when new vertex and edge objects are created
    public event Action<VertexObj> OnVertexObjectCreation;
    public event Action<EdgeObj> OnEdgeObjectCreation;

    private void Awake() {
        // Initiate graph ds
        this.Graph = new Graph();

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = this.clickableLayers;
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    // TODO: Set to static to move to separate utility class
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    public void CreateGraphObjs() {
        // Array to store the child index under GraphObj of each vertex
        // Index corresponds to the child index, value corresponds to vertex reference
        Vertex[] vertexTransformPositions = new Vertex[this.Graph.vertices.Count];

        // Currently avaiable child index of GraphObj
        int childIndex = 0;
        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj
        foreach (Vertex vertex in this.Graph.vertices) {
            // TODO: Change Testing code
            // Testing: Vertex objs spawns in random position
            Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized * 3f;
            if (vertex.x_pos != null && vertex.y_pos != null) {
                pos = new Vector2((float) vertex.x_pos, (float) vertex.y_pos);
            }

            // Instantiate a vertex object, set its parent to the graphObj, and store its reference in the vertex transforms array, and increase the child index
            VertexObj vertexObj = Instantiate(this.vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(this.GraphObj);
            vertexTransformPositions[childIndex++] = vertex;
            // Call the initiation function of the vertex object
            vertexObj.Initiate(vertex);

            // If snap to grid is enabled, move the new vertex obj to the nearest grid spot
            if (Grid.singleton.GridEnabled)
            {
                vertexObj.transform.position = Grid.singleton.FindClosestGridPosition(vertexObj);
            }
        }

        // Iterate through each edge in the graph data structure and create a correspoinding edgeObj
        foreach (KeyValuePair<(Vertex, Vertex), Edge> kvp in this.Graph.adjacency)
        {
            EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
            // Find the child index of the from and to vertices and set the from vertex as the parent of edge object
            int fromVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item1);
            int toVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item2);
            edgeObj.transform.SetParent(this.GraphObj.GetChild(fromVertexIndex));
            // Call the initiation function of the edge object
            edgeObj.Initiate(kvp.Value, this.GraphObj.GetChild(fromVertexIndex).gameObject, this.GraphObj.GetChild(toVertexIndex).gameObject);
            // Send the OnEdgeObjectCreation event
            OnEdgeObjectCreation?.Invoke(edgeObj);
        }

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();

        // Make sure no objects are selected when they are first created
        SelectionManager.Singleton.DeSelectAll();

        // Enable graph physics for two seconds to spread out the graph
        // UseGraphPhysics(2);
    }

    // Remove all graph visualization objects from scene
    // Warning: Could lead to visualizaion not matching up with the graph ds if the ds is not also cleared.
    public void ClearGraphObjs() {
        // Deselect All
        SelectionManager.Singleton.DeSelectAll();

        // Destroy (or pool) all vertex objects
        for (int i = this.GraphObj.childCount - 1; i >= 0; i--) {
            // TODO: Once object pooling is implmented, add deleted objs to pool rather than destroy them.
            Destroy(this.GraphObj.GetChild(i).gameObject);
            this.GraphObj.GetChild(i).SetParent(null);
        }

        // If snap to grid is enabled, clear out the grid
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.ClearGrid();
        }

        // Update the Grpah information UI
        GraphInfo.Singleton.UpdateGraphInfo();

        // Reset toolbar toggles
        Toolbar.Singleton.ResetAll();
    }

    // Method to update graph objects to match the graph ds if new vertices or edges are added
    public void UpdateGraphObjs() {
        // Get a list of all the vertex objects in scene
        VertexObj[] allVertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
        // Get a list of the references of current vertex objects
        List<Vertex> existingVertexObjs = new List<Vertex>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            existingVertexObjs.Add(vertexObj.Vertex);
        }
        // If a vertex is in the ds but doens't have an associated vertex object, create it
        foreach (Vertex vertex in this.Graph.vertices) {
            if (!existingVertexObjs.Contains(vertex)) {
                CreateVertexObj(vertex);
            }
        }

        // Get a list of all the edge objects in scene
        EdgeObj[] allEdgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        // Get a list of the references of current edge objects
        List<Edge> existingEdgeObjs = new List<Edge>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            existingEdgeObjs.Add(edgeObj.Edge);
        }
        // If an edge is in the ds but doens't have an associated edge object, create it
        foreach (KeyValuePair<(Vertex, Vertex), Edge> kvp in this.Graph.adjacency) {
            if (!existingEdgeObjs.Contains(kvp.Value)) {
                CreateEdgeObj(kvp.Value);
            }
        }
    }

    // Create a new vertex object to correspond to a passed in graph vertex
    public void CreateVertexObj(Vertex vertex) {
        // TODO: Change Testing code
        // Testing: Vertex objs spawns in random position
        Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized * 3f;
        if (vertex.x_pos != null && vertex.y_pos != null) {
            pos = new Vector2( (float) vertex.x_pos, (float) vertex.y_pos);
        }

        // Instantiate a vertex object, set its parent to the graphObj, and call the initiation function
        VertexObj vertexObj = Instantiate(this.vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
        vertexObj.transform.SetParent(this.GraphObj);
        vertexObj.Initiate(vertex);

        // If snap to grid is enabled, move the new vertex obj to the nearest grid spot
        if (Grid.singleton.GridEnabled)
        {
            vertexObj.transform.position = Grid.singleton.FindClosestGridPosition(vertexObj);
        }
    }

    // Create a new edge object to correspond to a passed in graph edge
    public void CreateEdgeObj(Edge edge) {
        // Get a list of all the vertex objects in scene
        VertexObj[] allVertexObjs = this.GraphObj.GetComponentsInChildren<VertexObj>();
        VertexObj fromVertexObj = null;
        VertexObj toVertexObj = null;

        // Find the vertex objects associated with the from and to vertices of the edge
        foreach (VertexObj vertexObj in allVertexObjs) {
            if (vertexObj.Vertex == edge.vert1) {
                fromVertexObj = vertexObj;
            }
            if (vertexObj.Vertex == edge.vert2) {
                toVertexObj = vertexObj;
            }
        }

        // Instantiate an edge object
        EdgeObj edgeObj = Instantiate(this.edgeObjPrefab, Vector2.zero, Quaternion.identity).transform.GetChild(0).GetComponent<EdgeObj>();
        // Find the child index of the from and to vertices and set the from vertex as the parent of edge object, then initiate the edge object
        edgeObj.transform.parent.SetParent(fromVertexObj.transform);
        edgeObj.Initiate(edge, fromVertexObj.gameObject, toVertexObj.gameObject);
        // Send the OnEdgeObjectCreation event
        OnEdgeObjectCreation?.Invoke(edgeObj);
    }
    
}
