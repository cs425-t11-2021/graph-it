using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexMovement : MonoBehaviour
{
    private Vector3 cursorOffset;
    private bool followCursor;
    public bool FollowCursor {
        get => this.followCursor;
        set {
            Logger.Log(string.Format("Vertex {0}.", value ? "picked up" : "droped off."), this, LogType.DEBUG);
            this.cursorOffset = this.transform.position - Controller.Singleton.GetCursorWorldPosition();
            this.followCursor = value;
        }
    }

    private void Update() {
        if (this.followCursor) {
            this.transform.position = Controller.Singleton.GetCursorWorldPosition() + this.cursorOffset;
        }
    }
}
