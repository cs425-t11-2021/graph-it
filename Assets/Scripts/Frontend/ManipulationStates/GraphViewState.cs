using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphViewState : ManipulationState
{
    private bool graphMovementInProgress = false;

    public override void OnStateEnter() {
        InputManager.Singleton.OnMouseDragStart += OnDragStart;
        InputManager.Singleton.OnMouseDragEnd += OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace += OnClickInPlace;
        InputManager.Singleton.OnDeleteKeyPress += OnDeleteKeyPress;
    }

    public override void OnStateExit() {
        InputManager.Singleton.OnMouseDragStart -= OnDragStart;
        InputManager.Singleton.OnMouseDragEnd -= OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace -= OnClickInPlace;
        InputManager.Singleton.OnDeleteKeyPress -= OnDeleteKeyPress;
    }

    public override void OnDoubleClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        SelectionManager.Singleton.DeSelectAll();
        Controller.Singleton.AddVertex(InputManager.Singleton.CursorWorldPosition);
    }

    public void OnClickInPlace() {
        if (InputManager.Singleton.CurrentHoveringVertex) {
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();

            if (!vertex.Selected && !InputManager.Singleton.HoldSelectionKeyHeld) {
                SelectionManager.Singleton.DeSelectAll();
            }

            SelectionManager.Singleton.ToggleVertexSelection(vertex);
        }
        else if (InputManager.Singleton.CurrentHoveringEdge) {
            EdgeObj edge = InputManager.Singleton.CurrentHoveringEdge.GetComponent<EdgeObj>();

            if (!edge.Selected && !InputManager.Singleton.HoldSelectionKeyHeld) {
                SelectionManager.Singleton.DeSelectAll();
            }

            SelectionManager.Singleton.ToggleEdgeSelection(edge);
        }
        else {
            if (!InputManager.Singleton.HoldSelectionKeyHeld) {
                SelectionManager.Singleton.DeSelectAll();
            }
        }
    }

    public void OnDragStart() {
        if (InputManager.Singleton.CurrentHoveringVertex) {
            graphMovementInProgress = true;
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();

            if (!vertex.Selected)
                SelectionManager.Singleton.DeSelectAll();

            SelectionManager.Singleton.SelectVertex(vertex);
        }
        else if (InputManager.Singleton.CurrentHoveringEdge) {
            graphMovementInProgress = true;
        }
        else {
            return;
        }

        foreach (VertexObj selectedVertex in SelectionManager.Singleton.SelectedVertices) {
            selectedVertex.GetComponent<VertexMovement>().FollowCursor = true;

            if (Grid.Singleton.GridEnabled)
            {
                Grid.Singleton.RemoveFromOccupied(selectedVertex);
                Grid.Singleton.DisplayGridLines();
            }
        }
    }

    public void OnDragEnd() {
        if (!graphMovementInProgress) return;

        foreach (VertexObj selectedVertex in SelectionManager.Singleton.SelectedVertices) {
            selectedVertex.GetComponent<VertexMovement>().FollowCursor = false;

            if (Grid.Singleton.GridEnabled)
            {
                selectedVertex.transform.position = Grid.Singleton.FindClosestGridPosition(selectedVertex);
                Grid.Singleton.HideGridLines();
            }

            selectedVertex.Vertex.x_pos = selectedVertex.transform.position.x;
            selectedVertex.Vertex.y_pos = selectedVertex.transform.position.y;
        }

        graphMovementInProgress = false;
    }

    private void OnDeleteKeyPress() {
        SelectionManager.Singleton.DeleteSelection();
    }
}
