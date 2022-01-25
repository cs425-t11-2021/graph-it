//All code developed by Team 11

// Supress class name conflict warning
#pragma warning disable 0436

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour, IUsesDragEvents
{
    // Property reference of the vertex associated with the vertex object
    public Vertex Vertex { get; private set; }

    // // Reference to the rigidbody compoonent of the vertex object
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
    // Reference to the labelObj attached to the vertexObj
    private LabelObj labelObj;

    // // Getter for id
    // public int GetID()
    // {
    //     return id;
    // }

    private void Awake() {
        // Vertex objects starts non active
        gameObject.SetActive(false);

        // Setup component references
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteObj = transform.GetChild(0);
        spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
        labelObj = GetComponentInChildren<LabelObj>();
    }

    // Method called by a controller class to setup properties of the vertex object
    // Updated to use Vertex struct
    public void Initiate(Vertex vertex) {
        this.Vertex = vertex;

        // Activate the vertex object once it has been initiated
        gameObject.SetActive(true);

        // Initiate the label
        labelObj.Initiate(vertex.label);
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

    // Method for changing whether or not object is selected, updates the selection manager and activates the corresponding animation
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        animator.SetBool("Selected", selected);
        if (!selected)
        {
            SelectionManager.singleton.DeselectVertex(this);
            labelObj.MakeUneditable();
        }
        else
        {
            SelectionManager.singleton.SelectVertex(this);
            labelObj.MakeEditable();
        }
    }

    // Reset the size of the object when the cursor exists
    private void OnMouseExit()
    {
        animator.SetBool("Hovering", false);
    }

    // If vertex object is deleted, remove it from the grid
    private void OnDestroy()
    {
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.RemoveFromOccupied(this);
        }
    }

    // Method called when the vertex object is first picked up to be dragged
    public void OnDragStart()
    {
        // If the grid is currently enabled, remove the vertex obejct from any grid points and display the gridlines
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.RemoveFromOccupied(this);
            Grid.singleton.DisplayGridLines();
        }
    }

    // Method called after dragging is finished
    public void OnDragFinish()
    {
        // If the grid is currently enabled, move the vertex object to the nearest grid point and hide the gridlines
        if (Grid.singleton.GridEnabled)
        {
            this.transform.position = Grid.singleton.FindClosestGridPosition(this);
            Grid.singleton.HideGridLines();
        }
    }

    // When user clicks a vertex obj without dragging it, select/deselect it using selection manager
    public void OnMouseDownNonDrag()
    {
        // If add edge mode is enabled in the toolbar, add an edge instead of selecting the vertex
        if (Toolbar.singleton.AddEdgeMode) {
            SelectionManager.singleton.AddEdge(this);
            return;
        }

        SetSelected(!selected);
    }

    // TODO: CREATE A UNIVERSAL MOUSE INPUT MANAGEMENT SYSTEM
}
