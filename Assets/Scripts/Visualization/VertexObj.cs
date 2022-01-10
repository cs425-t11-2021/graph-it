//All code developed by Team 11

// Supress class name conflict warning
#pragma warning disable 0436

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour, IUsesDragEvents
{
    // ID of the associated vertex in the graph data structure, -1 is unintialized
    private int id = -1;
    // Label of the vertex
    private string label;
    // Reference to the rigidbody compoonent of the vertex object
    private Rigidbody2D rb;
    // Reference to the sprite child object of the vertex object
    private Transform spriteObj;

    // TODO: Remove once animations are implemented
    // Whether vertex is selected in the SelectionManager
    private bool selected = false;
    // Reference to the spriteRenderer component of the Sprite child object
    private SpriteRenderer spriteRenderer;
    // Reference to the animator component
    private Animator animator;

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
        // At the start of the program, if the vertex has no connected edges, give it 
        // extra mass and drag to avoid being pushed away by the other vertices
        if (transform.childCount == 0) {
            rb.mass = 5 * rb.mass;
            rb.drag = 5 * rb.drag;
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

    
    private void OnMouseDown()
    {
        if (Grid.singleton.enableGrid)
        {
            Grid.singleton.RemoveFromOccupied(this);
        }
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

    public void OnDragStart()
    {
        if (Grid.singleton.enableGrid)
        {
            Grid.singleton.DisplayGridLines();
        }
    }

    public void OnDragFinish()
    {
        if (Grid.singleton.enableGrid)
        {
            Grid.singleton.HideGridLines();
        }
    }

    // When user clicks a vertex obj without dragging it, select/deselect it using selection manager
    public void OnMouseDownNonDrag()
    {
        SetSelected(!selected);
    }

    // TODO: CREATE A UNIVERSAL MOUSE INPUT MANAGEMENT SYSTEM
}
