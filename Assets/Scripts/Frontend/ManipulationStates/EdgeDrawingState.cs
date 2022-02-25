using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDrawingState : ManipulationState
{
    private VertexObj startingVertex;
    private EdgeTemplate edgeTemplate;

    public override void OnStateEnter()
    {
        InputManager.Singleton.OnMouseRightClick += OnRightClick;
        
        this.startingVertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();

        this.edgeTemplate = GameObject.Instantiate(Controller.Singleton.edgeTemplatePrefab, InputManager.Singleton.CursorWorldPosition, Quaternion.identity).GetComponent<EdgeTemplate>();
        this.edgeTemplate.Initiate(this.startingVertex.transform.position);
        this.edgeTemplate.Directed = false;
    }

    public override void OnStateExit()
    {
        InputManager.Singleton.OnMouseRightClick -= OnRightClick;
        
        GameObject.Destroy(edgeTemplate.gameObject);
    }

    public override void OnClick()
    {
        if (!InputManager.Singleton.CurrentHoveringVertex) {
            this.edgeTemplate.Directed = !this.edgeTemplate.Directed;
        }
    }

    public void OnRightClick()
    {
        ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
    }

    public override void OnVertexClick(GameObject clicked)
    {
        Controller.Singleton.AddEdge(startingVertex.Vertex, clicked.GetComponent<VertexObj>().Vertex, this.edgeTemplate.Directed);
        ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
    }
}
