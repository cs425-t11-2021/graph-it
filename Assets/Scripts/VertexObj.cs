using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour
{
    // ID of the associated vertex in the graph data structure
    private int vertex_id;

    private void Awake() {
        // Vertex objects starts non active
        gameObject.SetActive(false);
    }

    // Method called by a controller class to setup properties of the vertex object
    public void Initiate(int id) {
        vertex_id = id;

        // Activate the vertex object once it has been initiated
        gameObject.SetActive(true);
    }
}
