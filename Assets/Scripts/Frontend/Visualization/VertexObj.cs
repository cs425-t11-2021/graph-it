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
    
    private Color normalColor = Color.black;
    private Color selectedColor = new Color32(0, 125, 255, 255);

    private int resultStatus = 0;
    private float resultTimer = 0f;

    // Property for whether or not the vertex object is selected
    private bool selected;
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            // If the vertex object becomes selected, make its label editable
            if (value) {
                this.labelObj.MakeEditable();
                this.spriteRenderer.color = selectedColor;
            }
            else {
                this.labelObj.MakeUneditable();
                this.spriteRenderer.color = normalColor;
            }
            // Change the animator to show that the vertex is selected
            // this.animator.SetBool("Selected", value);
        }
    }

    public int AlgorithmResultLevel
    {
        set
        {
            this.resultStatus = value;
            if (this.resultStatus == 0)
            {
                this.spriteRenderer.color = this.normalColor;
            }
            else if (this.resultStatus == 1)
            {
                this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                this.resultTimer = 0f;
            }
        }
    }

    // Reference to the spriteRenderer component of the Sprite child object
    private SpriteRenderer spriteRenderer;
    // Reference to the animator component
    private Animator animator;
    // Reference to the labelObj attached to the vertexObj
    public VertexLabelObj labelObj;
    private Collider2D collider;

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
        AddColliderBasedOnSprite(false);

        this.spriteRadius = this.spriteRenderer.bounds.size.x / 2f;
    }

    private void Update()
    {
        if (this.transform.position != this.previousPosition)
        {
            this.previousPosition = this.transform.position;
            this.OnVertexObjMove?.Invoke();
        }

        if (resultStatus == 1)
        {
            if (resultTimer <= 0f)
            {
                resultTimer = 0.33f;
                if (this.spriteRenderer.color == normalColor)
                {
                    this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                }
                else
                {
                    this.spriteRenderer.color = this.normalColor;
                }
            }
            resultTimer -= Time.deltaTime;
        }
        else if (resultStatus == 2)
        {
            this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[1];
        }
        else if (resultStatus == 3)
        {
            this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[2];
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
        
        if (this.Vertex.Style == ResourceManager.Singleton.vertexSprites.Length - 1) {
            SetStyle(0);
        }
        else {
            SetStyle(this.Vertex.Style + 1);
        }
    }

    public void SetStyle(uint style, bool updateDS = true) {
        if (updateDS)
            this.Vertex.Style = style;

        Sprite sprite = ResourceManager.Singleton.vertexSprites[style];
        this.spriteRenderer.sprite = sprite;
        this.spriteRadius = this.spriteRenderer.bounds.size.x / 2f;
        Destroy(this.collider);

        if (style == 1) {
            this.labelObj.CenteredLabel = true;
            AddColliderBasedOnSprite(true);
            this.labelObj.UpdatePosition();
        }
        else {
            this.labelObj.CenteredLabel = false;
            AddColliderBasedOnSprite(false);
            this.labelObj.UpdatePosition();
        }
        
        this.normalColor = style < 2 ? Color.black  : Color.white;
    }

    private void AddColliderBasedOnSprite(bool poly)
    {
        if (poly)
        {
            PolygonCollider2D newCol = this.spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
            this.collider = this.gameObject.AddComponent<PolygonCollider2D>().CopyFromCollider(newCol);
            Destroy(newCol);
        }
        else
        {
            CircleCollider2D newCol = this.spriteRenderer.gameObject.AddComponent<CircleCollider2D>();
            this.collider = this.gameObject.AddComponent<CircleCollider2D>().CopyFromCollider(newCol);
            Destroy(newCol);
        }
    }
}
