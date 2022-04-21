//All code developed by Team 11

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class EdgeObj : MonoBehaviour
{
    // Property reference of edge associated with the edge object
    public Edge Edge { get; private set; }

    // Int representing the direction of the label, 0 being undirected, 1 being the direction given at creation, and -1 being reversed
    private int direction;

    public VertexObj Vertex1 {get; private set;}
    public VertexObj Vertex2 {get; private set;}
    
    // Curved Visuals
    [SerializeField] private GameObject curvedVisuals;
    private SpriteShapeController shapeController;
    private SpriteShapeRenderer shapeRenderer;
    
    // Straight Visuals
    [SerializeField] private GameObject straightVisuals;
    private SpriteRenderer spriteRenderer;
    
    // Directed Edge Visuals
    [SerializeField] private Transform arrow;
    private SpriteRenderer arrowSpriteRenderer;
    
    // State variables
    // Whether edge is selected in the SelectionManager
    private bool selected = false;
    // Whether edge is currently being hovered over
    private bool hovering = false;

    // Other visual things
    // Width scale factor for edge thickness increse of 1
    [SerializeField] private float edgeWidthScaleFactor = 0.05f;
    // Edge weights/labels
    public EdgeLabelObj labelObj;
    
    // Property for getting and setting whether or not the edge is selected, and edit the edge's color to match
    public bool Selected {
        get => this.selected;
        set {
            this.selected = value;
            if (value) {
                this.shapeRenderer.color = new Color32(0, 125, 255, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 125, 255, 255);
                this.spriteRenderer.color = new Color32(0, 125, 255, 255);
                labelObj.MakeEditable();
                shapeRenderer.sortingOrder = -1;
                spriteRenderer.sortingOrder = -1;
            }
            else {
                this.shapeRenderer.color = new Color32(0, 0, 0, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 0, 0, 255);
                this.spriteRenderer.color = new Color32(0, 0, 0, 255);
                labelObj.MakeUneditable();
                shapeRenderer.sortingOrder = -2;
                spriteRenderer.sortingOrder = -2;
            }
        }
    }

    private int resultStatus = 0;
    private float resultTimer = 0f;
    public int AlgorithmResultLevel
    {
        set
        {
            this.resultStatus = value;
            if (this.resultStatus == 0)
            {
                this.shapeRenderer.color = new Color32(0, 0, 0, 255);
                this.arrowSpriteRenderer.color = new Color32(0, 0, 0, 255);
                this.spriteRenderer.color = new Color32(0, 0, 0, 255);
                shapeRenderer.sortingOrder = -2;
                spriteRenderer.sortingOrder = -2;
            }
            else
            {
                shapeRenderer.sortingOrder = -1;
                spriteRenderer.sortingOrder = -1;
            }

            if (this.resultStatus == 1)
            {
                this.shapeRenderer.color = Controller.Singleton.algorithmResultColors[0];
                this.arrowSpriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                this.resultTimer = 0f;
            }
        }
    }

    private void Awake() {
        // Edge objects starts non active
        this.gameObject.SetActive(false);

        this.spriteRenderer = this.straightVisuals.GetComponent<SpriteRenderer>();
        this.arrowSpriteRenderer = this.arrow.GetComponent<SpriteRenderer>();
        this.shapeController = this.curvedVisuals.GetComponent<SpriteShapeController>();
        this.shapeRenderer = this.curvedVisuals.GetComponent<SpriteShapeRenderer>();
    }

    public void Initiate(Edge edge, VertexObj vertex1, VertexObj vertex2) {
        this.Edge = edge;

        this.Vertex1 = vertex1;
        this.Vertex2 = vertex2;
        
        this.gameObject.SetActive(true);

        if (edge.vert1 == edge.vert2)
        {
            this.Edge.SetCurvature(int.MaxValue, false);
        }

        this.direction = edge.Directed ? 1 : 0;

        this.labelObj.Initiate(this);

        this.Vertex1.OnVertexObjMove += UpdateSpline;
        this.Vertex1.OnVertexObjMove += UpdateLabelPosition;
        this.Vertex2.OnVertexObjMove += UpdateSpline;
        this.Vertex2.OnVertexObjMove += UpdateLabelPosition;
        
        UpdateSpline();
    }

    private void Update()
    {
        if (resultStatus == 1)
        {
            if (resultTimer <= 0f)
            {
                resultTimer = 0.33f;
                if (this.spriteRenderer.color == new Color32(0, 0, 0, 255))
                {
                    this.shapeRenderer.color = Controller.Singleton.algorithmResultColors[0];
                    this.arrowSpriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                    this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[0];
                }
                else
                {
                    this.shapeRenderer.color = new Color32(0, 0, 0, 255);
                    this.arrowSpriteRenderer.color = new Color32(0, 0, 0, 255);
                    this.spriteRenderer.color = new Color32(0, 0, 0, 255);
                }
            }

            resultTimer -= Time.deltaTime;
        }
        else if (resultStatus == 2)
        {
            this.shapeRenderer.color = Controller.Singleton.algorithmResultColors[1];
            this.arrowSpriteRenderer.color = Controller.Singleton.algorithmResultColors[1];
            this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[1];
        }
        else if (resultStatus == 3)
        {
            this.shapeRenderer.color = Controller.Singleton.algorithmResultColors[2];
            this.arrowSpriteRenderer.color = Controller.Singleton.algorithmResultColors[2];
            this.spriteRenderer.color = Controller.Singleton.algorithmResultColors[2];
        }
    }

    public void UpdateSpline()
    {
        this.transform.parent.position = this.Vertex1.transform.position + new Vector3(0f, 0f, 1f);
        if (this.Edge.Curvature == int.MaxValue) {
            UpdateCircularSpline(0.7f, FindBestAngleForLoop());
        }
        else if (this.Edge.Curvature == 0) {
            UpdateStraightSpline();
        }
        else
        {
            UpdateCurvedSpline(this.Edge.Curvature * 0.1f);
        }
    }

    private void UpdateLabelPosition() {
        this.labelObj.UpdatePosition();
    }

    private float FindBestAngleForLoop()
    {
        List<float> connectedEdgeAngles = new List<float>();
        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (edgeObj.Vertex1 == edgeObj.Vertex2) continue;
            
            // Connected To
            if (edgeObj.Vertex2 == this.Vertex1)
            {
                float angle = Mathf.Atan2(edgeObj.Vertex1.transform.position.y - this.Vertex2.transform.position.y,
                    edgeObj.Vertex1.transform.position.x - this.Vertex2.transform.position.x) * Mathf.Rad2Deg;
                // if (angle < 0) angle += 180f;
                connectedEdgeAngles.Add(angle);
            }
            // Connected From
            else if (edgeObj.Vertex1 == this.Vertex1)
            {
                float angle = Mathf.Atan2(edgeObj.Vertex2.transform.position.y - this.Vertex1.transform.position.y,
                    edgeObj.Vertex2.transform.position.x - this.Vertex1.transform.position.x) * Mathf.Rad2Deg;
                // if (angle < 0) angle += 180f;
                connectedEdgeAngles.Add(angle);
            }
        }

        if (connectedEdgeAngles.Count == 0) return 0f;
        return AngleAverage(connectedEdgeAngles) + 180f;
    }

    private float AngleAverage(List<float> connectedEdgeAngles)
    {
        float num = 0;
        float den = 0;
        foreach (float angle in connectedEdgeAngles)
        {
            num += Mathf.Sin(angle * Mathf.Deg2Rad);
            den += Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        return Mathf.Atan2(num, den) * Mathf.Rad2Deg;
    }

    // TODO: Find a way not to hard code this
    private void UpdateCircularSpline(float largeRadius, float angle) {
        this.straightVisuals.SetActive(false);
        this.curvedVisuals.SetActive(true);
        
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
        
        pointsOnCurve = new Vector3[] {new Vector3(largeRadius - (1 + this.Edge.Thickness) * this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f), 0f, 0f), new Vector3(0f, largeRadius - (1 + this.Edge.Thickness) * this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f), 0f), new Vector3(-(largeRadius - (1 + this.Edge.Thickness) * this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f)), 0f, 0f), new Vector3(0f, -(largeRadius - (1 + this.Edge.Thickness) * this.edgeWidthScaleFactor * (this.hovering ? 1.33f : 1f)), 0f)};
        
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

        this.curvedVisuals.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(largeRadius * .9f, 0f, 0f);

        if (this.Edge.Directed) {
            Vector3 dir =  (Quaternion.AngleAxis(angle - 20, Vector3.forward) * pointsOnCurve[1]).normalized;
            this.arrow.rotation = Quaternion.AngleAxis(angle - 110, Vector3.forward);
            this.arrow.position = this.transform.parent.position + (dir * this.arrowSpriteRenderer.size.x);
            this.arrow.localScale = new Vector3(1f, (.5f + (1f + this.Edge.Thickness) * (.25f)) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void UpdateStraightSpline()
    {
        this.curvedVisuals.SetActive(false);
        this.straightVisuals.SetActive(true);
        
        Vector2 distance = this.Vertex2.transform.position - this.Vertex1.transform.position;
        this.straightVisuals.transform.localScale = new Vector3(distance.magnitude * 2 - 4 * (Vertex2.spriteRadius + SettingsManager.Singleton.EdgeVertexGap) - 2 * (this.Edge.Directed ? this.arrowSpriteRenderer.size.x / 2f : 0f), (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor * 2f * (this.hovering ? 1.33f : 1f), 1);
        this.straightVisuals.transform.localPosition = distance.normalized * (Vertex1.spriteRadius + SettingsManager.Singleton.EdgeVertexGap);
        
        float angle = Mathf.Atan2(distance.normalized.y, distance.normalized.x) * Mathf.Rad2Deg;
        this.straightVisuals.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (this.Edge.Directed) {
            this.arrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.arrow.localPosition = distance * (1f - (this.arrowSpriteRenderer.size.x + this.Vertex2.spriteRadius) / distance.magnitude) - distance.normalized * SettingsManager.Singleton.EdgeVertexGap;
            this.arrow.localScale = new Vector3(1f, (.5f + (1f + this.Edge.Thickness) * (.25f)) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }

    private void UpdateCurvedSpline(float radius) {
        this.straightVisuals.SetActive(false);
        this.curvedVisuals.SetActive(true);

        Vector3 distance = this.Vertex2.transform.position - this.Vertex1.transform.position;
        Vector3 normal = Vector2.Perpendicular(distance).normalized;

        Vector3[] pointsOnCurve = {Vector3.zero + distance.normalized * (this.Vertex1.spriteRadius + SettingsManager.Singleton.EdgeVertexGap), distance / 2f + (radius * normal), distance - distance.normalized * (this.Vertex2.spriteRadius + SettingsManager.Singleton.EdgeVertexGap + (this.Edge.Directed ? this.arrowSpriteRenderer.size.x / 2f : 0f))};
        
        this.shapeController.spline.Clear();
        
        this.shapeController.spline.InsertPointAt(0,  pointsOnCurve[0] + normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(0, ShapeTangentMode.Linear);
        
        this.shapeController.spline.InsertPointAt(1,  pointsOnCurve[1] + normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(1, -distance * .25f);
        this.shapeController.spline.SetRightTangent(1, distance * .25f);

        this.shapeController.spline.InsertPointAt(2,  pointsOnCurve[2] + normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f));
        this.shapeController.spline.SetTangentMode(2, ShapeTangentMode.Linear);

        this.shapeController.spline.InsertPointAt(3,  pointsOnCurve[2] - normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f) + (pointsOnCurve[1] - pointsOnCurve[2]).normalized * 0.1f);
        this.shapeController.spline.SetTangentMode(3, ShapeTangentMode.Linear);

        this.shapeController.spline.InsertPointAt(4,  pointsOnCurve[1] - normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.66f : 1.25f));
        this.shapeController.spline.SetTangentMode(4, ShapeTangentMode.Continuous);
        this.shapeController.spline.SetLeftTangent(4, distance * .25f);
        this.shapeController.spline.SetRightTangent(4, -distance * .25f);
        
        this.shapeController.spline.InsertPointAt(5,  pointsOnCurve[0] - normal * (this.Edge.Thickness + 1) * this.edgeWidthScaleFactor / 2f * (this.hovering ? 1.33f : 1f) + (pointsOnCurve[1] - pointsOnCurve[0]).normalized * 0.1f);
        this.shapeController.spline.SetTangentMode(5, ShapeTangentMode.Linear);

        if (this.Edge.Directed) {
            distance = this.Vertex2.transform.position - (this.transform.position + pointsOnCurve[1]);
            float angle = Mathf.Atan2(distance.normalized.y, distance.normalized.x) * Mathf.Rad2Deg;
            this.arrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.arrow.position = this.Vertex2.transform.position + -distance.normalized * (this.arrowSpriteRenderer.size.x + this.Vertex2.spriteRadius) - distance.normalized * SettingsManager.Singleton.EdgeVertexGap;
            this.arrow.localScale = new Vector3(1f, (.5f + (1f + this.Edge.Thickness) * (.25f)) * (this.hovering ? 1.33f : 1f), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }

    }

    // Toggle between undirected, direction 1, and direction -1
    public void ToggleEdgeType() {
        // TEMPORARY: If special case where two directed edges are connected to a pair of vertices, do not allow type change until one is removed
        if (this.Edge.Directed && Controller.Singleton.Graph.Adjacency.ContainsKey( (this.Edge.vert1, this.Edge.vert2) ) && Controller.Singleton.Graph.Adjacency.ContainsKey( (this.Edge.vert2, this.Edge.vert1) ))
        {
            Logger.Log("Modifying the type of a directed edge with a corresponding directed edge connecting the same pair of vertices is unsupported at this time", this, LogType.WARNING);
            return;
        }
        
        if (this.direction == 0)
        {
            SetDirectedness(true);
        }
        else if (this.direction == 1)
        {
            ReverseEdge();
        }
        else
        {
            SetDirectedness(false);
        }
    }

    public void SetDirectedness(bool directed, bool updateDS = true) {
        if (updateDS)
        {
            this.Edge.Directed = directed;
            Controller.Singleton.ForceInvokeModificationEvent();
        }
        
        if (directed) this.direction = 1;
        else this.direction = 0;
        UpdateSpline();
    }

    public void ReverseEdge(bool updateDS = true) {
        if (updateDS)
        {
            this.Edge.Reverse();
            Controller.Singleton.ForceInvokeModificationEvent();
        }
        this.direction *= -1;
        (this.Vertex1, this.Vertex2) = (this.Vertex2, this.Vertex1);
        UpdateSpline();
    }
    // When Cursor enters a edge obj, increase its sprite object size by 33%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        this.hovering = true;
        UpdateSpline();
    }

    private void OnMouseExit()
    {
        this.hovering = false;
        UpdateSpline();
    }

    // public void ChangeThickness(int change)
    // {
    //     uint newThickness = this.Edge.Thickness;
    //     if (change > 0)
    //     {
    //         newThickness = this.Edge.Thickness + 1;
    //         if (newThickness > 5)
    //         {
    //             newThickness = 5;
    //         }
    //     }
    //     else
    //     {
    //         if (this.Edge.Thickness != 0)
    //         {
    //             newThickness = this.Edge.Thickness - 1;
    //         }
    //     }
        
    //     Logger.Log("Edge thickness changed to " + newThickness, this, LogType.INFO);
    //     SetThickness(newThickness);
    // }

    private void OnDestroy()
    {
        this.Vertex1.OnVertexObjMove -= UpdateSpline;
        this.Vertex1.OnVertexObjMove -= UpdateLabelPosition;
        this.Vertex2.OnVertexObjMove -= UpdateSpline;
        this.Vertex2.OnVertexObjMove -= UpdateLabelPosition;
    }
}
