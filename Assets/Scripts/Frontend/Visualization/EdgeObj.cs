//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObj : MonoBehaviour
{
    // Property refernce of edge associated with the edge object
    public Edge Edge { get; private set; }

    // Label of the edge
    private string label;

    // Uses a custom timer to reduce the frequency of physics updates (to reduce lag)
    private float physicsTimer = 0f;

    // Reference to the game object of the source vertex
    public GameObject FromVertexObj {get; set;}
    // Reference to the game object of the target vertex
    public GameObject ToVertexObj {get; set;}

    // TODO: Remove once animations are implemented
    // Whether edge is selected in the SelectionManager
    private bool selected = false;
    // Property for getting and setting whether or not the edge is selected, and edit the edge's color to match
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            if (value) {
                this.spriteRenderer.color = new Color32(0, 125, 255, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 125, 255, 255);
                labelObj.MakeEditable();
            }
            else {
                this.spriteRenderer.color = new Color32(0, 0, 0, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 0, 0, 255);
                labelObj.MakeUneditable();
            }
        }
    }
    // Reference to the spriteRenderer component of the object
    private SpriteRenderer spriteRenderer;

    // Width scale factor for edge thickness increse of 1
    [SerializeField] private float edgeWidthScaleFactor = 0.1f;

    // Directed edge variables
    private Transform arrow;
    private SpriteRenderer arrowSpriteRenderer;
    private int direction;
    // Edge weights/labels
    private EdgeLabelObj labelObj;

    private void Awake() {
        // Edge objects starts non active
        this.gameObject.SetActive(false);

        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.arrow = this.transform.GetChild(0);
        this.arrowSpriteRenderer = this.arrow.GetComponent<SpriteRenderer>();
        this.labelObj = this.transform.parent.GetComponentInChildren<EdgeLabelObj>();
    }

    // TODO: Modify this initialize code to not involve passing around a Unity GameObject
    public void Initiate(Edge edge, GameObject from, GameObject to) {
        this.Edge = edge;

        this.ToVertexObj = to;
        this.FromVertexObj = from;
        
        this.gameObject.SetActive(true);
        // TODO: Make this better
        // Currently, direction = 1 means pointing from parent to target vertex
        this.direction = 1;
        
        this.labelObj.Initiate(this.Edge.weight);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        this.transform.parent.localPosition = new Vector3(0, 0, 1);
        Vector2 dir = point2 - point1;
        this.transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (this.Edge.directed) {
            this.arrow.localPosition = new Vector3((this.direction == 1 ? 0.5f : 0f) - (this.direction * this.arrowSpriteRenderer.size.x / this.transform.localScale.x), 0f, 0f);
            this.arrow.localScale = new Vector3(1f / this.transform.lossyScale.x, 1f / (this.transform.lossyScale.y - this.Edge.thickness * edgeWidthScaleFactor), 1);
            this.arrow.localRotation = Quaternion.AngleAxis(this.direction == 1 ? 0f : 180f, Vector3.forward);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (this.Selected) {
            // If selected and Plus is pressed, increase thickness
            if (Input.GetKeyDown(KeyCode.Equals) && this.Edge.thickness < 5) {
                this.Edge.thickness++;
                this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
            }
            // If selected and Minus is pressed, decrease thickness
            else if (Input.GetKeyDown(KeyCode.Minus) && this.Edge.thickness > 0) {
                this.Edge.thickness--;
                this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
            }
        }

        if (Edge != null) {
            // Stretch the edge between the two vertices
            StretchBetweenPoints(this.transform.parent.position, ToVertexObj.transform.position);
        }
    }

    // Toggle between undirected, direction 1, and direction -1
    public void ToggleEdgeType() {
        if (!this.Edge.directed) {
            this.Edge.directed = true;
            this.direction = 1;
        }
        else {
            if (this.direction == 1) {
                this.Edge.Reverse();
                this.direction = -1;
            }
            else {
                this.Edge.Reverse();
                this.direction = 1;
                this.Edge.directed = false;
            }
        }
    }

    // When Cursor enters a edge obj, increase its sprite object size by 33%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, (0.25f + (this.Edge.thickness * edgeWidthScaleFactor)) * 1.33f, 1f);

    }

    private void OnMouseExit()
    {
        // When cursor exits, reset the thickness
        this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
    }

    public void UpdateWeight(double newWeight) {
        this.Edge.weight = newWeight;
    }
}
