using UnityEngine;

// Selection state, left click to select multiple components, left click and drag on graph to select using the selection box
public class SelectionState : ManipulationState
{
    // Reference to the selection box
    private RectTransform selectionRect;
    // Stored last cursor world position
    private Vector3 lastCursorWorldPos;

    public override void OnStateEnter()
    {
        // Get reference and deactivate the selection box
        this.selectionRect = UIManager.Singleton.selectionRect.GetComponent<RectTransform>();
        this.selectionRect.gameObject.SetActive(false);
        // Subscribe to appropriate input event
        InputManager.Singleton.OnMouseClickInPlace += OnClickInPlace;
    }

    public override void OnStateExit()
    {
        this.selectionRect.gameObject.SetActive(false);
        InputManager.Singleton.OnMouseClickInPlace -= OnClickInPlace;
    }

    public override void OnClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        // Activate the selection box and update its position when clicked
        this.lastCursorWorldPos = InputManager.Singleton.CursorWorldPosition;
        UpdateSelectionRect();
        this.selectionRect.gameObject.SetActive(true);
    }

    public void OnClickInPlace() {
        // If graph objects are clicked, select them
        if (InputManager.Singleton.CurrentHoveringVertex) {
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();
            SelectionManager.Singleton.ToggleVertexSelection(vertex);
        }
        else if (InputManager.Singleton.CurrentHoveringEdge) {
            EdgeObj edge = InputManager.Singleton.CurrentHoveringEdge.GetComponent<EdgeObj>();
            SelectionManager.Singleton.ToggleEdgeSelection(edge);
        }
    }

    public override void OnMouseHold()
    {
        // While mouse is being held, update the position of the selection box
        if (this.selectionRect.gameObject.activeInHierarchy) {
            UpdateSelectionRect();
        }
    }

    public override void OnMouseRelease()
    {
        // When mouse is released, calcualte all objects that fall within the selection box and select them
        Bounds bounds = UpdateSelectionRect();
        foreach (VertexObj v in Controller.Singleton.VertexObjs) {
            if (bounds.Contains(v.transform.position))
                SelectionManager.Singleton.SelectVertex(v);
        }
        foreach (EdgeObj e in Controller.Singleton.EdgeObjs) {
            if (bounds.Contains((Vector2) e.transform.position)) {
                SelectionManager.Singleton.SelectEdge(e);
            }
        }

        this.selectionRect.gameObject.SetActive(false);
    }

    // Method for updating the size and position of the selection box given the current cursor position
    private Bounds UpdateSelectionRect() {
        Vector2 currentMouseWorldPos = InputManager.Singleton.CursorWorldPosition;
        Vector2 middle = (InputManager.Singleton.CursorWorldPosition + lastCursorWorldPos) / 2f;
        Vector2 size = new Vector2(Mathf.Abs(currentMouseWorldPos.x - lastCursorWorldPos.x), Mathf.Abs(currentMouseWorldPos.y - lastCursorWorldPos.y));

        this.selectionRect.position = middle;
        this.selectionRect.sizeDelta = size;

        Bounds worldBounds = new Bounds(middle, size);
        return worldBounds;
    }
}
