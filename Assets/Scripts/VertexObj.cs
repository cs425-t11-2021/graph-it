using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour
{
    // ID of the associated vertex in the graph data structure
    private int vertex_id;
    // Reference to the rigidbody compoonent of the vertex object
    private Rigidbody2D rb;


    private float lifetime;

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
            rb.mass = 5 * rb.mass;
            rb.drag = 5 * rb.drag;
        }

        // At the creation of the vertex object, set its lifetime to 0
        // Turn off all its edges and their corresponding distance joints
        lifetime = 0f;
        DistanceJoint2D[] joints = GetComponents<DistanceJoint2D>();
        foreach (DistanceJoint2D joint in joints) {
            joint.enabled = false;
        }
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Update() {
        lifetime += Time.deltaTime;
        
        // Once a vertex object has existed for more than half a second, turn on its edges and distance joints
        // This is to allow time for the vertices' mutual repulsive force to more evenly spread themselves out before they are connected.
        if (lifetime > 0.5f) {
            DistanceJoint2D[] joints = GetComponents<DistanceJoint2D>();
            foreach (DistanceJoint2D joint in joints) {
                joint.enabled = true;
            }
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
