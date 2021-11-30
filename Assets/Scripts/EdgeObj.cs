using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObj : MonoBehaviour
{
    // ID of vertices of the two associated vertices in the graph data structure
    // Starts at -1 which means the edge is uninitiated
    private int fromID = -1;
    private int toID = -1;

    // Uses a custom timer to reduce the frequency of physics updates (to reduce lag)
    private float physicsTimer = 0f;

    // Reference to the game object of the target vertex
    public GameObject targetVertexObj;

    private void Awake() {
        // Edge objects starts non active
        gameObject.SetActive(false);
        
        // Do not let the physics engine update the collider of the edge in real time
        // as it causes massive lag at the start while the graph is still settling in.
        Physics2D.autoSyncTransforms = false;
    }

    public void Initiate(int from, int to, GameObject target) {
        fromID = from;
        toID = to;

        targetVertexObj = target;

        gameObject.SetActive(true);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        transform.position = new Vector3(point1.x, point1.y, 1);
        Vector2 dir = point2 - point1;
        transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate() {
        if (fromID != -1 && toID != -1) {
            // Stretch the edge between the two vertices
            StretchBetweenPoints(transform.parent.position, targetVertexObj.transform.position);
        }

        // Only update the Physics 2D collider of the edge every 0.25s instead of real time to reduce physics lag
        if (physicsTimer >= 0.25f) {
            Physics2D.SyncTransforms();
            physicsTimer = 0f;
        }
        else {
            physicsTimer += Time.fixedDeltaTime;
        }
    }
}
