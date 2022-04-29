using System;
using UnityEngine;

// Central class for managing mouse and keyboard input using C# events
public class InputManager : SingletonBehavior<InputManager>
{
    // Actions associated with various input related events
    public event Action OnMouseClick;
    public event Action OnMouseDoubleClick;
    public event Action OnMouseHold;
    public event Action OnMouseRelease;
    public event Action OnMouseRightClick;
    public event Action<GameObject> OnVertexClick;
    public event Action<GameObject> OnEdgeClick;
    public event Action OnMouseDragStart;
    public event Action OnMouseDragEnd;
    public event Action OnMouseClickInPlace;
    public event Action OnDeleteKeyPress;

    // Duration between clicks that counts as a double click
    [SerializeField] private float doubleClickTimeDelta = 0.2f;
    private float timeOfLastClick = 0f;
    // Stored mouse position used to detect a drag
    private Vector3 mouseLastClickPosition;
    private bool dragging = false;
    private bool doubleClickedThisFrame = false;

    // Property for whether or not the hold selection key is being held(shift by default)
    public bool HoldSelectionKeyHeld {
        get {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }

    // Property for whether or not Control/Command key is being held
    public bool ControlCommandKeyHeld {
        get {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand);
        }
    }
    
    // Whether or not the left mouse button is held
    public bool LeftMouseButtonHeld
    {
        get
        {
            return Input.GetMouseButton(0);
        }
    }

    // Property for getting the vertex object the cursor is currently hovering
    public GameObject CurrentHoveringVertex {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Vertex"));
            if (hit.collider) {
                return hit.collider.gameObject;
            }
            return null;
        }
    }

    // Property for getting the edge object the cursor is currently hovering
    public GameObject CurrentHoveringEdge {
        get {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(cursorPos, Vector2.zero, 0f, LayerMask.GetMask("Edge"));
            if (hit.collider) {
                return hit.collider.gameObject;
            }
            return null;
        }
    }

    // Property to detect whether the cursor is over a graph object
    public bool CursorOverGraphObj {
        get {
            return CurrentHoveringVertex || CurrentHoveringEdge;
        }
    }

    // Property to detect whether or not a mouse button was pressed
    public bool MouseButtonPressedThisFrame {
        get {
            return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0);
        }
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    public Vector3 CursorWorldPosition {
        get => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (!UIManager.Singleton.CursorOnUI) {
                // Double click detection
                if (Time.time - timeOfLastClick < doubleClickTimeDelta) {
                    doubleClickedThisFrame = true;
                    OnMouseDoubleClick?.Invoke();
                }
                else {
                    // Single click detection
                    OnMouseClick?.Invoke();

                    // Vertex click detection
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

            // Mouse drag start detection
            if (!this.doubleClickedThisFrame && !this.dragging && Input.mousePosition != this.mouseLastClickPosition) {
                this.dragging = true;
                OnMouseDragStart?.Invoke();
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            if (!doubleClickedThisFrame) {
                OnMouseRelease?.Invoke();

                /// Mouse drag end detection
                if (Input.mousePosition != this.mouseLastClickPosition) {
                    OnMouseDragEnd?.Invoke();
                }
                else {
                    // Non mouse drag detection
                    if (!UIManager.Singleton.CursorOnUI) {
                        OnMouseClickInPlace?.Invoke();
                    }
                }
            }
            this.dragging = false;
            this.doubleClickedThisFrame = false;
        }
        else if (Input.GetMouseButton(1))
        {
            OnMouseRightClick?.Invoke();
        }

        // Keyboard events detection and firing
        if (!UIManager.Singleton.CursorOnUI) {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)) OnDeleteKeyPress?.Invoke();
        }
    }
}
