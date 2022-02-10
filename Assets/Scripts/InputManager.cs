using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonBehavior<InputManager>
{

    public event Action OnMouseClick;
    public event Action OnMouseDoubleClick;
    public event Action OnMouseHold;
    public event Action OnMouseRelease;
    public event Action<GameObject> OnVertexClick;
    public event Action<GameObject> OnEdgeClick;
    public event Action OnMouseDragStart;
    public event Action OnMouseDragEnd;
    public event Action OnMouseClickInPlace;
    public event Action OnDeleteKeyPress;

    [SerializeField] private float doubleClickTimeDelta = 0.2f;
    private float timeOfLastClick = 0f;
    private Vector3 mouseLastClickPosition;
    private bool dragging = false;
    private bool doubleClickedThisFrame = false;

    // Property to detect whether the cursor is over a vertex using Raycast 2D
    public bool CursorOverVertex {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Edge"));
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
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Edge"));
            if (hit.collider) {
                Logger.Log("Cursor over edge.", this, LogType.DEBUG);
                return true;
            }
            return false;
        }
    }

    // Property for whether or not the hold selection key is being held(shift by default)
    public bool HoldSelectionKeyHeld {
        get {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }

    public GameObject CurrentHoveringVertex {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Vertex"));
            if (hit.collider) {
                Logger.Log("Cursor over vertex.", this, LogType.DEBUG);
                return hit.collider.gameObject;
            }
            return null;
        }
    }

    public GameObject CurrentHoveringEdge {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Edge"));
            if (hit.collider) {
                Logger.Log("Cursor over edge.", this, LogType.DEBUG);
                return hit.collider.gameObject;
            }
            return null;
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
        if (Input.GetMouseButtonDown(0)) {
            if (!UIManager.Singleton.CursorOnUI) {
                if (Time.time - timeOfLastClick < doubleClickTimeDelta) {
                    Logger.Log("Double click detected.", this, LogType.DEBUG);
                    doubleClickedThisFrame = true;
                    OnMouseDoubleClick?.Invoke();
                }
                else {
                    Logger.Log("Click detected.", this, LogType.DEBUG);
                    OnMouseClick?.Invoke();

                    GameObject hoveringVertex = CurrentHoveringVertex;
                    if (hoveringVertex) {
                        OnVertexClick?.Invoke(hoveringVertex);
                    }
                }
            }

            this.timeOfLastClick = Time.time;
            this.mouseLastClickPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) {
            OnMouseHold?.Invoke();

            if (!this.dragging && Input.mousePosition != this.mouseLastClickPosition) {
                Logger.Log("Mouse drag started.", this, LogType.DEBUG);
                this.dragging = true;
                OnMouseDragStart?.Invoke();
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            if (!doubleClickedThisFrame) {
                OnMouseRelease?.Invoke();

                if (Input.mousePosition != this.mouseLastClickPosition) {
                    Logger.Log("Mouse drag finished.", this, LogType.DEBUG);
                    OnMouseDragEnd?.Invoke();
                }
                else {
                    if (!UIManager.Singleton.CursorOnUI) {
                        Logger.Log("Click in place detected.", this, LogType.DEBUG);
                        OnMouseClickInPlace?.Invoke();
                    }
                }
            }
            this.dragging = false;
            this.doubleClickedThisFrame = false;
        }

        // Keyboard events detection and firing
        if (!UIManager.Singleton.CursorOnUI) {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)) OnDeleteKeyPress?.Invoke();
        }
    }
}
