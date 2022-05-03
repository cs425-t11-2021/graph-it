using UnityEngine;

// Manipulation state which represents when the user double clicks on an vertex to create a new edge. Clicking on a blank part
// of the graph toggles between directed/undirected edge.
public class EdgeDrawingState : ManipulationState
{
    private VertexObj startingVertex;
    private EdgeTemplate edgeTemplate;

    public override void OnStateEnter()
    {
        InputManager.Singleton.OnMouseRightClick += OnRightClick;
        
        this.startingVertex = InputManager.Singleton.CurrentHoveringVertex.GetComponent<VertexObj>();

        this.edgeTemplate = GameObject.Instantiate(Controller.Singleton.edgeTemplatePrefab, InputManager.Singleton.CursorWorldPosition, Quaternion.identity).GetComponentInChildren<EdgeTemplate>();
        this.edgeTemplate.Initiate(this.startingVertex.transform.position);
        this.edgeTemplate.Directed = false;
    }

    public override void OnStateExit()
    {
        InputManager.Singleton.OnMouseRightClick -= OnRightClick;
        
        GameObject.Destroy(edgeTemplate.transform.parent.gameObject);
    }

    public override void OnClick()
    {
        if (!InputManager.Singleton.CurrentHoveringVertex) {
            this.edgeTemplate.Directed = !this.edgeTemplate.Directed;
        }
    }

    public void OnRightClick()
    {
        Toolbar.Singleton.EnterViewMode();
    }

    public override void OnVertexClick(GameObject clicked)
    {
        Controller.Singleton.AddEdge(startingVertex.Vertex, clicked.GetComponent<VertexObj>().Vertex, this.edgeTemplate.Directed);
        Toolbar.Singleton.EnterViewMode();
    }
}
