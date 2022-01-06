//All code developed by Team 11
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
    // Amount of time this object has been in scene
    private float lifetime;

    // TODO: Remove once animations are implemented
    // Whether vertex is selected in the SelectionManager
    private bool selected = false;
    // Reference to the spriteRenderer component of the Sprite child object
    private SpriteRenderer spriteRenderer;
    // Reference to the animator component
    private Animator animator;

    // Keep track of whether or not the object was dragged
    private Vector3 positionBeforeDrag;

    // Getter for id
    public int GetID()
    {
        return id;
    }

    private void Awake() {
        // Vertex objects starts non active
        gameObject.SetActive(false);

        // Setup component references
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        spriteObj = transform.GetChild(0);
        spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
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
        this.rb.isKinematic = false;
        this.rb.WakeUp();

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
            lifetime = Mathf.NegativeInfinity;

            if (Grid.singleton.enableGrid)
            {
                this.transform.position = Grid.singleton.FindClosestGridPosition(this);
            }
        }
        else lifetime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (lifetime > -1) return;

        // Disable the joints and set the rigidbody to kinematic when graph physics is disabled
        // TODO: Replace with more efficient code
        if (Controller.singleton.enableGraphPhysics)
        {
            DistanceJoint2D[] joints = GetComponents<DistanceJoint2D>();
            foreach (DistanceJoint2D joint in joints)
            {
                joint.enabled = true;
            }
            this.rb.isKinematic = false;
            this.rb.WakeUp();
        }
        else
        {
            DistanceJoint2D[] joints = GetComponents<DistanceJoint2D>();
            foreach (DistanceJoint2D joint in joints)
            {
                joint.enabled = false;
            }
            this.rb.isKinematic = true;
            this.rb.velocity = Vector2.zero;
        }
    }

    // When Cursor enters a vertex obj, play hovering animation
    private void OnMouseOver()
    {
        // Check if cursor is over collider
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex"));  //11f since camera is at z = -10
        if (hit && hit.collider.gameObject == gameObject)
        {
            animator.SetBool("Hovering", true);
        }
    }

    // When user clicks a vertex obj, select/deselect it using selection manager
    // Change color to blue when selected
    // TODO: Replace with Unity animator
    private void OnMouseDown()
    {
        SetSelected(!selected);

        if (Grid.singleton.enableGrid)
        {
            Grid.singleton.RemoveFromOccupied(this);
        }

        positionBeforeDrag = transform.position;
    }

    // Method for changing whether or not object is selected, updates the selection manager and activates the corresponding animation
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        animator.SetBool("Selected", selected);
        if (!selected)
        {
            SelectionManager.singleton.DeselectVertex(this);
        }
        else
        {
            SelectionManager.singleton.SelectVertex(this);
        }
    }

    // Reset the size of the object when the cursor exists
    private void OnMouseExit()
    {
        animator.SetBool("Hovering", false);
    }

    // If snap to grid is enabled, find the cloest grid position when mouse up
    private void OnMouseUp()
    {
        if (transform.position != positionBeforeDrag)
        {
            SetSelected(false);
        }

        if (Grid.singleton.enableGrid)
        {
            this.transform.position = Grid.singleton.FindClosestGridPosition(this);
        }
    }

    // If vertex object is deleted, remove it from the grid
    private void OnDestroy()
    {
        if (Grid.singleton.enableGrid)
        {
            Grid.singleton.RemoveFromOccupied(this);
        }
    }

    // TODO: CREATE A UNIVERSAL MOUSE INPUT MANAGEMENT SYSTEM
}
