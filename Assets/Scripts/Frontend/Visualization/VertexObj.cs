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

    private void Awake() {
        // Vertex objects starts non active
        this.gameObject.SetActive(false);

        // Setup component references
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteObj = transform.GetChild(0);
        this.spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
        this.labelObj = GetComponentInChildren<VertexLabelObj>();
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

    // When Cursor enters a vertex obj, play hovering animation
    // private void OnMouseOver()
    // {
    //     // Check if cursor is over collider
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex"));  //11f since camera is at z = -10
    //     if (hit && hit.collider.gameObject == gameObject)
    //     {
    //         this.animator.SetBool("Hovering", true);
    //     }
    // }

    private void OnMouseOver() {
        if (InputManager.Singleton.CurrentHoveringVertex && InputManager.Singleton.CurrentHoveringVertex == this.gameObject) {
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
}