// Supress class name conflict warning
#pragma warning disable 0436

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behavior for handling dragging graph objects, requires colliders to be attached to the objects, calls OnDragStart and OnMouseDownNonDrag methods
// on any attached components that implment IUsesDragEvents
public class ObjDrag : MonoBehaviour
{
    // The current distance between the curosr to the center of the object
    private Vector3 cursorOffset;

    // List of components on the current game object which uses drag events
    private IUsesDragEvents[] dragEventHandlers;

    private bool clicked = false;
    private float dragDuration;

    private void Awake()
    {
        // Setup references to all drag event handlers
        dragEventHandlers = GetComponents<IUsesDragEvents>();
    }


    // When the mouse is first pressed, store the cursor offset in relation to the transform position
    private void OnMouseDown()
    {
        // cursorOffset = this.transform.position - Controller.singleton.GetCursorWorldPosition();

        // When a click is detected, set clicked to true and store the current mouse position, and reset the drag duration timer
        dragDuration = 0f;
        clicked = true;
    }

    // When the object is dragged, set its position to be cursor position with offset
    private void OnMouseDrag()
    {
        // Increase the drag duration timer while the object is being dragged
        if (clicked)
        {
            dragDuration += Time.deltaTime;

            // If the object is held for more than 1/10 of a second, count as the start of a drag
            if (dragDuration > .1f)
            {
                clicked = false;
                foreach (IUsesDragEvents handler in dragEventHandlers)
                {
                    handler.OnDragStart();
                }
            }
        }
    }

    private void OnMouseUp()
    {
        clicked = false;
        // If the object has been dragged for less than 1/10 of a second, count as a non-dragging click
        if (dragDuration < .1f)
        {
            foreach (IUsesDragEvents handler in dragEventHandlers)
            {
                handler.OnMouseDownNonDrag();
            }
        }
        else
        {
            foreach (IUsesDragEvents handler in dragEventHandlers)
            {
                handler.OnDragFinish();
            }
        }
        dragDuration = 0f;
    }
}
