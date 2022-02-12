using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCreationState : ManipulationState
{

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        SelectionManager.Singleton.DeSelectAll();
        Controller.Singleton.AddVertex(InputManager.Singleton.CursorWorldPosition);
    }
}
