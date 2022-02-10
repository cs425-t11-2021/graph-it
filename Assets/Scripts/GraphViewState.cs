using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphViewState : ManipulationState
{
    public override void OnStateEnter() {
        InputManager.Singleton.OnMouseDragStart += OnDragStart;
        InputManager.Singleton.OnMouseDragEnd += OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace += OnClickInPlace;
    }

    public override void OnStateExit() {
        InputManager.Singleton.OnMouseDragStart -= OnDragStart;
        InputManager.Singleton.OnMouseDragEnd -= OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace -= OnClickInPlace;
    }

    public override void OnDoubleClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Controller.Singleton.Graph.AddVertex(mousePos.x, mousePos.y);
        Controller.Singleton.UpdateGraphObjs();
        SelectionManager.Singleton.DeSelectAll();
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    public void OnClickInPlace() {
        if (InputManager.Singleton.CurrentHoveringVertex) {
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();
            vertex.SetSelected(!vertex.selected);
        }
    }

    public void OnDragStart() {
        if (InputManager.Singleton.CurrentHoveringVertex) {
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();
            if (!vertex.selected)
                vertex.SetSelected(true);
        }

        foreach (VertexObj selectedVertex in SelectionManager.Singleton.selectedVertices) {
            selectedVertex.GetComponent<VertexMovement>().FollowCursor = true;

            if (Grid.singleton.GridEnabled)
            {
                Grid.singleton.RemoveFromOccupied(selectedVertex);
                Grid.singleton.DisplayGridLines();
            }
        }
    }

    public void OnDragEnd() {
        foreach (VertexObj selectedVertex in SelectionManager.Singleton.selectedVertices) {
            selectedVertex.GetComponent<VertexMovement>().FollowCursor = false;

            if (Grid.singleton.GridEnabled)
            {
                selectedVertex.transform.position = Grid.singleton.FindClosestGridPosition(selectedVertex);
                Grid.singleton.HideGridLines();
            }

            selectedVertex.Vertex.x_pos = selectedVertex.transform.position.x;
            selectedVertex.Vertex.y_pos = selectedVertex.transform.position.y;
        }
    }
}
