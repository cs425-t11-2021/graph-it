// Default viewing state, double click to add vertex, left click to select, shift + left click to select multiple
public class GraphViewState : ManipulationState
{
    // Whether or not graph objects are current being moved
    private bool graphMovementInProgress = false;

    // Subscribe to appropriate input events on state enter
    public override void OnStateEnter() {
        InputManager.Singleton.OnMouseDragStart += OnDragStart;
        InputManager.Singleton.OnMouseDragEnd += OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace += OnClickInPlace;
        InputManager.Singleton.OnDeleteKeyPress += OnDeleteKeyPress;
    }

    // Unsubscribe to input events on state exit
    public override void OnStateExit() {
        InputManager.Singleton.OnMouseDragStart -= OnDragStart;
        InputManager.Singleton.OnMouseDragEnd -= OnDragEnd;
        InputManager.Singleton.OnMouseClickInPlace -= OnClickInPlace;
        InputManager.Singleton.OnDeleteKeyPress -= OnDeleteKeyPress;
    }

    // Add a new vertex on double click
    public override void OnDoubleClick()
    {
        // Do not add new vertex if double clicking on an existing vertex
        if (InputManager.Singleton.CursorOverGraphObj) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.edgeDrawingState;
        }
        else {
            SelectionManager.Singleton.DeSelectAll();
            Controller.Singleton.AddVertex(InputManager.Singleton.CursorWorldPosition);
        }
    }

    // Clicked and not dragged
    public void OnClickInPlace() {
        // If a vertex or edge object is clicked, select/deselect it
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
        // If a vertex or edge object is not clicked, deselect all
        else {
            if (!InputManager.Singleton.HoldSelectionKeyHeld) {
                SelectionManager.Singleton.DeSelectAll();
            }
        }
    }

    public void OnDragStart() {
        // If mouse drag starts on a vertex or edge, start dragging the component
        if (InputManager.Singleton.CurrentHoveringVertex) {
            this.graphMovementInProgress = true;
            VertexObj vertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();

            if (!vertex.Selected)
                SelectionManager.Singleton.DeSelectAll();

            SelectionManager.Singleton.SelectVertex(vertex);
        }
        else if (InputManager.Singleton.CurrentHoveringEdge) {
            this.graphMovementInProgress = true;
        }
        else {
            return;
        }

        // Tell all of the vertex objects to follow cursor, and remove it from the grid
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
        if (!this.graphMovementInProgress) return;

        // Tell the vertices to stop following the cursor and add them back to the grid
        foreach (VertexObj selectedVertex in SelectionManager.Singleton.SelectedVertices) {
            selectedVertex.GetComponent<VertexMovement>().FollowCursor = false;

            if (Grid.Singleton.GridEnabled)
            {
                selectedVertex.transform.position = Grid.Singleton.FindClosestGridPosition(selectedVertex);
                Grid.Singleton.HideGridLines();
            }

            // Update the stored position info in the verterices
            selectedVertex.Vertex.X = selectedVertex.transform.position.x;
            selectedVertex.Vertex.Y = selectedVertex.transform.position.y;
        }

        this.graphMovementInProgress = false;
    }

    // Delete current selection when the delete key is pressed
    private void OnDeleteKeyPress() {
        SelectionManager.Singleton.DeleteSelection();
    }
}
