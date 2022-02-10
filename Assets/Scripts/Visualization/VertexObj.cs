//All code developed by Team 11

// Supress class name conflict warning
#pragma warning disable 0436

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObj : MonoBehaviour
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

    // The current distance between the curosr to the center of the object
    private Vector3? cursorOffset;
    private bool clicked = false;
    private float dragDuration;

    private void Awake() {
        // Vertex objects starts non active
        this.gameObject.SetActive(false);

        // Setup component references
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteObj = transform.GetChild(0);
        this.spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
        this.labelObj = GetComponentInChildren<LabelObj>();

        this.cursorOffset = null;
    }

    // Method called by a controller class to setup properties of the vertex object
    // Updated to use Vertex struct
    public void Initiate(Vertex vertex) {
        this.Vertex = vertex;

        // Activate the vertex object once it has been initiated
        this.gameObject.SetActive(true);

        // Initiate the label
        this.labelObj.Initiate(vertex.label);

        // Update associated Vertex positions;
        this.Vertex.x_pos = transform.position.x;
        this.Vertex.y_pos = transform.position.y;
    }

    private void Start() {
        // At the start of the program, if the vertex has no connected edges, give it 
        // extra mass and drag to avoid being pushed away by the other vertices
        if (transform.childCount == 0) {
            this.rb.mass = 5 * this.rb.mass;
            this.rb.drag = 5 * this.rb.drag;
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
            this.animator.SetBool("Hovering", true);
        }
    }

    // Method for changing whether or not object is selected, updates the selection manager and activates the corresponding animation
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        this.animator.SetBool("Selected", selected);
        if (!selected)
        {
            SelectionManager.Singleton.DeselectVertex(this);
            this.labelObj.MakeUneditable();
        }
        else
        {
            SelectionManager.Singleton.SelectVertex(this);
            this.labelObj.MakeEditable();
        }
    }

    // Reset the size of the object when the cursor exists
    private void OnMouseExit()
    {
        this.animator.SetBool("Hovering", false);
    }

    // If vertex object is deleted, remove it from the grid
    private void OnDestroy()
    {
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.RemoveFromOccupied(this);
        }
    }

    private void OnMouseDown() {
         // When a click is detected, set clicked to true and store the current mouse position, and reset the drag duration timer
        this.dragDuration = 0f;
        this.clicked = true;
    }

    public void SetCursorDragOffset() {
        this.cursorOffset = this.transform.position - Controller.Singleton.GetCursorWorldPosition();

        // If the grid is currently enabled, remove the vertex obejct from any grid points and display the gridlines
        if (Grid.singleton.GridEnabled)
        {
            Grid.singleton.RemoveFromOccupied(this);
            Grid.singleton.DisplayGridLines();
        }
    }

    // Method called when the vertex object is first picked up to be dragged
    private void OnDragStart()
    {
        if (!Toolbar.Singleton.SelectionMode && !Toolbar.Singleton.CreateVertexMode) {
            if (this.selected) {
                SelectionManager.Singleton.DragSelectedVerticesStart();
            }
            else {
                SetCursorDragOffset();
            }
        }
    }

    private void OnMouseDrag() {
        // Increase the drag duration timer while the object is being dragged
        if (clicked)
        {
            this.dragDuration += Time.deltaTime;

            // If the object is held for more than 1/10 of a second, count as the start of a drag
            if (this.dragDuration > .1f)
            {
                this.clicked = false;
                OnDragStart();
            }
        }

        if (this.cursorOffset != null) {
            // Disable dragging when in selection or vertex creation mode
            if (!Toolbar.Singleton.SelectionMode && !Toolbar.Singleton.CreateVertexMode) {
                if (this.selected) {
                    SelectionManager.Singleton.DragSelectedVertices();
                }
                else {
                    DragVertexWithMouse();
                }
            } 
        }
    }

    private void OnMouseUp() {
        this.clicked = false;
        // If the object has been dragged for less than 1/10 of a second, count as a non-dragging click
        if (this.dragDuration < .1f)
        {
            OnMouseDownNonDrag();
        }
        else
        {
            OnDragFinish();
        }
        this.dragDuration = 0f;
    }

    public void DragVertexWithMouse() {
        if (this.cursorOffset != null)
            this.transform.position = Controller.Singleton.GetCursorWorldPosition() + (Vector3) this.cursorOffset;
    }

    // Method called after dragging is finished
    private void OnDragFinish()
    {
        if (this.selected) {
            SelectionManager.Singleton.DragSelectedVerticesEnd();
        }
        else {
            FinishDragging();
        }
    }

    public void FinishDragging() {
        // If the grid is currently enabled, move the vertex object to the nearest grid point and hide the gridlines
        if (Grid.singleton.GridEnabled)
        {
            this.transform.position = Grid.singleton.FindClosestGridPosition(this);
            Grid.singleton.HideGridLines();
        }

        this.cursorOffset = null;

        // Update associated Vertex positions;
        this.Vertex.x_pos = transform.position.x;
        this.Vertex.y_pos = transform.position.y;
    }

    // When user clicks a vertex obj without dragging it, select/deselect it using selection manager
    private void OnMouseDownNonDrag()
    {
        // If add edge mode is enabled in the toolbar, add an edge instead of selecting the vertex
        if (Toolbar.Singleton.EdgeCreationMode) {
            SelectionManager.Singleton.AddEdge(this);
            return;
        }

        SetSelected(!selected);
    }

    // TODO: CREATE A UNIVERSAL MOUSE INPUT MANAGEMENT SYSTEM
}
