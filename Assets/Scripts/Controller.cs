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
    }

    private void Start() {
        CreateGraphObjs();
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    public void CreateUnityGraphObjs() {
        foreach (var kvp in graph.incidence)
        {
            Vector2 pos = Random.insideUnitCircle * 5f;
        }
    }

    // TODO: add comments
    public void CreateGraphObjs() {
        foreach (var kvp in graph.incidence) {
            Vector2 pos = Random.insideUnitCircle.normalized * 3f;
            VertexObj vertexObj = Instantiate(vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(graphObj);
            vertexObj.Initiate(graph.vertices[kvp.Key]);
        }

        for (int i = 0; i < graph.incidence.Count; i++) {
            foreach (Edge edge in graph.incidence[i]) {
                EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
                edgeObj.transform.SetParent(graphObj.GetChild(i));
                edgeObj.Initiate(i, edge.vert2.id, graphObj.GetChild(edge.vert2.id).gameObject);
            }
        }
        for (int i = 0; i < graph.incidence.Count; i++) {
            foreach (Edge edge in graph.incidence[i]) {
                // Instantiate an edge object and set its parent to the source vertex
                // Initiate the edge object script with the correct parameters
                EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
                edgeObj.transform.SetParent(graphObj.GetChild(i));
                edgeObj.Initiate(i, edge.vert2.id, graphObj.GetChild(edge.vert2.id).gameObject);
                // Debug.Log("Creating edge between " + i + " and " + edge.incidence.Item2.id);

                // Add a DistanceJoint2D which connects the two vertices
                DistanceJoint2D joint = graphObj.GetChild(i).gameObject.AddComponent<DistanceJoint2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.enableCollision = true;
                joint.distance = edgeLength;
                joint.maxDistanceOnly = true;
                joint.autoConfigureDistance = false;
                joint.connectedBody = graphObj.GetChild(edge.vert2.id).gameObject.GetComponent<Rigidbody2D>();
            }
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
