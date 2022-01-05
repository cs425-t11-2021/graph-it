using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behavior for handling dragging graph objects, requires colliders to be attached to the objects
public class ObjDrag : MonoBehaviour
{
    private Vector3 cursorOffset;

    // When the mouse is first pressed, store the cursor offset in relation to the transform position
    private void OnMouseDown()
    {
        cursorOffset = this.transform.position - Controller.singleton.GetCursorWorldPosition();
    }

    // When the object is dragged, set its position to be cursor position with offset
    private void OnMouseDrag()
    {
        this.transform.position = Controller.singleton.GetCursorWorldPosition() + cursorOffset;
    }
}
