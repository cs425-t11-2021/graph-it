//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObj : MonoBehaviour
{
    //// ID of vertices of the two associated vertices in the graph data structure
    //// Starts at -1 which means the edge is uninitiated
    //private int fromID = -1;
    //private int toID = -1;

    // ID of the associated edge in the graph data structure, -1 is unintialized
    // public int id = -1;

    // Property refernce of edge associated with the edge object
    public Edge Edge { get; private set; }

    // Label of the edge
    private string label;

    // Uses a custom timer to reduce the frequency of physics updates (to reduce lag)
    private float physicsTimer = 0f;

    // Reference to the game object of the target vertex
    public GameObject targetVertexObj;

    // TODO: Remove once animations are implemented
    // Whether edge is selected in the SelectionManager
    private bool selected = false;
    // Reference to the spriteRenderer component of the object
    private SpriteRenderer spriteRenderer;

    // // Getter for id
    // public int GetID()
    // {
    //     return id;
    // }

    private void Awake() {
        // Edge objects starts non active
        this.gameObject.SetActive(false);
        
        // Do not let the physics engine update the collider of the edge in real time
        // as it causes massive lag at the start while the graph is still settling in.
        Physics2D.autoSyncTransforms = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // TODO: Modify this initialize code to not involve passing around a Unity GameObject
    public void Initiate(Edge edge, GameObject target) {
        this.Edge = edge;

        this.targetVertexObj = target;
        this.gameObject.SetActive(true);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        this.transform.position = new Vector3(point1.x, point1.y, 1);
        Vector2 dir = point2 - point1;
        this.transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate() {
        if (Edge != null) {
            // Stretch the edge between the two vertices
            StretchBetweenPoints(this.transform.parent.position, targetVertexObj.transform.position);
        }

        // Only update the Physics 2D collider of the edge every 0.25s instead of real time to reduce physics lag
        if (this.physicsTimer >= 0.25f) {
            Physics2D.SyncTransforms();
            this.physicsTimer = 0f;
        }
        else {
            this.physicsTimer += Time.fixedDeltaTime;
        }
    }

    // When Cursor enters a edge obj, increase its sprite object size by 10%
    // TODO: Change this to be controlled by an animator later
    private void OnMouseOver()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, .33f, 1f);

    }

    // When user clicks a edge obj, select/deselect it using selection manager
    // Change color to blue when selected
    // TODO: Replace with Unity animator
    private void OnMouseDown()
    {
        // Disable edge selection if quick edge creation mode is enabled
        if (Toolbar.singleton.AddEdgeMode) return;

        SetSelected(!selected);
    }

    // Method for changing whether or not object is selected
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        if (!selected)
        {
            SelectionManager.singleton.DeselectEdge(this);
            this.spriteRenderer.color = new Color32(0, 0, 0, 255);
        }
        else
        {
            SelectionManager.singleton.SelectEdge(this);
            this.spriteRenderer.color = new Color32(0, 125, 255, 255);
        }
    }

    private void OnMouseExit()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, .25f, 1f);
    }
}
