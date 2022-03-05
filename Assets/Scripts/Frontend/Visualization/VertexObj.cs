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

    // Property for whether or not the vertex object is selected
    private bool selected;
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            // If the vertex object becomes selected, make its label editable
            if (value) {
                this.labelObj.MakeEditable();               
            }
            else {
                this.labelObj.MakeUneditable();
            }
            // Change the animator to show that the vertex is selected
            this.animator.SetBool("Selected", value);
        }
    }

    // Reference to the spriteRenderer component of the Sprite child object
    private SpriteRenderer spriteRenderer;
    // Reference to the animator component
    private Animator animator;
    // Reference to the labelObj attached to the vertexObj
    private VertexLabelObj labelObj;
    private CircleCollider2D collider;

    public float spriteRadius;
    
    // Store previous global position of the vertexObj
    private Vector3 previousPosition;
    public event Action OnVertexObjMove;

    private void Awake() {
        // Vertex objects starts non active
        this.gameObject.SetActive(false);

        // Setup component references
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteObj = transform.GetChild(0);
        this.spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
        this.labelObj = GetComponentInChildren<VertexLabelObj>();
        this.collider = GetComponent<CircleCollider2D>();

        this.spriteRadius = this.spriteRenderer.bounds.size.x / 2f;
    }

    private void Update()
    {
        if (this.transform.position != this.previousPosition)
        {
            this.previousPosition = this.transform.position;
            this.OnVertexObjMove?.Invoke();
        }
    }

    // Method called by a controller class to setup properties of the vertex object
    // Updated to use Vertex struct
    public void Initiate(Vertex vertex) {
        this.Vertex = vertex;

        // Activate the vertex object once it has been initiated
        this.gameObject.SetActive(true);

        // Initiate the label
        this.labelObj.Initiate(vertex.Label);

        // Update associated Vertex positions;
        this.Vertex.SetPos( new System.Numerics.Vector2( transform.position.x, transform.position.y ), false );
    }

    // Set the hover animation when the mouse is hoving over the vertex object
    private void OnMouseOver() {
        GameObject hoveringVertex = InputManager.Singleton.CurrentHoveringVertex;
        if (hoveringVertex && hoveringVertex == this.gameObject) {
            this.animator.SetBool("Hovering", true);
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
        if (Grid.Singleton.GridEnabled)
        {
            Grid.Singleton.RemoveFromOccupied(this);
        }
    }

    public void ChangeStyle()
    {
        uint spriteIndex = (uint) (this.Vertex.Style == 0 ? 1 : 0);
        this.Vertex.Style = spriteIndex;

        Sprite sprite = SettingsManager.Singleton.vertexSprites[spriteIndex];

        this.spriteRenderer.sprite = sprite;
        this.spriteRadius = this.spriteRenderer.bounds.size.x / 2f;
        this.collider.radius = this.spriteRadius;

        if (this.Vertex.Style == 1) {
            this.labelObj.CenteredLabel = true;
            this.labelObj.UpdatePosition();
        }
        else {
            this.labelObj.CenteredLabel = false;
            this.labelObj.UpdatePosition();
        }
    }
}
