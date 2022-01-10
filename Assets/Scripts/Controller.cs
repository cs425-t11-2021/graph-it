//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    // Singleton
    public static Controller singleton;

    // Prefabs for the unity vertex and edge objects
    public GameObject vertexObjPrefab;
    public GameObject edgeObjPrefab;

    // Length of each edge (manually set for now, could implment an algorithm to determine the distance from graph size/shape or whatever)
    private float edgeLength;

    // Reference to the parent object for all the vertex and edge objects in the scene hiearchy
    public Transform graphObj;

    // Mask of Collider Layers that should receive mouse input
    public LayerMask clickableLayers;

    // TODO: Move the visual settings away from controller and into its own object
    [Header("Visual Settings")]
    public bool displayVertexLabels;
    public bool snapVerticesToGrid;

    // Main graph DS
    // SET TO PUBLIC FOR TESTING PURPUSES, CHANGE LATER
    public Graph graph;

    // Timer used for tempoarily enabling graph physics
    private float physicsTimer;
    // Graph physics enabled
    private bool graphPhysicsEnabled = false;

    private void Awake() {
        // Singleton pattern setup
        if (Controller.singleton == null) {
            Controller.singleton = this;
        }
        else {
            Debug.LogError("[Controller] Singleton pattern violation");
            Destroy(this);
            return;
        }

        // Initiate graph ds
        this.graph = new Graph();
        // Manually set edge length
        this.edgeLength = 5;

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = this.clickableLayers;
    }

    private void Update() {
        // If graph physics is currently enabled and the timer isn't set to -1 (indefinite duration), decrease the timer
        if (this.graphPhysicsEnabled && this.physicsTimer != -1)
        {
            if (this.physicsTimer <= 0f)
            {
                // Turn off graph physics once timer hits 0
                SetGraphPhysics(false);
            }
            else
            {
                this.physicsTimer -= Time.deltaTime;
            }
        }
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    // TODO: Set to static to move to separate utility class
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    public void CreateGraphObjs() {
        // Array to store the child index under GraphObj of each vertex
        // Index corresponds to the child index, value corresponds to vertex id
        int[] vertexTransformPositions = new int[this.graph.vertices.Count];

        // Currently avaiable child index of GraphObj
        int childIndex = 0;
        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj
        foreach (Vertex vertex in this.graph.vertices) {
            // TODO: Change Testing code
            // Testing: Vertex objs spawns in random position
            Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized * 3f;
            if (vertex.x_pos != null && vertex.y_pos != null) {
                pos = new Vector2((float) vertex.x_pos, (float) vertex.y_pos);
            }

            // Instantiate a vertex object, set its parent to the graphObj, and store its id in the vertex transforms array, and increase the child index
            VertexObj vertexObj = Instantiate(this.vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(this.graphObj);
            vertexTransformPositions[childIndex++] = vertex.id;
            // Call the initiation function of the vertex object
            vertexObj.Initiate(vertex);

            // If snap to grid is enabled, move the new vertex obj to the nearest grid spot
            if (Grid.singleton.GridEnabled)
            {
                vertexObj.transform.position = Grid.singleton.FindClosestGridPosition(vertexObj);
            }
        }

        // Iterate through each edge in the graph data structure and create a correspoinding edgeObj
        foreach (KeyValuePair<(int, int), Edge> kvp in this.graph.adjacency)
        {
            EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
            // Find the child index of the from and to vertices and set the from vertex as the parent of edge object
            int fromVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item1);
            int toVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item2);
            edgeObj.transform.SetParent(this.graphObj.GetChild(fromVertexIndex));
            // Call the initiation function of the edge object
            edgeObj.Initiate(kvp.Value, this.graphObj.GetChild(toVertexIndex).gameObject);

            // Add a DistanceJoint2D which connects the two vertices
            DistanceJoint2D joint = this.graphObj.GetChild(fromVertexIndex).gameObject.AddComponent<DistanceJoint2D>();
            // Configure the properties of the joint
            joint.autoConfigureConnectedAnchor = false;
            joint.enableCollision = true;
            joint.distance = this.edgeLength;
            joint.maxDistanceOnly = true;
            joint.autoConfigureDistance = false;
            joint.connectedBody = this.graphObj.GetChild(toVertexIndex).gameObject.GetComponent<Rigidbody2D>();
            // Disable joint by default, the joint will only be enabled when graph physics is in use
            joint.enabled = false;
        }

        // Update the Grpah information UI
        GraphInfo.singleton.UpateGraphInfo();

        // Make sure no objects are selected when they are first created
        SelectionManager.singleton.DeSelectAll();

        // Enable graph physics for two seconds to spread out the graph
        UseGraphPhysics(2);
    }

    // Remove all graph visualization objects from scene
    // Warning: Could lead to visualizaion not matching up with the graph ds if the ds is not also cleared.
    public void ClearGraphObjs() {
        // Deselect All
        SelectionManager.singleton.DeSelectAll();

        // Destroy (or pool) all vertex objects
        for (int i = this.graphObj.childCount - 1; i >= 0; i--) {
            // TODO: Once object pooling is implmented, add deleted objs to pool rather than destroy them.
            Destroy(this.graphObj.GetChild(i).gameObject);
            this.graphObj.GetChild(i).SetParent(null);
        }

        // If snap to grid is enabled, clear out the grid
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.ClearGrid();
        }

        // Update the Grpah information UI
        GraphInfo.singleton.UpateGraphInfo();
    }

    // Method to update graph objects to match the graph ds if new vertices or edges are added
    public void UpdateGraphObjs() {
        // Get a list of all the vertex objects in scene
        VertexObj[] allVertexObjs = Controller.singleton.graphObj.GetComponentsInChildren<VertexObj>();
        // Get a list of the ids of current vertex objects
        List<int> existingVertexObjIDs = new List<int>();
        foreach (VertexObj vertexObj in allVertexObjs)
        {
            existingVertexObjIDs.Add(vertexObj.GetID());
        }
        // If a vertex is in the ds but doens't have an associated vertex object, create it
        foreach (Vertex vertex in this.graph.vertices) {
            if (!existingVertexObjIDs.Contains(vertex.id)) {
                CreateVertexObj(vertex);
            }
        }

        // Get a list of all the edge objects in scene
        EdgeObj[] allEdgeObjs = Controller.singleton.graphObj.GetComponentsInChildren<EdgeObj>();
        // Get a list of the ids of current edge objects
        List<int> existingEdgeObjIDs = new List<int>();
        foreach (EdgeObj edgeObj in allEdgeObjs)
        {
            existingEdgeObjIDs.Add(edgeObj.GetID());
        }
        // If an edge is in the ds but doens't have an associated edge object, create it
        foreach (KeyValuePair<(int, int), Edge> kvp in this.graph.adjacency) {
            if (!existingEdgeObjIDs.Contains(kvp.Value.id)) {
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
        vertexObj.transform.SetParent(this.graphObj);
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
        VertexObj[] allVertexObjs = this.graphObj.GetComponentsInChildren<VertexObj>();
        VertexObj fromVertexObj = null;
        VertexObj toVertexObj = null;

        // Find the vertex objects associated with the from and to vertices of the edge
        foreach (VertexObj vertexObj in allVertexObjs) {
            if (vertexObj.GetID() == edge.vert1.id) {
                fromVertexObj = vertexObj;
            }
            if (vertexObj.GetID() == edge.vert2.id) {
                toVertexObj = vertexObj;
            }
        }

        // Instantiate an edge object
        EdgeObj edgeObj = Instantiate(this.edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
        // Find the child index of the from and to vertices and set the from vertex as the parent of edge object, then initiate the edge object
        edgeObj.transform.SetParent(fromVertexObj.transform);
        edgeObj.Initiate(edge, toVertexObj.gameObject);

        // Add a DistanceJoint2D which connects the two vertices, setup its properties
        DistanceJoint2D joint = fromVertexObj.gameObject.AddComponent<DistanceJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.enableCollision = true;
        joint.distance = edgeLength;
        joint.maxDistanceOnly = true;
        joint.autoConfigureDistance = false;
        joint.connectedBody = toVertexObj.gameObject.GetComponent<Rigidbody2D>();
        // Disable joint by default, the joint will only be enabled when graph physics is in use
        joint.enabled = false;
    }

    // Returns true if any UI elements are being interacted with
    public bool IsUIactive()
    {
        return EventSystem.current.currentSelectedGameObject != null;
    }

    // Enables graph physics for a certain duartion
    public void UseGraphPhysics(float duration)
    {
        if (!this.graphPhysicsEnabled)
        {
            SetGraphPhysics(true);
        }
        this.physicsTimer = duration;
    }

    // Enable/disable the components associated with graph physics
    private void SetGraphPhysics(bool enabled)
    {
        // Turns off the grid if physics is turned on
        if (this.snapVerticesToGrid)
        {
            Grid.singleton.ClearGrid();
            Grid.singleton.GridEnabled = !enabled;
        }

        // Find all vertex objects
        VertexObj[] vertexObjs = GameObject.FindObjectsOfType<VertexObj>();
        foreach (VertexObj vertexObj in vertexObjs)
        {
            // Enable the joints connecting vertices when physics is on
            DistanceJoint2D[] joints = vertexObj.GetComponents<DistanceJoint2D>();
            foreach (DistanceJoint2D joint in joints)
            {
                joint.enabled = enabled;
            }
            Rigidbody2D vertexObjRB = vertexObj.GetComponent<Rigidbody2D>();
            vertexObjRB.velocity = Vector3.zero;
            // Set vertex rigidbody to kinematic when physics is off
            vertexObjRB.isKinematic = !enabled;

            if (!enabled && this.snapVerticesToGrid)
            {
                // Resnap vertices when physics turns off if snap to grid is enabled
                vertexObj.transform.position = Grid.singleton.FindClosestGridPosition(vertexObj);
            }
        }
        this.graphPhysicsEnabled = enabled;
    }
}
