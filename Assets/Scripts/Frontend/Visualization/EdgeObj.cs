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
    private bool hovering = false;

    
    // Property for getting and setting whether or not the edge is selected, and edit the edge's color to match
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            if (value) {
                if (!this.curved)
                    this.shapeRenderer.color = new Color32(0, 125, 255, 255);
                else
                    this.shapeRenderer.color = new Color32(0, 125, 255, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 125, 255, 255);
                labelObj.MakeEditable();
            }
            else {
                if (!this.curved)
                    this.shapeRenderer.color = new Color32(0, 0, 0, 255);
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

        this.curved = edge.vert1 == edge.vert2;
        
        // Fix for edge temporarily appearing in the wrong place when getting added
        if (spriteRenderer) spriteRenderer.enabled = false;

        // if (this.curved) {
        this.shapeController = GetComponent<SpriteShapeController>();
        this.shapeRenderer = GetComponent<SpriteShapeRenderer>();
        // }
        
        this.labelObj.Initiate(this);
    }

    // Method to stretch the edge so it extends from one point to another 
    // private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    // {
    //     this.transform.parent.position = point1;
    //     Vector2 dir = point2 - point1;
    //     this.transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

    //     float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    //     if (this.Edge.directed) {
    //         this.arrow.localPosition = new Vector3((this.Edge.directed ? 0.5f : 0f) - (this.arrowSpriteRenderer.size.x / this.transform.localScale.x), 0f, 0f);
    //         this.arrow.localScale = new Vector3(1f / this.transform.lossyScale.x, 1f / (this.transform.lossyScale.y - this.Edge.thickness * edgeWidthScaleFactor), 1);
    //         this.arrow.localRotation = Quaternion.AngleAxis(this.Edge.directed ? 0f : 180f, Vector3.forward);
    //         this.arrow.gameObject.SetActive(true);
    //     }
    //     else {
    //         this.arrow.gameObject.SetActive(false);
    //     }
        
    //     // Fix for edge temporarily appearing in the wrong place when getting added
    //     if (spriteRenderer) spriteRenderer.enabled = true;
    // }

    private void Update() {
        if (this.Selected) {
            // If selected and Plus is pressed, increase thickness
            // if (Input.GetKeyDown(KeyCode.Equals) && this.Edge.thickness < 5) {
            //     this.Edge.thickness++;
            //     this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
            // }
            // // If selected and Minus is pressed, decrease thickness
            // else if (Input.GetKeyDown(KeyCode.Minus) && this.Edge.thickness > 0) {
            //     this.Edge.thickness--;
            //     this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
            // }
        }

        if (this.Edge != null) {
            transform.parent.position = this.Vertex1.transform.position;
            if (!this.curved) {
                // Stretch the edge between the two vertices
                // StretchBetweenPoints(this.Vertex1.transform.position, this.Vertex2.transform.position);
                // UpdateStraightSpline();
                UpdateCurvedSpline(-1f);
            }
            else {
                UpdateCircularSpline(0.7f, 45f);
            }
        }
    }

    // TODO: Find a way not to hard code this
    private void UpdateCircularSpline(float largeRadius, float angle) {
        Vector3[] pointsOnCurve = {new Vector3(largeRadius, 0f, 0f), new Vector3(0f, largeRadius, 0f), new Vector3(-largeRadius, 0f, 0f), new Vector3(0f, -largeRadius, 0f)};

        this.shapeController.spline.Clear();
        int splinePointIndex = 0;
        for (int i = 0; i < pointsOnCurve.Length; i++) {
            this.shapeController.spline.InsertPointAt(splinePointIndex,  Quaternion.AngleAxis(angle, Vector3.forward) * pointsOnCurve[i]);
            this.shapeController.spline.SetTangentMode(splinePointIndex, ShapeTangentMode.Continuous);
            splinePointIndex++;
        }

        float largeArm = .55f * largeRadius;
        float smallArm = .45f * largeRadius;
        
        this.shapeController.spline.SetTangentMode(0, ShapeTangentMode.Broken);
        this.shapeController.spline.SetLeftTangent(0, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-smallArm, 0f, 0f));
        this.shapeController.spline.SetRightTangent(0, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, largeArm, 0f));
        this.shapeController.spline.SetLeftTangent(1, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(largeArm, 0f, 0f));
        this.shapeController.spline.SetRightTangent(1, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-largeArm, 0f, 0f));
        this.shapeController.spline.SetLeftTangent(2, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, largeArm, 0f));
        this.shapeController.spline.SetRightTangent(2, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, -largeArm, 0f));
        this.shapeController.spline.SetLeftTangent(3, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-largeArm, 0f, 0f));
        this.shapeController.spline.SetRightTangent(3, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(largeArm, 0f, 0f));
        
        this.shapeController.spline.InsertPointAt(4,  Quaternion.AngleAxis(angle, Vector3.forward) * (pointsOnCurve[0]+ new Vector3(0f, 0f, 1f)));
        this.shapeController.spline.SetTangentMode(4, ShapeTangentMode.Broken);
        this.shapeController.spline.SetLeftTangent(4, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, -largeArm, 0f));
        this.shapeController.spline.SetRightTangent(4, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-smallArm, 0, 0f));
        
        pointsOnCurve = new Vector3[] {new Vector3(largeRadius - this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f), 0f, 0f), new Vector3(0f, largeRadius - this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f), 0f), new Vector3(-(largeRadius - this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f)), 0f, 0f), new Vector3(0f, -(largeRadius - this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f)), 0f)};
        
        this.shapeController.spline.InsertPointAt(5,  Quaternion.AngleAxis(angle, Vector3.forward) * pointsOnCurve[0]);
        this.shapeController.spline.SetTangentMode(5, ShapeTangentMode.Broken);
        this.shapeController.spline.SetRightTangent(5, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, -smallArm, 0f));
        this.shapeController.spline.SetLeftTangent(5, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(smallArm, 0, 0f));
        
        this.shapeController.spline.InsertPointAt(6,  Quaternion.AngleAxis(angle, Vector3.forward) * pointsOnCurve[3]);
        this.shapeController.spline.SetTangentMode(6, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(6, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(smallArm, 0f, 0f));
        this.shapeController.spline.SetRightTangent(6, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-smallArm, 0f, 0f));
        
        this.shapeController.spline.InsertPointAt(7,  Quaternion.AngleAxis(angle, Vector3.forward) * pointsOnCurve[2]);
        this.shapeController.spline.SetTangentMode(7, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(7, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, -smallArm, 0f));
        this.shapeController.spline.SetRightTangent(7, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, smallArm, 0f));
        
        this.shapeController.spline.InsertPointAt(8,  Quaternion.AngleAxis(angle, Vector3.forward) * pointsOnCurve[1]);
        this.shapeController.spline.SetTangentMode(8, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(8, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(-smallArm, 0f, 0f));
        this.shapeController.spline.SetRightTangent(8, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(smallArm, 0f, 0f));
        
        this.shapeController.spline.InsertPointAt(9,  Quaternion.AngleAxis(angle, Vector3.forward) * (pointsOnCurve[0] + new Vector3(0f, 0f, 1f)));
        this.shapeController.spline.SetTangentMode(9, ShapeTangentMode.Broken);
        this.shapeController.spline.SetLeftTangent(9, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0f, smallArm, 0f));
        this.shapeController.spline.SetRightTangent(9, Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(smallArm, 0f, 0f));

        this.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(largeRadius * .9f, 0f, 0f);

        // this.arrow.transform.rotation = Quaternion.AngleAxis(45f, Vector3.forward);
        // this.arrow.gameObject.SetActive(true);

        if (this.Edge.directed) {
            Vector3 dir =  (Quaternion.AngleAxis(angle - 20, Vector3.forward) * pointsOnCurve[1]).normalized;
            this.arrow.rotation = Quaternion.AngleAxis(angle - 110, Vector3.forward);
            this.arrow.position = this.transform.parent.position + (dir * this.arrowSpriteRenderer.size.x);
            this.arrow.localScale = new Vector3(1f, (1f + this.Edge.thickness * edgeWidthScaleFactor) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void UpdateStraightSpline() {
        Vector3 distance = this.Vertex2.transform.position - this.Vertex1.transform.position;
        Vector3[] pointsOnCurve = {Vector3.zero, distance};
        Vector3 normal = Vector2.Perpendicular(distance).normalized;

        this.shapeController.spline.Clear();
        this.shapeController.spline.InsertPointAt(0,  pointsOnCurve[0] + normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(0, ShapeTangentMode.Linear);
        this.shapeController.spline.InsertPointAt(1,  pointsOnCurve[1] + normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(1, ShapeTangentMode.Linear);
        this.shapeController.spline.InsertPointAt(2,  pointsOnCurve[1] - normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(2, ShapeTangentMode.Linear);
        this.shapeController.spline.InsertPointAt(3,  pointsOnCurve[0] - normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(3, ShapeTangentMode.Linear);

        if (this.Edge.directed) {
            float angle = Mathf.Atan2(distance.normalized.y, distance.normalized.x) * Mathf.Rad2Deg;
            this.arrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.arrow.localPosition = distance * (1f - (this.arrowSpriteRenderer.size.x + .066f) / distance.magnitude);
            this.arrow.localScale = new Vector3(1f, (1f + this.Edge.thickness * edgeWidthScaleFactor) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void UpdateCurvedSpline(float radius) {
        Vector3 distance = this.Vertex2.transform.position - this.Vertex1.transform.position;
        Vector3 normal = Vector2.Perpendicular(distance).normalized;

        Vector3[] pointsOnCurve = {Vector3.zero, distance / 2f + (radius * normal), distance};

        this.shapeController.spline.Clear();

        this.shapeController.spline.InsertPointAt(0,  pointsOnCurve[0] + normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(0, ShapeTangentMode.Linear);

        this.shapeController.spline.InsertPointAt(1,  pointsOnCurve[1] + normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(1, -distance * .25f);
        this.shapeController.spline.SetRightTangent(1, distance * .25f);

        this.shapeController.spline.InsertPointAt(2,  pointsOnCurve[2] + normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(2, ShapeTangentMode.Linear);

        this.shapeController.spline.InsertPointAt(3,  pointsOnCurve[2] - normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f) + (pointsOnCurve[1] - pointsOnCurve[2]).normalized * 0.1f);
        this.shapeController.spline.SetTangentMode(3, ShapeTangentMode.Linear);

        this.shapeController.spline.InsertPointAt(4,  pointsOnCurve[1] - normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.66f : 1.25f));
        this.shapeController.spline.SetTangentMode(4, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(4, distance * .25f);
        this.shapeController.spline.SetRightTangent(4, -distance * .25f);
        
        this.shapeController.spline.InsertPointAt(5,  pointsOnCurve[0] - normal * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f) + (pointsOnCurve[1] - pointsOnCurve[0]).normalized * 0.1f);
        this.shapeController.spline.SetTangentMode(5, ShapeTangentMode.Linear);

        if (this.Edge.directed) {
            distance = this.Vertex2.transform.position - (this.transform.position + pointsOnCurve[1]);
            float angle = Mathf.Atan2(distance.normalized.y, distance.normalized.x) * Mathf.Rad2Deg;
            this.arrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.arrow.position = this.Vertex2.transform.position + -distance.normalized * (this.arrowSpriteRenderer.size.x + .066f);
            // this.arrow.localPosition = distance * (1f - (this.arrowSpriteRenderer.size.x + .066f) / distance.magnitude);
            this.arrow.localScale = new Vector3(1f, (1f + this.Edge.thickness * edgeWidthScaleFactor) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }

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
        this.Edge.directed = !this.Edge.directed;
    }

    // When Cursor enters a edge obj, increase its sprite object size by 33%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        // if (this.curved)
        // {
        //     this.hovering = true;
        // }
        // else
        // {
        //     this.transform.localScale = new Vector3(this.transform.localScale.x, (0.25f + (this.Edge.thickness * edgeWidthScaleFactor)) * 1.33f, 1f);
        // }
        this.hovering = true;
        
    }

    private void OnMouseExit()
    {
        // When cursor exits, reset the thickness
        // if (this.curved)
        // {
        //     this.hovering = false;
        // }
        // else
        // {
        //     this.transform.localScale = new Vector3(this.transform.localScale.x, 0.25f + (this.Edge.thickness * edgeWidthScaleFactor), 1f);
        // }
        this.hovering = false;
    }

    public void UpdateWeight(double newWeight) {
        this.Edge.weight = newWeight;
    }
}
