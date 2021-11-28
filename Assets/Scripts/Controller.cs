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
    public float edgeLength = 1f;

    // Reference to the parent object for all the vertex and edge objects in the scene hiearchy
    public Transform graphObj;

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
    }

    private void Start() {
        CreateUnityGraphObjs();
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    // Creates the vertex and edge unity objects according to the contents of the graph ds
    // TODO: add comments
    public void CreateUnityGraphObjs() {
        foreach (var kvp in graph.adj) {
            Vector2 pos = Random.insideUnitCircle * 5f;
            VertexObj vertexObj = Instantiate(vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(graphObj);
            vertexObj.Initiate(kvp.Key);
        }

        for (int i = 0; i < graph.adj.Count; i++) {
            foreach (Edge edge in graph.adj[i]) {
                // Instantiate an edge object and set its parent to the source vertex
                // Initiate the edge object script with the correct parameters
                EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
                edgeObj.transform.SetParent(graphObj.GetChild(i));
                edgeObj.Initiate(i, edge.incidence.Item2.id, graphObj.GetChild(edge.incidence.Item2.id).gameObject);
                Debug.Log("Creating edge between " + i + " and " + edge.incidence.Item2.id);

                // Add a DistanceJoint2D which connects the two vertices
                DistanceJoint2D joint = graphObj.GetChild(i).gameObject.AddComponent<DistanceJoint2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.enableCollision = false;
                joint.distance = edgeLength;
                joint.autoConfigureDistance = false;
                joint.connectedBody = graphObj.GetChild(edge.incidence.Item2.id).gameObject.GetComponent<Rigidbody2D>();
            }
        }
    }   
}
