// State for quickly creating vertices, left click on graph to create a new vertex
public class VertexCreationState : ManipulationState
{
    public override void OnStateEnter() {}

    public override void OnStateExit() {}

    public override void OnClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        // When an empty space on the graph is clicked, put a new vertex there
        SelectionManager.Singleton.DeSelectAll();
        Controller.Singleton.AddVertex(InputManager.Singleton.CursorWorldPosition);
    }
}
