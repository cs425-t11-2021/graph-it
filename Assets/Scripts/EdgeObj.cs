using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObj : MonoBehaviour
{
    // ID of vertices of the two associated vertices in the graph data structure
    // Starts at -1 which means the edge is uninitiated
    private int from_id = -1;
    private int to_id = -1;

    // Reference to the game object of the target vertex
    public GameObject targetVertexObj;

    private void Awake() {
        // Edge objects starts non active
        gameObject.SetActive(false);
    }

    public void Initiate(int from, int to, GameObject target) {
        from_id = from;
        to_id = to;

        targetVertexObj = target;

        gameObject.SetActive(true);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        transform.position = point1;
        Vector2 dir = point2 - point1;
        transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate() {
        if (from_id != -1 && to_id != -1) {
            // Stretch the edge between the two vertices
            StretchBetweenPoints(transform.parent.position, targetVertexObj.transform.position);
        }
    }
}
