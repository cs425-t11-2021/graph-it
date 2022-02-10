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
    }

    public override void OnStateExit()
    {
        this.selectionRect.gameObject.SetActive(false);
    }

    public override void OnClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        this.lastCursorWorldPos = Controller.Singleton.GetCursorWorldPosition();
        UpdateSelectionRect();
        this.selectionRect.gameObject.SetActive(true);
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
        VertexObj[] vertexObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<VertexObj>();
        foreach (VertexObj v in vertexObjs) {
            if (bounds.Contains(v.transform.position)) {
                v.SetSelected(true);
            }
        }
        EdgeObj[] edgeObjs = Controller.Singleton.GraphObj.GetComponentsInChildren<EdgeObj>();
        foreach (EdgeObj e in edgeObjs) {
            if (bounds.Contains((Vector2) e.transform.position)) {
                e.SetSelected(true);
            }
        }

        this.selectionRect.gameObject.SetActive(false);
    }

    private Bounds UpdateSelectionRect() {
        Vector2 currentMouseWorldPos = Controller.Singleton.GetCursorWorldPosition();
        Vector2 middle = (Controller.Singleton.GetCursorWorldPosition() + lastCursorWorldPos) / 2f;
        Vector2 size = new Vector2(Mathf.Abs(currentMouseWorldPos.x - lastCursorWorldPos.x), Mathf.Abs(currentMouseWorldPos.y - lastCursorWorldPos.y));

        this.selectionRect.position = middle;
        this.selectionRect.sizeDelta = size;

        Bounds worldBounds = new Bounds(middle, size);
        return worldBounds;
    }
}
