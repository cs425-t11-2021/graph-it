using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Main graph DS
    // SET TO PUBLIC FOR TESTING PURPUSES, CHANGE LATER
    public Graph graph;

    private void Awake() {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[Controller] Singleton pattern violation");
            Destroy(this);
            return;
        }

        // Initiate graph ds
        graph = new Graph();
        // Manually set edge length
        edgeLength = 5;

        // Set the camera's event mask to clickableLayers
        Camera.main.eventMask = clickableLayers;
    }

    private void Start() {
        CreateGraphObjs();
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    // TODO: add comments
    public void CreateGraphObjs() {
        // Array to store the child index under GraphObj of each vertex
        // Index corresponds to the child index, value corresponds to vertex id
        int[] vertexTransformPositions = new int[graph.vertices.Count];

        // Currently avaiable child index of GraphObj
        int childIndex = 0;
        // Iterate through each vertex in the graph data structure and create a corresponding vertexObj
        foreach (Vertex vertex in graph.vertices) {
            // TODO: Change Testing code
            // Testing: Vertex objs spawns in random position
            Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized * 3f;
            VertexObj vertexObj = Instantiate(vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(graphObj);
            vertexTransformPositions[childIndex++] = vertex.id;
            vertexObj.Initiate(vertex);
        }

        // Iterate through each edge in the graph data structure and create a correspoinding edgeObj
        foreach (KeyValuePair<(int, int), Edge> kvp in graph.adjacency)
        {
            EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
            // Find the child index of the from and to vertices and set the from vertex as the parent of edge object
            int fromVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item1);
            int toVertexIndex = Array.IndexOf(vertexTransformPositions, kvp.Key.Item2);
            edgeObj.transform.SetParent(graphObj.GetChild(fromVertexIndex));
            edgeObj.Initiate(kvp.Value, graphObj.GetChild(toVertexIndex).gameObject);

            // Add a DistanceJoint2D which connects the two vertices
            DistanceJoint2D joint = graphObj.GetChild(fromVertexIndex).gameObject.AddComponent<DistanceJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.enableCollision = true;
            joint.distance = edgeLength;
            joint.maxDistanceOnly = true;
            joint.autoConfigureDistance = false;
            joint.connectedBody = graphObj.GetChild(toVertexIndex).gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Remove all graph visualization objects from scene
    // Warning: Could lead to visualizaion not matching up with the graph ds if the ds is not also cleared.
    // TODO: add comments
    public void ClearGraphObjs() {
        Debug.LogWarning("[Controller] Calling ClearGraphObjs could lead to the visual not matching up with the graph data structure if the graph data structure isn't also cleared.");

        for (int i = 0; i < graphObj.childCount; i++) {
            // TODO: Once object pooling is implmented, add deleted objs to pool rather than destroy them.
            Destroy(graphObj.GetChild(i).gameObject);
        }
    }
}