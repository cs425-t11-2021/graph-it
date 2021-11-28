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
    public void CreateUnityGraphObjs() {
        foreach (var kvp in graph.adj) {
            Vector2 pos = Random.insideUnitCircle * 5f;
            VertexObj vertexObj = Instantiate(vertexObjPrefab, pos, Quaternion.identity).GetComponent<VertexObj>();
            vertexObj.transform.SetParent(graphObj);
            vertexObj.Initiate(kvp.Key);
        }

        for (int i = 0; i < graph.adj.Count; i++) {
            foreach (int connected_v in graph.adj[i]) {
                EdgeObj edgeObj = Instantiate(edgeObjPrefab, Vector2.zero, Quaternion.identity).GetComponent<EdgeObj>();
                edgeObj.transform.SetParent(graphObj.GetChild(i));
                edgeObj.Initiate(i, connected_v, graphObj.GetChild(connected_v).gameObject);
            }
        }
    }    
}
