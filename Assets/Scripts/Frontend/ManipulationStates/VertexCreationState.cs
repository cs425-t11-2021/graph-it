// Manipulation state enabling quickly creating vertices, selected from the toolbar. Left click on graph to create a new vertex.
public class VertexCreationState : ManipulationState
{
    public override void OnStateEnter()
    {
        Grid.Singleton.DisplayGridLines();
    }

    public override void OnStateExit()
    {
        Grid.Singleton.HideGridLines();
    }

    public override void OnClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        // When an empty space on the graph is clicked, put a new vertex there
        SelectionManager.Singleton.DeSelectAll();
        Controller.Singleton.AddVertex(InputManager.Singleton.CursorWorldPosition);
    }
}
