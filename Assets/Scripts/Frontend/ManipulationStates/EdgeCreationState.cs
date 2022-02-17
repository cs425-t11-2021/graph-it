using UnityEngine;

// State for adding edges from a given vertex
public class EdgeCreationState : ManipulationState
{
    public override void OnStateEnter() {}

    public override void OnStateExit() {}

    public override void OnVertexClick(GameObject clicked)
    {
        VertexObj vertex = clicked.GetComponent<VertexObj>();
        SelectionManager.Singleton.AddEdge(vertex);
    }
}
