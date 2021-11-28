using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour
{
    // ID of the associated vertex in the graph data structure
    private int vertex_id;
    // Reference to the rigidbody compoonent of the vertex object
    private Rigidbody2D rb;

    private void Awake() {
        // Vertex objects starts non active
        gameObject.SetActive(false);

        rb = GetComponent<Rigidbody2D>(); 
    }

    // Method called by a controller class to setup properties of the vertex object
    public void Initiate(int id) {
        vertex_id = id;

        // Activate the vertex object once it has been initiated
        gameObject.SetActive(true);
    }

    private void Start() {
        // At the start of the program, if the vertex has no connected edges, give it 
        // extra mass and drag to avoid being pushed away by the other vertices
        if (transform.childCount == 0) {
            rb.mass = 5;
            rb.drag = 5;
        }
    }
}
