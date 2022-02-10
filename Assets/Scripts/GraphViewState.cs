using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphViewState : ManipulationState
{
    public override void OnStateEnter() {}

    public override void OnStateExit() {}

    public override void OnDoubleClick()
    {
        if (InputManager.Singleton.CursorOverGraphObj) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Controller.Singleton.Graph.AddVertex(mousePos.x, mousePos.y);
        Controller.Singleton.UpdateGraphObjs();
        SelectionManager.Singleton.DeSelectAll();
        GraphInfo.Singleton.UpdateGraphInfo();
    }
}
