using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour
{
    // ID of the associated vertex in the graph data structure, -1 is unintialized
    private int id = -1;
    // Label of the vertex
    private string label;
    // Reference to the rigidbody compoonent of the vertex object
    private Rigidbody2D rb;
    // Reference to the sprite child object of the vertex object
    private Transform spriteObj;

    private float lifetime;

    // Getter for id
    public int GetID()
    {
        return id;
    }

    private void Awake() {
        // Vertex objects starts non active
        gameObject.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        spriteObj = transform.GetChild(0);
    }

    // Method called by a controller class to setup properties of the vertex object
    // Updated to use Vertex struct
    public void Initiate(Vertex vertex) {
        id = vertex.id;
        label = vertex.label;

        // Activate the vertex object once it has been initiated
        gameObject.SetActive(true);

        // Initiate the label
        transform.GetChild(2).GetComponent<LabelObj>().Initiate(label);
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
        else lifetime += Time.deltaTime;
    }

    // When Cursor enters a vertex obj, increase its sprite object size by 10%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        // Check if cursor is over collider, if so, ignore panning until the mouse button is released
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex"));  //11f since camera is at z = -10
        if (hit && hit.collider.gameObject == gameObject)
        {
            spriteObj.localScale = new Vector3(1.25f, 1.25f, 1f);
        }
        //else
        //{
        //    spriteObj.localScale = new Vector3(1f, 1f, 1f);
        //}
            
    }

    private void OnMouseExit()
    {
        spriteObj.localScale = new Vector3(1f, 1f, 1f);
    }

    // TODO: CREATE A UNIVERSAL MOUSE INPUT MANAGEMENT SYSTEM
}
