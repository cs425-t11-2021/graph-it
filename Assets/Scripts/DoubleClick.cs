//All code developed by Team 11

// CODE COPIED FROM OLD PROJECT
// TODO: WRITE NEW CODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClick : MonoBehaviour
{
	private float timerForDoubleClick = 0.0f;
	private float delay = 0.3f;
	private bool isDoubleClick = false;

	private bool PointerOverNode = false;
	private bool PointerOverEdge = false;

	// Singleton Pattern
	public static DoubleClick singleton;
    private void Awake()
    {
		// Singleton pattern setup
        if (DoubleClick.singleton == null)
        {
            DoubleClick.singleton = this;
        }
        else
        {
            Debug.LogError("[DoubleClick] Singleton pattern violation");
            Destroy(this);
            return;
        }
    }

    private void Update()
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex", "Edge"));  //11f since camera is at z = -10
        if (hit) {
            return;
        }

        if (Controller.singleton.UIActive()) {
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

		if (!PointerOverNode && !PointerOverEdge && Input.GetMouseButtonDown(0))
		{
			if (isDoubleClick || Toolbar.singleton.CreateVertexMode)
			{
				timerForDoubleClick = 0.0f;
				isDoubleClick = false;

				Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
				Controller.singleton.Graph.AddVertex(mousePos.x, mousePos.y);
                Controller.singleton.UpdateGraphObjs();
                SelectionManager.singleton.DeSelectAll();
				GraphInfo.singleton.UpdateGraphInfo();
			}
			else
			{
				isDoubleClick = true;
			}
		}

	}
}
