using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCreationState : ManipulationState
{
    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnVertexClick(GameObject clicked)
    {
        VertexObj vertex = clicked.GetComponent<VertexObj>();
        SelectionManager.Singleton.AddEdge(vertex);
    }
}
