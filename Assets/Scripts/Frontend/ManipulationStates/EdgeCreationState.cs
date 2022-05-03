using UnityEngine;

// Manipulation state activated from the toolbar for quickly adding new edges between vertices. When shift is held, enable the
// creation of multiple edges from a single vertex.
public class EdgeCreationState : ManipulationState
{
    private VertexObj startingVertex;
    private EdgeTemplate edgeTemplate;

    public override void OnStateEnter()
    {
        InputManager.Singleton.OnMouseRightClick += OnRightClick;
    }

    public override void OnStateExit()
    {
        InputManager.Singleton.OnMouseRightClick -= OnRightClick;
        if (this.edgeTemplate != null)
            GameObject.Destroy(edgeTemplate.gameObject);
    }

    public override void OnVertexClick(GameObject clicked)
    {
        if (this.startingVertex == null) {
            this.startingVertex = clicked.GetComponent<VertexObj>();
            this.edgeTemplate = GameObject.Instantiate(Controller.Singleton.edgeTemplatePrefab, InputManager.Singleton.CursorWorldPosition, Quaternion.identity).GetComponent<EdgeTemplate>();
            this.edgeTemplate.Initiate(this.startingVertex.transform.position);
            this.edgeTemplate.Directed = false;
        }
        else {
            if (InputManager.Singleton.HoldSelectionKeyHeld) {
                Controller.Singleton.AddEdge(startingVertex.Vertex, clicked.GetComponent<VertexObj>().Vertex, this.edgeTemplate.Directed);
                GameObject.Destroy(this.edgeTemplate.gameObject);

                this.edgeTemplate = GameObject.Instantiate(Controller.Singleton.edgeTemplatePrefab, InputManager.Singleton.CursorWorldPosition, Quaternion.identity).GetComponent<EdgeTemplate>();
                this.edgeTemplate.Initiate(this.startingVertex.transform.position);
                this.edgeTemplate.Directed = false;
            }
            else {
                Controller.Singleton.AddEdge(startingVertex.Vertex, clicked.GetComponent<VertexObj>().Vertex, this.edgeTemplate.Directed);
                GameObject.Destroy(this.edgeTemplate.gameObject);
                this.startingVertex = null;
                this.edgeTemplate = null;
            }
        }        
    }

    public override void OnClick()
    {
        if (this.edgeTemplate != null && !InputManager.Singleton.CurrentHoveringVertex) {
            this.edgeTemplate.Directed = !this.edgeTemplate.Directed;
        }
    }

    public void OnRightClick()
    {
        if (this.edgeTemplate != null) {
            GameObject.Destroy(edgeTemplate.gameObject);
            this.edgeTemplate = null;
        }
        this.startingVertex = null;
    }
}
