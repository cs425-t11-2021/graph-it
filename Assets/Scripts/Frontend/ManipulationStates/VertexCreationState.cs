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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Controller.Singleton.Graph.AddVertex(mousePos.x, mousePos.y);
        Controller.Singleton.UpdateGraphObjs();
        SelectionManager.Singleton.DeSelectAll();
        GraphInfo.Singleton.UpdateGraphInfo();
    }
}
