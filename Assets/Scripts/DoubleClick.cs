//All code developed by Team 11

// CODE COPIED FROM OLD PROJECT
// TODO: WRITE NEW CODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClick : SingletonBehavior<DoubleClick>
{
	private float timerForDoubleClick = 0.0f;
	private float delay = 0.3f;
	private bool isDoubleClick = false;

    private void Update()
	{
        if (UIManager.Singleton.CursorOnUI) {
            return;
        }

		if (isDoubleClick == true)
		{
			timerForDoubleClick += Time.deltaTime;
		}

		if (timerForDoubleClick >= delay)
		{
			timerForDoubleClick = 0.0f;
			isDoubleClick = false;
		}

		if (!InputManager.Singleton.CursorOverGraphObj && Input.GetMouseButtonDown(0))
		{
			if (isDoubleClick || Toolbar.Singleton.CreateVertexMode)
			{
				timerForDoubleClick = 0.0f;
				isDoubleClick = false;

				Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
				Controller.Singleton.Graph.AddVertex(mousePos.x, mousePos.y);
                Controller.Singleton.UpdateGraphObjs();
                SelectionManager.Singleton.DeSelectAll();
				GraphInfo.Singleton.UpdateGraphInfo();
			}
			else
			{
				isDoubleClick = true;
			}
		}

	}
}
