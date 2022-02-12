using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionState : ManipulationState
{
    private RectTransform selectionRect;

    private Vector3 lastCursorWorldPos;

    public override void OnStateEnter()
    {
        this.selectionRect = UIManager.Singleton.selectionRect.GetComponent<RectTransform>();
        this.selectionRect.gameObject.SetActive(false);

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

        this.lastCursorWorldPos = InputManager.Singleton.CursorWorldPosition;
        UpdateSelectionRect();
        this.selectionRect.gameObject.SetActive(true);
    }

    public void OnClickInPlace() {
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
        if (this.selectionRect.gameObject.activeInHierarchy) {
            UpdateSelectionRect();
        }
    }

    public override void OnMouseRelease()
    {
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
