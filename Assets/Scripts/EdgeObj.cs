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
    private int id = -1;
    // Label of the edge
    private string label;

    // Uses a custom timer to reduce the frequency of physics updates (to reduce lag)
    private float physicsTimer = 0f;

    // Reference to the game object of the target vertex
    public GameObject targetVertexObj;

    // Getter for id
    public int GetID()
    {
        return id;
    }

    private void Awake() {
        // Edge objects starts non active
        this.gameObject.SetActive(false);
        
        // Do not let the physics engine update the collider of the edge in real time
        // as it causes massive lag at the start while the graph is still settling in.
        Physics2D.autoSyncTransforms = false;
    }

    // TODO: Modify this initialize code to not involve passing around a Unity GameObject
    public void Initiate(Edge edge, GameObject target) {
        this.id = edge.id;
        this.label = edge.label;

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
        if (id != -1) {
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
}
