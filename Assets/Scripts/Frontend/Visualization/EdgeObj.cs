//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EdgeObj : MonoBehaviour
{
    // Property refernce of edge associated with the edge object
    public Edge Edge { get; private set; }

    // Label of the edge
    private string label;

    // Uses a custom timer to reduce the frequency of physics updates (to reduce lag)
    private float physicsTimer = 0f;

    public VertexObj Vertex1 {get; private set;}
    public VertexObj Vertex2 {get; private set;}

    private bool curved;
    private SpriteShapeController shapeController;
    private SpriteShapeRenderer shapeRenderer;

    // TODO: Remove once animations are implemented
    // Whether edge is selected in the SelectionManager
    private bool selected = false;

    
    // Property for getting and setting whether or not the edge is selected, and edit the edge's color to match
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            if (value) {
                if (!this.curved)
                    this.spriteRenderer.color = new Color32(0, 125, 255, 255);
                else
                    this.shapeRenderer.color = new Color32(0, 125, 255, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 125, 255, 255);
                labelObj.MakeEditable();
            }
            else {
                if (!this.curved)
                    this.spriteRenderer.color = new Color32(0, 0, 0, 255);
                else
                    this.shapeRenderer.color = new Color32(0, 0, 0, 255);
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
    [SerializeField] private Transform arrow;
    private SpriteRenderer arrowSpriteRenderer;
    // private int direction;
    // Edge weights/labels
    [SerializeField] private EdgeLabelObj labelObj;

    private void Awake() {
        // Edge objects starts non active
        this.gameObject.SetActive(false);

        this.spriteRenderer = GetComponent<SpriteRenderer>();
        // this.arrow = this.transform.GetChild(0);
        this.arrowSpriteRenderer = this.arrow.GetComponent<SpriteRenderer>();
        
    }

    // TODO: Modify this initialize code to not involve passing around a Unity GameObject
    public void Initiate(Edge edge, VertexObj vertex1, VertexObj vertex2) {
        this.Edge = edge;

        this.Vertex1 = vertex1;
        this.Vertex2 = vertex2;
        
        this.gameObject.SetActive(true);
        // TODO: Make this better
        // Currently, direction = 1 means pointing from parent to target vertex
        // this.direction = 1;

        this.curved = edge.vert1 == edge.vert2;

        if (this.curved) {
            this.shapeController = GetComponent<SpriteShapeController>();
            this.shapeRenderer = GetComponent<SpriteShapeRenderer>();
        }
        
        this.labelObj.Initiate(this);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        this.transform.parent.position = point1;
        Vector2 dir = point2 - point1;
        this.transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (this.Edge.Directed) {
            this.arrow.localPosition = new Vector3((this.Edge.Directed ? 0.5f : 0f) - (this.arrowSpriteRenderer.size.x / this.transform.localScale.x), 0f, 0f);
            this.arrow.localScale = new Vector3(1f / this.transform.lossyScale.x, 1f / (this.transform.lossyScale.y - this.Edge.Thickness * edgeWidthScaleFactor), 1);
            this.arrow.localRotation = Quaternion.AngleAxis(this.Edge.Directed ? 0f : 180f, Vector3.forward);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (this.Selected) {
            // If selected and Plus is pressed, increase thickness
            if (Input.GetKeyDown(KeyCode.Equals) && this.Edge.Thickness < 5) {
                this.Edge.Thickness++;
                this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.Thickness * edgeWidthScaleFactor), 1f);
            }
            // If selected and Minus is pressed, decrease thickness
            else if (Input.GetKeyDown(KeyCode.Minus) && this.Edge.Thickness > 0) {
                this.Edge.Thickness--;
                this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.Thickness * edgeWidthScaleFactor), 1f);
            }
        }

        if (Edge != null) {
            if (!this.curved) {
                // Stretch the edge between the two vertices
                StretchBetweenPoints(this.Vertex1.transform.position, this.Vertex2.transform.position);
            }
            else {
                transform.parent.position = this.Vertex1.transform.position;
                UpdateSpline();
            }
        }
    }

    public void UpdateSpline() {
        Vector3[] pointsOnCurve = {new Vector3(0f, 0.3f, 0f), new Vector3(.5f, 2f, 0f), new Vector3(1f, 0f, 0f), new Vector3(.5f, -2f, 0f), new Vector3(0f, -0.3f, 0f)};

        this.shapeController.spline.Clear();
        int splinePointIndex = 0;
        for (int i = 0; i < pointsOnCurve.Length; i++) {
            if (i == 0 || i == 1) {
                this.shapeController.spline.InsertPointAt(splinePointIndex, pointsOnCurve[i] + new Vector3(0f, 0.2f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Linear);

                if (i == 1) {
                    this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                    this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(-.33f, 0, 0f));
                    this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(.25f, 0, 0f));
                }
            }
            else if (i == 2) {
                this.shapeController.spline.InsertPointAt(splinePointIndex,  pointsOnCurve[i] + new Vector3(0.125f, 0f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(0f, 2f, 0f));
                this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(0f, -2f, 0f));
            }
            else if (i == 4 || i == 3) {
                this.shapeController.spline.InsertPointAt(splinePointIndex,  pointsOnCurve[i] + new Vector3(0f, -0.2f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Linear);

                if (i == 3) {
                    this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                    this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(.25f, 0, 0f));
                    this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(-.33f, 0, 0f));
                }
            }
            splinePointIndex++;
        }

        for (int i = pointsOnCurve.Length - 1; i >= 0; i--) {
            if (i == 0 || i == 1) {
                this.shapeController.spline.InsertPointAt(splinePointIndex,  pointsOnCurve[i] + new Vector3(0f, -0.2f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Linear);

                if (i == 1) {
                    this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                    this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(.33f, 0, 0f));
                    this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(-.25f, 0, 0f));
                }
            }
            else if (i == 2) {
                this.shapeController.spline.InsertPointAt(splinePointIndex,  pointsOnCurve[i] + new Vector3(0f, 0f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(0f, -1.5f, 0f));
                this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(0f, 1.5f, 0f));
            }
            else if (i == 4 || i == 3) {
                this.shapeController.spline.InsertPointAt(splinePointIndex,  pointsOnCurve[i] + new Vector3(0f, 0.2f, 0f));
                this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Linear);

                if (i == 3) {
                    this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
                    this.shapeController.spline.SetLeftTangent(splinePointIndex, new Vector3(-.25f, 0, 0f));
                    this.shapeController.spline.SetRightTangent(splinePointIndex, new Vector3(.33f, 0, 0f));
                }
            }
            splinePointIndex++;
        }

        // this.arrow.transform.rotation = Quaternion.AngleAxis(45f, Vector3.forward);
        this.arrow.gameObject.SetActive(true);
    }

    // Toggle between undirected, direction 1, and direction -1
    public void ToggleEdgeType() {
        // if (!this.Edge.directed) {
        //     this.Edge.directed = true;
        //     // this.direction = 1;
        // }
        // else {
        //     // if (this.direction == 1) {
        //     //     this.Edge.Reverse();
        //     //     this.direction = -1;
        //     // }
        //     // else {
        //     //     this.Edge.Reverse();
        //     //     this.direction = 1;
        //     //     this.Edge.directed = false;
        //     // }
        // }
        this.Edge.Directed = !this.Edge.Directed;
    }

    // When Cursor enters a edge obj, increase its sprite object size by 33%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, (0.25f + (this.Edge.Thickness * edgeWidthScaleFactor)) * 1.33f, 1f);
    }

    private void OnMouseExit()
    {
        // When cursor exits, reset the thickness
        this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.Thickness * edgeWidthScaleFactor), 1f);
    }

    // public void UpdateWeight(double newWeight) {
    //     this.Edge.Weight = newWeight;
    // }
}
