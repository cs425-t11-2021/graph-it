using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonBehavior<InputManager>
{
    // Property to detect whether the cursor is over a vertex using Raycast 2D
    public bool CursorOverVertex {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, LayerMask.GetMask("Edge"));
            if (hit.collider) {
                Logger.Log("Cursor over vertex.", this, LogType.DEBUG);
                return true;
            }
            return false;
        }
    }

    // Property to detect whether the cursor is over an edge using Raycast 2D
    public bool CursorOverEdge {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, LayerMask.GetMask("Edge"));
            if (hit.collider) {
                Logger.Log("Cursor over edge.", this, LogType.DEBUG);
                return true;
            }
            return false;
        }
    }

    // Property to detect whether the cursor is over a graph object
    public bool CursorOverGraphObj {
        get {
            return CursorOverEdge || CursorOverVertex;
        }
    }

    // Property to detect whether or not a mouse button was pressed
    public bool MouseButtonPressedThisFrame {
        get {
            return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0);
        }
    }

    private void Update() {
        bool test = CursorOverEdge;
        bool test2 = CursorOverVertex;
    }
}
