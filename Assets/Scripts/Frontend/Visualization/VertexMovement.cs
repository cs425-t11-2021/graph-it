using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for controlling the movement of vertices (being dragged by the mouse)
public class VertexMovement : MonoBehaviour
{
    // The distance between the starting position of the vertex and the cursor world position
    private Vector3 cursorOffset;
    // Whether or no the associated vertex object should follow the cursor
    private bool followCursor;
    // Public property for setting followCursor
    public bool FollowCursor {
        get => this.followCursor;
        set {
            Logger.Log(string.Format("Vertex {0}.", value ? "picked up" : "droped off."), this, LogType.DEBUG);
            this.cursorOffset = this.transform.position - InputManager.Singleton.CursorWorldPosition;
            this.followCursor = value;
        }
    }

    // Update the position of the vertex to follow the cursor if followCusor is true
    private void Update() {
        if (this.followCursor) {
            this.transform.position = InputManager.Singleton.CursorWorldPosition + this.cursorOffset;
        }
    }
}
